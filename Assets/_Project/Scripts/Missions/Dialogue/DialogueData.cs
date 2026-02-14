using UnityEngine;

/// <summary>
/// Stores all dialogue for a single NPC across multiple missions
/// Each NPC has ONE DialogueData containing all their mission-specific conversations
/// ARCHITECTURE: Mission-aware dialogue system for CD26
/// </summary>
[CreateAssetMenu(fileName = "Dialogue_NPCName", menuName = "CD26/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Header("NPC Information")]
    [Tooltip("Name of the NPC (e.g., 'Maria', 'Uncle Ray')")]
    public string npcName = "NPC";
    
    [Tooltip("Title/role (e.g., 'Cafe Owner', 'Mechanic')")]
    public string npcTitle = "Role";
    
    [Header("Mission-Specific Dialogues")]
    [Tooltip("All dialogue sequences for this NPC, mapped by mission")]
    public MissionDialogue[] missionDialogues;
    
    /// <summary>
    /// Find dialogue entries for a specific mission
    /// Returns null if no dialogue exists for this mission
    /// </summary>
    public DialogueEntry[] GetDialogueForMission(string missionID)
    {
        foreach (var missionDialogue in missionDialogues)
        {
            if (missionDialogue.missionID == missionID)
            {
                return missionDialogue.dialogueEntries;
            }
        }
        
        // No dialogue for this mission
        return null;
    }
    
    /// <summary>
    /// Check if this NPC has dialogue for a specific mission
    /// </summary>
    public bool HasDialogueForMission(string missionID)
    {
        return GetDialogueForMission(missionID) != null;
    }
}

/// <summary>
/// Maps a mission ID to dialogue entries
/// </summary>
[System.Serializable]
public class MissionDialogue
{
    [Tooltip("Mission ID this dialogue is for (e.g., 'first_lap', 'coffee_run')")]
    public string missionID;
    
    [Tooltip("Dialogue entries for this mission")]
    public DialogueEntry[] dialogueEntries;
}

/// <summary>
/// Individual dialogue entry (one "page" of dialogue)
/// </summary>
[System.Serializable]
public class DialogueEntry
{
    [TextArea(3, 6)]
    public string dialogueText;
    
    [Tooltip("Optional: Trigger event when this dialogue entry is displayed")]
    public string eventTrigger = "";
}
