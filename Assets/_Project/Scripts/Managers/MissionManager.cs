using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Singleton manager for mission system
/// Tracks mission state, unlocks, and completion
/// </summary>
public class MissionManager : MonoBehaviour
{
    #region Singleton
    public static MissionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        Debug.Log("[MissionManager] Initialized");
    }
    #endregion

    [Header("Mission Database")]
    [Tooltip("All missions in the game (drag MissionData ScriptableObjects here)")]
    public List<MissionData> allMissions = new List<MissionData>();
    
    [Header("Current State")]
    [SerializeField] private List<string> completedMissions = new List<string>();
    [SerializeField] private List<string> activeMissions = new List<string>();
    [SerializeField] private string currentMissionID = "";
    
    #region Events
    public UnityEvent<MissionData> OnMissionStarted = new UnityEvent<MissionData>();
    public UnityEvent<MissionData> OnMissionCompleted = new UnityEvent<MissionData>();
    public UnityEvent<MissionData> OnMissionUnlocked = new UnityEvent<MissionData>();
    #endregion
    
    void Start()
    {
        InitializeMissions();
    }

    /*void Update()
    {
        // DEBUG: Press M to start first mission
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (activeMissions.Count > 0)
            {
                StartMission(activeMissions[0]);
            }
        }
        
        // DEBUG: Press N to complete current mission
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (!string.IsNullOrEmpty(currentMissionID))
            {
                CompleteMission(currentMissionID);
            }
        }
    }
    */
    
    /// <summary>
    /// Initialize missions at game start
    /// </summary>
    private void InitializeMissions()
    {
        // Unlock all missions that are available at start
        foreach (var mission in allMissions)
        {
            if (mission.availableAtStart && !activeMissions.Contains(mission.missionID))
            {
                UnlockMission(mission.missionID);
            }
        }
        
        Debug.Log($"[MissionManager] {activeMissions.Count} missions available at start");
    }
    
    /// <summary>
    /// Start a specific mission
    /// </summary>
    public void StartMission(string missionID)
    {
        MissionData mission = GetMissionByID(missionID);
        if (mission == null)
        {
            Debug.LogError($"[MissionManager] Mission '{missionID}' not found!");
            return;
        }
        
        if (!activeMissions.Contains(missionID))
        {
            Debug.LogWarning($"[MissionManager] Mission '{missionID}' is not unlocked!");
            return;
        }
        
        if (completedMissions.Contains(missionID))
        {
            Debug.LogWarning($"[MissionManager] Mission '{missionID}' already completed!");
            return;
        }
        
        currentMissionID = missionID;
        OnMissionStarted?.Invoke(mission);
        
        Debug.Log($"[MissionManager] Started mission: {mission.missionName}");
    }
    
    /// <summary>
    /// Complete the current mission
    /// </summary>
    public void CompleteMission(string missionID)
    {
        MissionData mission = GetMissionByID(missionID);
        if (mission == null)
        {
            Debug.LogError($"[MissionManager] Mission '{missionID}' not found!");
            return;
        }
        
        if (completedMissions.Contains(missionID))
        {
            Debug.LogWarning($"[MissionManager] Mission '{missionID}' already completed!");
            return;
        }
        
        // Mark as completed
        completedMissions.Add(missionID);
        activeMissions.Remove(missionID);
        
        // Award cash
        if (PlayerStateManager.Instance != null && PlayerStateManager.Instance.hudController != null)
        {
            PlayerStateManager.Instance.hudController.AddCash(mission.cashReward);
        }
        
        // Fire event
        OnMissionCompleted?.Invoke(mission);
        
        // Check if this unlocks new missions
        CheckMissionUnlocks();
        
        Debug.Log($"[MissionManager] Completed mission: {mission.missionName} (+${mission.cashReward})");
        
        // Clear current mission
        if (currentMissionID == missionID)
        {
            currentMissionID = "";
        }
    }
    
    /// <summary>
    /// Unlock a mission (make it available to start)
    /// </summary>
    private void UnlockMission(string missionID)
    {
        if (activeMissions.Contains(missionID) || completedMissions.Contains(missionID))
            return;
        
        activeMissions.Add(missionID);
        
        MissionData mission = GetMissionByID(missionID);
        if (mission != null)
        {
            OnMissionUnlocked?.Invoke(mission);
            Debug.Log($"[MissionManager] Unlocked mission: {mission.missionName}");
        }

    }
    
    /// <summary>
    /// Check if completing missions unlocks new ones
    /// </summary>
    private void CheckMissionUnlocks()
    {
        foreach (var mission in allMissions)
        {
            // Skip if already unlocked/completed
            if (activeMissions.Contains(mission.missionID) || completedMissions.Contains(mission.missionID))
                continue;
            
            // Check if prerequisites are met
            bool prerequisitesMet = true;
            foreach (var requiredMissionID in mission.requiredMissions)
            {
                if (!completedMissions.Contains(requiredMissionID))
                {
                    prerequisitesMet = false;
                    break;
                }
            }
            
            if (prerequisitesMet)
            {
                UnlockMission(mission.missionID);
            }
        }
    }
    
    /// <summary>
    /// Get mission data by ID
    /// </summary>
    public MissionData GetMissionByID(string missionID)
    {
        return allMissions.FirstOrDefault(m => m.missionID == missionID);
    }
    
    /// <summary>
    /// Get current active mission
    /// </summary>
    public MissionData GetCurrentMission()
    {
        if (string.IsNullOrEmpty(currentMissionID))
            return null;
        
        return GetMissionByID(currentMissionID);
    }
    
    /// <summary>
    /// Check if a mission is completed
    /// </summary>
    public bool IsMissionCompleted(string missionID)
    {
        return completedMissions.Contains(missionID);
    }
    
    /// <summary>
    /// Check if a mission is available
    /// </summary>
    public bool IsMissionAvailable(string missionID)
    {
        return activeMissions.Contains(missionID);
    }
    
    #region Debug Helpers
    [ContextMenu("Debug: List All Missions")]
    private void DebugListMissions()
    {
        Debug.Log($"=== MISSION STATUS ===");
        Debug.Log($"Active: {string.Join(", ", activeMissions)}");
        Debug.Log($"Completed: {string.Join(", ", completedMissions)}");
        Debug.Log($"Current: {currentMissionID}");
    }
    
    [ContextMenu("Debug: Reset All Missions")]
    private void DebugResetMissions()
    {
        completedMissions.Clear();
        activeMissions.Clear();
        currentMissionID = "";
        InitializeMissions();
        Debug.Log("[MissionManager] All missions reset!");
    }
    #endregion
}