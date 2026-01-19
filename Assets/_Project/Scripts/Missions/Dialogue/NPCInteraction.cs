using UnityEngine;

/// <summary>
/// Attach to NPCs to enable dialogue interactions
/// Detects player proximity and triggers dialogue UI
/// </summary>
[RequireComponent(typeof(Collider))]
public class NPCInteraction : MonoBehaviour
{
    [Header("NPC Configuration")]
    [Tooltip("The dialogue data for this NPC")]
    public DialogueData dialogueData;
    
    [Tooltip("Unique identifier for this NPC (e.g., 'maria_cafe', 'uncle_ray')")]
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
            Debug.Log($"[NPC '{npcID}'] ❌ Cannot interact: Already talked to (not repeatable)");
            return false;
        }
        
        // Check if player is in correct state
        if (walkingOnly && PlayerStateManager.Instance != null)
        {
            if (PlayerStateManager.Instance.CurrentState != PlayerStateManager.PlayerState.Walking)
            {
                Debug.Log($"[NPC '{npcID}'] ❌ Cannot interact: Player not in Walking state (current: {PlayerStateManager.Instance.CurrentState})");
                return false;
            }
        }
        
        // Check if required mission is active
        if (!string.IsNullOrEmpty(requiredMissionID))
        {
            if (MissionManager.Instance == null)
            {
                Debug.LogError($"[NPC '{npcID}'] ❌ Cannot interact: MissionManager.Instance is null!");
                return false;
            }
            
            MissionData currentMission = MissionManager.Instance.GetCurrentMission();
            if (currentMission == null)
            {
                Debug.Log($"[NPC '{npcID}'] ❌ Cannot interact: No current mission active (required: '{requiredMissionID}')");
                return false;
            }
            
            if (currentMission.missionID != requiredMissionID)
            {
                Debug.Log($"[NPC '{npcID}'] ❌ Cannot interact: Wrong mission active (required: '{requiredMissionID}', current: '{currentMission.missionID}')");
                return false;
            }
            
            Debug.Log($"[NPC '{npcID}'] ✅ Mission check passed: '{requiredMissionID}' is active");
        }
        
        Debug.Log($"[NPC '{npcID}'] ✅ CanInteract = true");
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
            // We'll add this method to HUDController
            hud.ShowTalkPrompt(dialogueData.npcName);
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
    /// Trigger the dialogue UI
    /// </summary>
    private void TriggerDialogue()
    {
        Debug.Log($"[NPC '{npcID}'] 🎬 TriggerDialogue() called");
        
        if (dialogueData == null)
        {
            Debug.LogError($"[NPC '{npcID}'] ❌ No DialogueData assigned!");
            return;
        }
        
        Debug.Log($"[NPC '{npcID}'] DialogueData found: {dialogueData.npcName}");
        
        // Find and trigger DialogueUIController
        DialogueUIController dialogueUI = FindFirstObjectByType<DialogueUIController>();
        if (dialogueUI != null)
        {
            Debug.Log($"[NPC '{npcID}'] Found DialogueUIController, starting dialogue...");
            dialogueUI.StartDialogue(dialogueData, this);
            hasBeenTalkedTo = true;
            
            Debug.Log($"[NPC '{npcID}'] ✅ Started dialogue: {dialogueData.npcName}");
        }
        else
        {
            Debug.LogError($"[NPC '{npcID}'] ❌ DialogueUIController not found in scene!");
        }
    }
    
    /// <summary>
    /// Called by DialogueUIController when dialogue completes
    /// </summary>
    public void OnDialogueComplete()
    {
        Debug.Log($"[NPC '{npcID}'] Dialogue completed");
        
        // If this dialogue advances a mission, notify MissionManager
        if (dialogueData.advancesMission && MissionManager.Instance != null)
        {
            if (!string.IsNullOrEmpty(dialogueData.linkedMissionID))
            {
                // For now, we just complete the mission
                // In a more complex system, you might have "objectives" within missions
                MissionManager.Instance.CompleteMission(dialogueData.linkedMissionID);
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
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 2.5f,
            $"NPC: {(dialogueData != null ? dialogueData.npcName : "NO DATA")}\n{stateText}"
        );
        #endif
    }
}