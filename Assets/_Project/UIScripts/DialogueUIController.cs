using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

/// <summary>
/// Controls the dialogue UI panel for NPC conversations
/// UPDATED: Works with mission-aware DialogueData architecture
/// Receives DialogueEntry[] arrays instead of full DialogueData
/// </summary>
public class DialogueUIController : MonoBehaviour
{
    private UIDocument uiDocument;
    
    // UI Elements
    private VisualElement dialoguePanel;
    private Label npcNameLabel;
    private Label npcTitleLabel;
    private Label dialogueTextLabel;
    private VisualElement continuePrompt;
    
    // State
    private DialogueEntry[] currentDialogueEntries;
    private string currentNPCName;
    private int currentLineIndex = 0;
    private bool isTyping = false;
    private bool dialogueActive = false;
    
    [Header("Typewriter Settings")]
    [Tooltip("Characters per second for typewriter effect")]
    [SerializeField] private float typewriterSpeed = 30f;
    
    [Tooltip("Skip typewriter effect with Space key")]
    [SerializeField] private bool canSkipTypewriter = true;
    
    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        
        if (uiDocument == null)
        {
            Debug.LogError("DialogueUIController: No UIDocument found!");
            return;
        }
        
        var root = uiDocument.rootVisualElement;
        
        // Cache references
        dialoguePanel = root.Q<VisualElement>("dialogue-panel");
        npcNameLabel = root.Q<Label>("npc-name");
        npcTitleLabel = root.Q<Label>("npc-title");
        dialogueTextLabel = root.Q<Label>("dialogue-text");
        continuePrompt = root.Q<VisualElement>("continue-prompt");
        
        // Validate
        if (dialoguePanel == null) Debug.LogError("DialogueUIController: 'dialogue-panel' not found!");
        if (npcNameLabel == null) Debug.LogError("DialogueUIController: 'npc-name' not found!");
        if (npcTitleLabel == null) Debug.LogError("DialogueUIController: 'npc-title' not found!");
        if (dialogueTextLabel == null) Debug.LogError("DialogueUIController: 'dialogue-text' not found!");
        if (continuePrompt == null) Debug.LogError("DialogueUIController: 'continue-prompt' not found!");
        
        // Start hidden
        HideDialogue();
        
        Debug.Log("[DialogueUI] Initialized");
    }
    
    void Update()
    {
        if (!dialogueActive) return;
        
        // Skip typewriter effect with Space
        if (canSkipTypewriter && isTyping && Input.GetKeyDown(KeyCode.Space))
        {
            SkipTypewriter();
        }
        
        // Advance to next line with E or Space (when not typing)
        if (!isTyping && (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space)))
        {
            AdvanceDialogue();
        }
    }
    
    /// <summary>
    /// Start a new dialogue sequence
    /// UPDATED: Now accepts DialogueEntry[] from mission-aware DialogueData
    /// </summary>
    public void StartDialogue(string npcName, string npcTitle, DialogueEntry[] dialogueEntries)
    {
        if (dialogueEntries == null || dialogueEntries.Length == 0)
        {
            Debug.LogError("DialogueUIController: Invalid dialogue entries!");
            return;
        }
        
        currentDialogueEntries = dialogueEntries;
        currentNPCName = npcName;
        currentLineIndex = 0;
        dialogueActive = true;
        
        // Populate NPC info
        if (npcNameLabel != null)
            npcNameLabel.text = npcName.ToUpper();
        
        if (npcTitleLabel != null)
            npcTitleLabel.text = npcTitle;
        
        // Show panel
        if (dialoguePanel != null)
            dialoguePanel.RemoveFromClassList("hidden");
        
        // Pause game
        Time.timeScale = 0f;
        
        // Display first line
        DisplayCurrentLine();
        
        Debug.Log($"[DialogueUI] Started dialogue with {npcName} ({dialogueEntries.Length} entries)");
    }
    
    /// <summary>
    /// Display the current dialogue line with typewriter effect
    /// </summary>
    private void DisplayCurrentLine()
    {
        if (currentDialogueEntries == null || currentLineIndex >= currentDialogueEntries.Length)
        {
            EndDialogue();
            return;
        }
        
        var entry = currentDialogueEntries[currentLineIndex];
        
        // Hide continue prompt while typing
        if (continuePrompt != null)
            continuePrompt.AddToClassList("hidden");
        
        // Start typewriter effect
        StartCoroutine(TypewriterEffect(entry.dialogueText));
        
        // Future: Handle entry.eventTrigger if needed
        if (!string.IsNullOrEmpty(entry.eventTrigger))
        {
            Debug.Log($"[DialogueUI] Event trigger: {entry.eventTrigger}");
            // TODO: Implement event system if needed
        }
    }
    
    /// <summary>
    /// Typewriter effect for dialogue text
    /// </summary>
    private IEnumerator TypewriterEffect(string fullText)
    {
        isTyping = true;
        
        if (dialogueTextLabel == null)
        {
            isTyping = false;
            yield break;
        }
        
        dialogueTextLabel.text = "";
        
        foreach (char c in fullText)
        {
            dialogueTextLabel.text += c;
            
            // Wait for next character (scaled by unscaled time since game is paused)
            yield return new WaitForSecondsRealtime(1f / typewriterSpeed);
        }
        
        isTyping = false;
        
        // Show continue prompt
        if (continuePrompt != null)
            continuePrompt.RemoveFromClassList("hidden");
    }
    
    /// <summary>
    /// Skip typewriter effect and show full text immediately
    /// </summary>
    private void SkipTypewriter()
    {
        if (!isTyping || currentDialogueEntries == null) return;
        
        StopAllCoroutines();
        isTyping = false;
        
        // Show full text
        if (dialogueTextLabel != null && currentLineIndex < currentDialogueEntries.Length)
        {
            dialogueTextLabel.text = currentDialogueEntries[currentLineIndex].dialogueText;
        }
        
        // Show continue prompt
        if (continuePrompt != null)
            continuePrompt.RemoveFromClassList("hidden");
    }
    
    /// <summary>
    /// Advance to next dialogue line
    /// </summary>
    private void AdvanceDialogue()
    {
        currentLineIndex++;
        
        if (currentLineIndex >= currentDialogueEntries.Length)
        {
            EndDialogue();
        }
        else
        {
            DisplayCurrentLine();
        }
    }
    
    /// <summary>
    /// End the current dialogue
    /// </summary>
    private void EndDialogue()
    {
        dialogueActive = false;
        
        // Hide panel
        HideDialogue();
        
        // Resume game
        Time.timeScale = 1f;
        
        Debug.Log($"[DialogueUI] Ended dialogue with {currentNPCName}");
        
        // Clear references
        currentDialogueEntries = null;
        currentNPCName = null;
        currentLineIndex = 0;
    }
    
    /// <summary>
    /// Hide the dialogue panel
    /// </summary>
    private void HideDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.AddToClassList("hidden");
    }
    
    /// <summary>
    /// Check if dialogue is currently active
    /// </summary>
    public bool IsDialogueActive()
    {
        return dialogueActive;
    }
}
