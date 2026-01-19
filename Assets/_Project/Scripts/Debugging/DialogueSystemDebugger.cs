using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// DEBUG: Diagnose dialogue system issues
/// Attach to any GameObject, press F6 to run checks
/// </summary>
public class DialogueSystemDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            RunDialogueSystemDiagnostics();
        }
    }
    
    [ContextMenu("Diagnose Dialogue System")]
    public void RunDialogueSystemDiagnostics()
    {
        Debug.Log("=== DIALOGUE SYSTEM DIAGNOSTICS ===");
        
        // Check 1: NPCInteraction script exists
        NPCInteraction[] npcs = FindObjectsByType<NPCInteraction>(FindObjectsSortMode.None);
        Debug.Log($"\n--- NPC INTERACTION SCRIPTS ---");
        Debug.Log($"Found {npcs.Length} NPC(s) with NPCInteraction component");
        
        if (npcs.Length == 0)
        {
            Debug.LogError("❌ NO NPCs found with NPCInteraction component!");
            Debug.LogError("   → Add NPCInteraction component to Maria_NPC and UncleRay_NPC");
        }
        else
        {
            foreach (var npc in npcs)
            {
                Debug.Log($"\nNPC: {npc.gameObject.name}");
                Debug.Log($"  - NPC ID: {npc.npcID}");
                Debug.Log($"  - Dialogue Data: {(npc.dialogueData != null ? "✅ Assigned" : "❌ NULL")}");
                Debug.Log($"  - Walking Only: {npc.walkingOnly}");
                Debug.Log($"  - Repeatable: {npc.repeatableDialogue}");
                Debug.Log($"  - Required Mission: {npc.requiredMissionID}");
                // Note: hasBeenTalkedTo is private, can't access for debugging
                
                // Check collider
                Collider col = npc.GetComponent<Collider>();
                if (col == null)
                {
                    Debug.LogError($"  ❌ No Collider on {npc.name}!");
                }
                else if (!col.isTrigger)
                {
                    Debug.LogError($"  ❌ Collider is not a trigger on {npc.name}!");
                }
                else
                {
                    Debug.Log($"  ✅ Trigger collider set up correctly");
                }
            }
        }
        
        // Check 2: DialogueUIController exists
        Debug.Log($"\n--- DIALOGUE UI CONTROLLER ---");
        DialogueUIController dialogueUI = FindFirstObjectByType<DialogueUIController>();
        
        if (dialogueUI == null)
        {
            Debug.LogError("❌ DialogueUIController NOT FOUND!");
            Debug.LogError("   → Attach DialogueUIController to your HUD GameObject");
        }
        else
        {
            Debug.Log("✅ DialogueUIController found");
            
            // Check if dialogue panel is in UI
            var uiDoc = dialogueUI.GetComponent<UnityEngine.UIElements.UIDocument>();
            if (uiDoc != null)
            {
                var root = uiDoc.rootVisualElement;
                var panel = root.Q<VisualElement>("dialogue-panel");
                if (panel == null)
                {
                    Debug.LogError("❌ 'dialogue-panel' element NOT FOUND in UXML!");
                    Debug.LogError("   → Update GameHUD.uxml with dialogue panel elements");
                }
                else
                {
                    Debug.Log("✅ Dialogue panel found in UXML");
                }
            }
        }
        
        // Check 3: Dialogue Data assets
        Debug.Log($"\n--- DIALOGUE DATA ASSETS ---");
        
        // Try to find dialogue data in project (this won't find them all, but gives a hint)
        DialogueData[] allDialogues = Resources.FindObjectsOfTypeAll<DialogueData>();
        Debug.Log($"Found {allDialogues.Length} DialogueData asset(s) in project");
        
        if (allDialogues.Length == 0)
        {
            Debug.LogError("❌ NO DialogueData assets found!");
            Debug.LogError("   → Create DialogueData ScriptableObjects for Maria and Uncle Ray");
        }
        else
        {
            foreach (var dialogue in allDialogues)
            {
                Debug.Log($"Dialogue: {dialogue.npcName} - {dialogue.lessonTopic}");
                Debug.Log($"  - Lines: {dialogue.dialogueLines.Length}");
                Debug.Log($"  - Linked Mission: {dialogue.linkedMissionID}");
                Debug.Log($"  - Advances Mission: {dialogue.advancesMission}");
            }
        }
        
        // Check 4: Current mission state
        Debug.Log($"\n--- CURRENT MISSION STATE ---");
        if (MissionManager.Instance != null)
        {
            MissionData currentMission = MissionManager.Instance.GetCurrentMission();
            if (currentMission != null)
            {
                Debug.Log($"Current Mission: {currentMission.missionName}");
                Debug.Log($"Mission ID: {currentMission.missionID}");
            }
            else
            {
                Debug.Log("No mission currently active");
            }
            
            bool coffeeRunAvailable = MissionManager.Instance.IsMissionAvailable("coffee_run");
            bool coffeeRunCompleted = MissionManager.Instance.IsMissionCompleted("coffee_run");
            Debug.Log($"Coffee Run available: {coffeeRunAvailable}");
            Debug.Log($"Coffee Run completed: {coffeeRunCompleted}");
        }
        
        Debug.Log("\n=== DIAGNOSTICS COMPLETE ===");
    }
}
