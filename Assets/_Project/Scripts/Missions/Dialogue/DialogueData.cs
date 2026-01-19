using UnityEngine;

/// <summary>
/// ScriptableObject that stores NPC dialogue data
/// Create via: Assets > Create > CD26 > Dialogue Data
/// </summary>
[CreateAssetMenu(fileName = "NewDialogue", menuName = "CD26/Dialogue Data", order = 2)]
public class DialogueData : ScriptableObject
{
    [Header("NPC Info")]
    [Tooltip("NPC's display name (e.g., 'Maria', 'Uncle Ray')")]
    public string npcName = "NPC";
    
    [Tooltip("NPC's role/title (e.g., 'Cafe Owner', 'Mentor')")]
    public string npcTitle = "Unknown";
    
    [Header("Dialogue Content")]
    [Tooltip("Lines of dialogue in order")]
    public DialogueLine[] dialogueLines;
    
    [Header("Financial Education")]
    [Tooltip("What financial concept does this teach? (e.g., 'Cash Flow', 'Assets vs Liabilities')")]
    public string lessonTopic = "";
    
    [TextArea(2, 4)]
    [Tooltip("Key takeaway message from this conversation")]
    public string lessonSummary = "";
    
    [Header("Mission Integration")]
    [Tooltip("Mission ID this dialogue is part of (leave empty if standalone)")]
    public string linkedMissionID = "";
    
    [Tooltip("Does completing this dialogue advance/complete the mission?")]
    public bool advancesMission = false;
    
    [System.Serializable]
    public class DialogueLine
    {
        [TextArea(2, 4)]
        public string text;
        
        [Tooltip("Optional: Delay before showing next line (seconds)")]
        public float delayAfter = 0f;
        
        [Tooltip("Optional: Play a sound effect when this line appears")]
        public AudioClip voiceClip;
    }
}
