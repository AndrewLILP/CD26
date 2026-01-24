using UnityEngine;

/// <summary>
/// Attach to NPCs to enable dialogue interactions
/// Detects player proximity and triggers dialogue UI
/// SPRINT 4 ENHANCED: Uncle Ray post-lap dialogue selection based on personal best
/// </summary>
[RequireComponent(typeof(Collider))]
public class NPCInteraction : MonoBehaviour
{
    [Header("NPC Configuration")]
    [Tooltip("The dialogue data for this NPC")]
    public DialogueData dialogueData;
    
    [Tooltip("Unique identifier for this NPC (e.g., 'maria_cafe', 'uncle_ray_garage')")]
    public string npcID = "npc_001";
    
    [Header("Interaction Settings")]
    [Tooltip("Distance at which prompt appears")]
    [SerializeField] private float interactionRadius = 3.0f;
    
    [Tooltip("Only allow interaction when walking (not driving)")]
    public bool walkingOnly = true;
    
    [Tooltip("Can this NPC be talked to multiple times?")]
    public bool repeatableDialogue = true;
    
    [Header("Mission Integration")]
    [Tooltip("Optional: Mission that must be active to talk to this NPC")]
    public string requiredMissionID = "";
    
    [Header("Post-Lap Dialogue (Uncle Ray Only)")]
    [Tooltip("Is this Uncle Ray at the garage? (enables post-lap dialogue selection)")]
    public bool isUncleRayGarage = false;
    
    [Tooltip("Dialogue to show if personal best was beaten")]
    public DialogueData postLapBestDialogue;
    
    [Tooltip("Dialogue to show if personal best was NOT beaten")]
    public DialogueData postLapRegularDialogue;
    
    [Header("State")]
    [SerializeField] private bool hasBeenTalkedTo = false;
    [SerializeField] private bool playerNearby = false;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    
    void Start()
    {
        // Ensure this GameObject has a trigger collider
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"NPCInteraction '{name}': Collider is not a trigger! Setting isTrigger = true.");
            col.isTrigger = true;
        }
        
        if (dialogueData == null)
        {
            Debug.LogError($"NPCInteraction '{name}': No DialogueData assigned!");
        }
        
        // Validate Uncle Ray setup
        if (isUncleRayGarage)
        {
            if (postLapBestDialogue == null)
            {
                Debug.LogError($"NPCInteraction '{name}': isUncleRayGarage is true but postLapBestDialogue not assigned!");
            }
            if (postLapRegularDialogue == null)
            {
                Debug.LogError($"NPCInteraction '{name}': isUncleRayGarage is true but postLapRegularDialogue not assigned!");
            }
        }
    }
    
    void Update()
    {
        // Check for E key press when player is nearby
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"[NPC '{npcID}'] E key pressed! playerNearby={playerNearby}");
            
            if (playerNearby)
            {
                Debug.Log($"[NPC '{npcID}'] Player is nearby, checking CanInteract()...");
                if (CanInteract())
                {
                    TriggerDialogue();
                }
            }
            else
            {
                Debug.Log($"[NPC '{npcID}'] Player not nearby, ignoring E press");
            }
        }
        
        // Update prompt display
        UpdatePromptDisplay();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other))
        {
            playerNearby = true;
            Debug.Log($"[NPC '{npcID}'] Player entered interaction radius");
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other))
        {
            playerNearby = false;
            HidePrompt();
            Debug.Log($"[NPC '{npcID}'] Player left interaction radius");
        }
    }
    
    /// <summary>
    /// Check if the collider belongs to the player character
    /// </summary>
    private bool IsPlayer(Collider other)
    {
        // Check for character controller in parent hierarchy
        if (other.GetComponentInParent<StarterAssets.ThirdPersonController>() != null)
        {
            return true;
        }
        
        // Fallback: check Player tag
        if (other.CompareTag("Player"))
        {
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Check if interaction is currently allowed
    /// </summary>
    private bool CanInteract()
    {
        // Check if already talked to
        if (hasBeenTalkedTo && !repeatableDialogue)
        {
            Debug.Log($"[NPC '{npcID}'] ✗ Cannot interact: Already talked to (not repeatable)");
            return false;
        }
        
        // Check if player is in correct state
        if (walkingOnly && PlayerStateManager.Instance != null)
        {
            if (PlayerStateManager.Instance.CurrentState != PlayerStateManager.PlayerState.Walking)
            {
                Debug.Log($"[NPC '{npcID}'] ✗ Cannot interact: Player not in Walking state (current: {PlayerStateManager.Instance.CurrentState})");
                return false;
            }
        }
        
        // Check if required mission is active
        if (!string.IsNullOrEmpty(requiredMissionID))
        {
            if (MissionManager.Instance == null)
            {
                Debug.LogError($"[NPC '{npcID}'] ✗ Cannot interact: MissionManager.Instance is null!");
                return false;
            }
            
            MissionData currentMission = MissionManager.Instance.GetCurrentMission();
            if (currentMission == null)
            {
                Debug.Log($"[NPC '{npcID}'] ✗ Cannot interact: No current mission active (required: '{requiredMissionID}')");
                return false;
            }
            
            if (currentMission.missionID != requiredMissionID)
            {
                Debug.Log($"[NPC '{npcID}'] ✗ Cannot interact: Wrong mission active (required: '{requiredMissionID}', current: '{currentMission.missionID}')");
                return false;
            }
            
            Debug.Log($"[NPC '{npcID}'] ✓ Mission check passed: '{requiredMissionID}' is active");
        }
        
        Debug.Log($"[NPC '{npcID}'] ✓ CanInteract = true");
        return true;
    }
    
    /// <summary>
    /// Update the interaction prompt based on state
    /// </summary>
    private void UpdatePromptDisplay()
    {
        if (!playerNearby)
        {
            return;
        }
        
        if (CanInteract())
        {
            ShowPrompt();
        }
        else
        {
            HidePrompt();
        }
    }
    
    /// <summary>
    /// Show "Press E to Talk" prompt
    /// </summary>
    private void ShowPrompt()
    {
        HUDController hud = FindFirstObjectByType<HUDController>();
        if (hud != null)
        {
            // Determine NPC name for prompt
            string displayName = dialogueData != null ? dialogueData.npcName : "NPC";
            
            // If this is Uncle Ray post-lap, use appropriate dialogue's name
            if (isUncleRayGarage)
            {
                DialogueData activeDialogue = GetPostLapDialogue();
                if (activeDialogue != null)
                {
                    displayName = activeDialogue.npcName;
                }
            }
            
            hud.ShowTalkPrompt(displayName);
        }
    }
    
    /// <summary>
    /// Hide interaction prompt
    /// </summary>
    private void HidePrompt()
    {
        HUDController hud = FindFirstObjectByType<HUDController>();
        if (hud != null)
        {
            hud.HideInteractionPrompt();
        }
    }
    
    /// <summary>
    /// Get the appropriate post-lap dialogue based on LapTimer state
    /// </summary>
    private DialogueData GetPostLapDialogue()
    {
        if (!isUncleRayGarage)
        {
            return dialogueData; // Not Uncle Ray, use default dialogue
        }
        
        // Find LapTimer to check personal best status
        LapTimer lapTimer = FindFirstObjectByType<LapTimer>();
        if (lapTimer == null)
        {
            Debug.LogWarning($"[NPC '{npcID}'] LapTimer not found! Using default dialogue.");
            return dialogueData;
        }
        
        // Select dialogue based on whether personal best was beaten
        if (lapTimer.LastLapBeatPersonalBest)
        {
            Debug.Log($"[NPC '{npcID}'] Personal best beaten → using postLapBestDialogue");
            return postLapBestDialogue != null ? postLapBestDialogue : dialogueData;
        }
        else
        {
            Debug.Log($"[NPC '{npcID}'] Personal best NOT beaten → using postLapRegularDialogue");
            return postLapRegularDialogue != null ? postLapRegularDialogue : dialogueData;
        }
    }
    
    /// <summary>
    /// Trigger the dialogue UI
    /// </summary>
    private void TriggerDialogue()
    {
        Debug.Log($"[NPC '{npcID}'] 🎬 TriggerDialogue() called");
        
        // Determine which dialogue to show
        DialogueData activeDialogue = isUncleRayGarage ? GetPostLapDialogue() : dialogueData;
        
        if (activeDialogue == null)
        {
            Debug.LogError($"[NPC '{npcID}'] ✗ No DialogueData available!");
            return;
        }
        
        Debug.Log($"[NPC '{npcID}'] DialogueData selected: {activeDialogue.npcName}");
        
        // Find and trigger DialogueUIController
        DialogueUIController dialogueUI = FindFirstObjectByType<DialogueUIController>();
        if (dialogueUI != null)
        {
            Debug.Log($"[NPC '{npcID}'] Found DialogueUIController, starting dialogue...");
            dialogueUI.StartDialogue(activeDialogue, this);
            hasBeenTalkedTo = true;
            
            Debug.Log($"[NPC '{npcID}'] ✓ Started dialogue: {activeDialogue.npcName}");
        }
        else
        {
            Debug.LogError($"[NPC '{npcID}'] ✗ DialogueUIController not found in scene!");
        }
    }
    
    /// <summary>
    /// Called by DialogueUIController when dialogue completes
    /// </summary>
    public void OnDialogueComplete()
    {
        Debug.Log($"[NPC '{npcID}'] Dialogue completed");
        
        // Determine which dialogue was just shown
        DialogueData completedDialogue = isUncleRayGarage ? GetPostLapDialogue() : dialogueData;
        
        // If this dialogue advances a mission, notify MissionManager
        if (completedDialogue != null && completedDialogue.advancesMission && MissionManager.Instance != null)
        {
            if (!string.IsNullOrEmpty(completedDialogue.linkedMissionID))
            {
                MissionManager.Instance.CompleteMission(completedDialogue.linkedMissionID);
            }
        }
    }
    
    /// <summary>
    /// Reset interaction state (useful for testing)
    /// </summary>
    [ContextMenu("Reset Interaction")]
    public void ResetInteraction()
    {
        hasBeenTalkedTo = false;
        Debug.Log($"[NPC '{npcID}'] Interaction reset");
    }
    
    void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;
        
        // Draw interaction radius
        Gizmos.color = playerNearby ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
        
        #if UNITY_EDITOR
        // Draw label
        string stateText = hasBeenTalkedTo ? "(Talked)" : "(Available)";
        string npcName = "NO DATA";
        
        if (isUncleRayGarage)
        {
            DialogueData activeDialogue = GetPostLapDialogue();
            npcName = activeDialogue != null ? activeDialogue.npcName + " (Post-Lap)" : "NO DATA";
        }
        else if (dialogueData != null)
        {
            npcName = dialogueData.npcName;
        }
        
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 2.5f,
            $"NPC: {npcName}\n{stateText}"
        );
        #endif
    }
}
