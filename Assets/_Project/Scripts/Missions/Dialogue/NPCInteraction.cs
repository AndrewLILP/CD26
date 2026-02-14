using UnityEngine;

/// <summary>
/// Handles NPC interaction - checks active mission and triggers appropriate dialogue
/// Attached to NPC GameObjects with trigger colliders
/// UPDATED: Added cooldown to prevent E key conflict with vehicle exit
/// </summary>
[RequireComponent(typeof(Collider))]
public class NPCInteraction : MonoBehaviour
{
    [Header("NPC Configuration")]
    [Tooltip("Dialogue data containing all mission-specific dialogues for this NPC")]
    public DialogueData npcDialogue;
    
    [Header("Trigger Settings")]
    [Tooltip("Tag of player character (usually 'Player')")]
    public string playerTag = "Player";
    
    [Tooltip("Show debug logs")]
    public bool debugMode = true;
    
    private bool playerInRange = false;
    private DialogueUIController dialogueUI;
    
    // Cooldown to prevent E key conflict with vehicle exit
    private float interactionCooldown = 0f;
    private const float COOLDOWN_DURATION = 0.3f;
    
    void Start()
    {
        // Validate configuration
        if (npcDialogue == null)
        {
            Debug.LogError($"[NPCInteraction] No DialogueData assigned to {gameObject.name}!");
            return;
        }
        
        // Find DialogueUIController in scene
        dialogueUI = FindFirstObjectByType<DialogueUIController>();
        if (dialogueUI == null)
        {
            Debug.LogError("[NPCInteraction] DialogueUIController not found in scene!");
        }
        
        // Ensure collider is set to trigger
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"[NPCInteraction] Collider on {gameObject.name} is not set to trigger! Setting now...");
            col.isTrigger = true;
        }
        
        if (debugMode)
        {
            Debug.Log($"[NPCInteraction] {npcDialogue.npcName} initialized");
        }
    }
    
    void Update()
    {
        // Update cooldown timer
        if (interactionCooldown > 0f)
        {
            interactionCooldown -= Time.deltaTime;
        }
        
        // If player is in range and presses E (with cooldown check)
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && interactionCooldown <= 0f)
        {
            TryStartDialogue();
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = true;
            
            // Set cooldown when entering NPC range to prevent immediate E key trigger
            // This prevents dialogue from starting when exiting vehicle with E key
            interactionCooldown = COOLDOWN_DURATION;
            
            // Small delay before checking dialogue (let cooldown start)
            Invoke(nameof(CheckAndShowPrompt), COOLDOWN_DURATION);
            
            if (debugMode)
            {
                Debug.Log($"[NPCInteraction] Player entered {npcDialogue.npcName}'s range");
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInRange = false;
            CancelInvoke(nameof(CheckAndShowPrompt));
            HideInteractionPrompt();
            
            if (debugMode)
            {
                Debug.Log($"[NPCInteraction] Player left {npcDialogue.npcName}'s range");
            }
        }
    }
    
    /// <summary>
    /// Check dialogue and show prompt (called after cooldown)
    /// </summary>
    private void CheckAndShowPrompt()
    {
        if (playerInRange && CanTriggerDialogue())
        {
            ShowInteractionPrompt();
        }
    }
    
    /// <summary>
    /// Check if dialogue can be triggered (mission active + dialogue exists)
    /// </summary>
    private bool CanTriggerDialogue()
    {
        // Check if MissionManager exists
        if (MissionManager.Instance == null)
        {
            if (debugMode) Debug.LogWarning("[NPCInteraction] MissionManager not found!");
            return false;
        }
        
        // Get current active mission
        MissionData currentMission = MissionManager.Instance.GetCurrentMission();
        
        if (currentMission == null)
        {
            if (debugMode) Debug.Log($"[NPCInteraction] No active mission - {npcDialogue.npcName} remains silent");
            return false;
        }
        
        // Check if this NPC has dialogue for the current mission
        bool hasDialogue = npcDialogue.HasDialogueForMission(currentMission.missionID);
        
        if (debugMode)
        {
            if (hasDialogue)
            {
                Debug.Log($"[NPCInteraction] {npcDialogue.npcName} has dialogue for mission '{currentMission.missionID}'");
            }
            else
            {
                Debug.Log($"[NPCInteraction] {npcDialogue.npcName} has no dialogue for mission '{currentMission.missionID}'");
            }
        }
        
        return hasDialogue;
    }
    
    /// <summary>
    /// Attempt to start dialogue with NPC
    /// </summary>
    private void TryStartDialogue()
    {
        if (npcDialogue == null || dialogueUI == null)
        {
            Debug.LogError("[NPCInteraction] Missing references!");
            return;
        }
        
        // Get current mission
        if (MissionManager.Instance == null) return;
        
        MissionData currentMission = MissionManager.Instance.GetCurrentMission();
        if (currentMission == null)
        {
            if (debugMode) Debug.Log("[NPCInteraction] No active mission - cannot trigger dialogue");
            return;
        }
        
        // Get dialogue for current mission
        DialogueEntry[] dialogueEntries = npcDialogue.GetDialogueForMission(currentMission.missionID);
        
        if (dialogueEntries == null || dialogueEntries.Length == 0)
        {
            if (debugMode) Debug.Log($"[NPCInteraction] No dialogue for mission '{currentMission.missionID}'");
            return;
        }
        
        // Start dialogue
        HideInteractionPrompt();
        dialogueUI.StartDialogue(npcDialogue.npcName, npcDialogue.npcTitle, dialogueEntries);
        
        if (debugMode)
        {
            Debug.Log($"[NPCInteraction] Started dialogue with {npcDialogue.npcName} for mission '{currentMission.missionID}'");
        }
    }
    
    /// <summary>
    /// Show "Press E to Talk" prompt in HUD
    /// </summary>
    private void ShowInteractionPrompt()
    {
        if (PlayerStateManager.Instance != null && PlayerStateManager.Instance.hudController != null)
        {
            PlayerStateManager.Instance.hudController.ShowTalkPrompt(npcDialogue.npcName);
        }
    }
    
    /// <summary>
    /// Hide interaction prompt from HUD
    /// </summary>
    private void HideInteractionPrompt()
    {
        if (PlayerStateManager.Instance != null && PlayerStateManager.Instance.hudController != null)
        {
            PlayerStateManager.Instance.hudController.HideInteractionPrompt();
        }
    }
}
