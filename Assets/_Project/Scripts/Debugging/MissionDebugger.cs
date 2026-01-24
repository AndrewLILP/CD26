using UnityEngine;

/// <summary>
/// DEBUG SCRIPT: Helps diagnose Mission 2 unlock issues
/// Attach to any GameObject in scene or run from MissionManager context menu
/// </summary>
public class MissionDebugger : MonoBehaviour
{
    void Update()
    {
        // Press F3 to run full diagnostics
        if (Input.GetKeyDown(KeyCode.F3))
        {
            RunFullDiagnostics();
        }
        
        // Press F4 to manually unlock Coffee Run
        if (Input.GetKeyDown(KeyCode.F4))
        {
            ManuallyUnlockCoffeeRun();
        }
        
        // Press F5 to complete First Lap
        if (Input.GetKeyDown(KeyCode.F5))
        {
            CompleteFirstLap();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            DebugMissionState();
        }
    }

    void DebugMissionState()
    {
        if (MissionManager.Instance == null)
        {
            Debug.LogError("MissionManager.Instance is NULL!");
            return;
        }
        
        Debug.Log("=== MISSION DEBUG ===");
        Debug.Log($"Current Mission: '{MissionManager.Instance.GetCurrentMission()?.missionID ?? "NONE"}'");
        Debug.Log($"Missions in Database: {MissionManager.Instance.allMissions.Count}");
        
        foreach (var mission in MissionManager.Instance.allMissions)
        {
            bool completed = MissionManager.Instance.IsMissionCompleted(mission.missionID);
            bool available = MissionManager.Instance.IsMissionAvailable(mission.missionID);
            Debug.Log($"  - {mission.missionID}: Completed={completed}, Available={available}");
        }
    }
    
    [ContextMenu("Run Full Diagnostics")]
    public void RunFullDiagnostics()
    {
        Debug.Log("=== MISSION SYSTEM DIAGNOSTICS ===");
        
        if (MissionManager.Instance == null)
        {
            Debug.LogError("❌ MissionManager.Instance is NULL!");
            return;
        }
        
        Debug.Log("✅ MissionManager found");
        
        // Check all missions
        Debug.Log($"\n--- ALL MISSIONS ({MissionManager.Instance.allMissions.Count}) ---");
        foreach (var mission in MissionManager.Instance.allMissions)
        {
            if (mission == null)
            {
                Debug.LogError("❌ NULL mission in allMissions list!");
                continue;
            }
            
            Debug.Log($"Mission: {mission.missionName} (ID: {mission.missionID})");
            Debug.Log($"  - Available at start: {mission.availableAtStart}");
            Debug.Log($"  - Required missions: [{string.Join(", ", mission.requiredMissions)}]");
            Debug.Log($"  - Type: {mission.type}");
            Debug.Log($"  - Reward: ${mission.cashReward}");
        }
        
        // Check First Lap status
        Debug.Log("\n--- FIRST LAP STATUS ---");
        bool firstLapCompleted = MissionManager.Instance.IsMissionCompleted("first_lap");
        bool firstLapAvailable = MissionManager.Instance.IsMissionAvailable("first_lap");
        Debug.Log($"First Lap completed: {firstLapCompleted}");
        Debug.Log($"First Lap available: {firstLapAvailable}");
        
        // Check Coffee Run status
        Debug.Log("\n--- COFFEE RUN STATUS ---");
        MissionData coffeeRun = MissionManager.Instance.GetMissionByID("coffee_run");
        if (coffeeRun == null)
        {
            Debug.LogError("❌ Coffee Run mission NOT FOUND in MissionManager!");
            Debug.LogError("   → Did you add Mission_CoffeeRun to MissionManager's 'All Missions' list?");
        }
        else
        {
            Debug.Log("✅ Coffee Run mission found");
            bool coffeeRunCompleted = MissionManager.Instance.IsMissionCompleted("coffee_run");
            bool coffeeRunAvailable = MissionManager.Instance.IsMissionAvailable("coffee_run");
            Debug.Log($"Coffee Run completed: {coffeeRunCompleted}");
            Debug.Log($"Coffee Run available: {coffeeRunAvailable}");
            
            if (!coffeeRunAvailable && !coffeeRunCompleted)
            {
                Debug.LogWarning("⚠️ Coffee Run is NOT available!");
                if (!firstLapCompleted)
                {
                    Debug.LogWarning("   → Reason: First Lap not completed yet");
                }
                else
                {
                    Debug.LogError("   → Reason: UNKNOWN! First Lap is complete but Coffee Run didn't unlock!");
                    Debug.LogError("   → Check Coffee Run's 'requiredMissions' array - should contain 'first_lap'");
                }
            }
        }
        
        // Check MissionTriggerZone
        Debug.Log("\n--- MISSION TRIGGER ZONES ---");
        MissionTriggerZone[] triggers = FindObjectsByType<MissionTriggerZone>(FindObjectsSortMode.None);
        Debug.Log($"Found {triggers.Length} mission trigger zone(s)");
        foreach (var trigger in triggers)
        {
            Debug.Log($"Trigger: {trigger.name}");
            Debug.Log($"  - Mission ID: {trigger.missionID}");
            Debug.Log($"  - Require Driving: {trigger.requireDriving}");
            Debug.Log($"  - Trigger Once: {trigger.triggerOnce}");
        }
        
        Debug.Log("=== DIAGNOSTICS COMPLETE ===\n");
    }
    
    [ContextMenu("Manually Unlock Coffee Run")]
    public void ManuallyUnlockCoffeeRun()
    {
        if (MissionManager.Instance == null)
        {
            Debug.LogError("MissionManager not found!");
            return;
        }
        
        // This is a hack to force unlock Coffee Run by using reflection
        var completedMissions = typeof(MissionManager).GetField("completedMissions", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (completedMissions != null)
        {
            var list = completedMissions.GetValue(MissionManager.Instance) as System.Collections.Generic.List<string>;
            if (!list.Contains("first_lap"))
            {
                list.Add("first_lap");
                Debug.Log("✅ Manually marked First Lap as completed");
            }
        }
        
        // Trigger unlock check
        typeof(MissionManager).GetMethod("CheckMissionUnlocks", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(MissionManager.Instance, null);
        
        Debug.Log("🔓 Attempted to unlock Coffee Run. Check console for 'Unlocked mission' message.");
    }
    
    [ContextMenu("Complete First Lap (Debug)")]
    public void CompleteFirstLap()
    {
        if (MissionManager.Instance == null)
        {
            Debug.LogError("MissionManager not found!");
            return;
        }
        
        if (MissionManager.Instance.IsMissionCompleted("first_lap"))
        {
            Debug.Log("First Lap already completed!");
            return;
        }
        
        MissionManager.Instance.CompleteMission("first_lap");
        Debug.Log("✅ Completed First Lap mission");
    }
}
