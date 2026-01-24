using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Tracks lap completion with checkpoint validation
/// SPRINT 4 ENHANCED: Personal best tracking, reward calculation, repeatable lap game loop
/// Supports both mission mode (first lap) and free roam mode (infinite laps)
/// </summary>
[RequireComponent(typeof(Collider))]
public class LapTimer : MonoBehaviour
{
    [Header("Lap Configuration")]
    [Tooltip("Mission ID to complete when lap is finished (first lap only)")]
    public string missionID = "first_lap";
    
    [Header("Checkpoint System")]
    [Tooltip("Checkpoints that must be crossed before finish line is valid")]
    private List<Checkpoint> checkpoints = new List<Checkpoint>();
    
    [Header("Timing")]
    [Tooltip("Minimum time before lap can be completed (prevents instant completion)")]
    public float minimumLapTime = 10f;
    
    [Header("Rewards")]
    [Tooltip("Base cash reward for completing lap (only in free roam mode after mission complete)")]
    public float baseCashReward = 500f;
    
    [Tooltip("Bonus cash if personal best is beaten")]
    public float personalBestBonus = 100f;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private Color gizmoColor = new Color(0f, 1f, 0f, 0.3f); // Green
    [SerializeField] private Color lockedColor = new Color(1f, 0f, 0f, 0.3f); // Red
    
    // Timing state
    private float lapStartTime = -1f;
    private bool lapInProgress = false;
    private int nextCheckpointNumber = 1;
    private bool allCheckpointsCrossed = false;
    
    // Personal best tracking
    private float personalBest = Mathf.Infinity;
    private float currentLapTime = 0f;
    private bool lastLapBeatPersonalBest = false;
    
    // PlayerPrefs key for persistent storage
    private const string BEST_LAP_TIME_KEY = "BestLapTime";
    
    // Public accessors
    public float CurrentLapTime => currentLapTime;
    public float PersonalBest => personalBest;
    public bool LastLapBeatPersonalBest => lastLapBeatPersonalBest;
    public bool LapInProgress => lapInProgress;
    
    void Start()
    {
        // Load personal best from PlayerPrefs
        personalBest = PlayerPrefs.GetFloat(BEST_LAP_TIME_KEY, Mathf.Infinity);
        Debug.Log($"[LapTimer] Loaded personal best: {(personalBest == Mathf.Infinity ? "None" : $"{personalBest:F2}s")}");
        
        // Ensure this is a trigger
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"LapTimer '{name}': Collider is not a trigger! Setting isTrigger = true.");
            col.isTrigger = true;
        }
        
        // Subscribe to mission events
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.OnMissionStarted.AddListener(OnMissionStarted);
        }
    }
    
    void Update()
    {
        // Update current lap time display
        if (lapInProgress)
        {
            currentLapTime = Time.time - lapStartTime;
        }
    }
    
    void OnDestroy()
    {
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.OnMissionStarted.RemoveListener(OnMissionStarted);
        }
    }
    
    /// <summary>
    /// Register a checkpoint (called by Checkpoint.Start())
    /// </summary>
    public void RegisterCheckpoint(Checkpoint checkpoint)
    {
        if (!checkpoints.Contains(checkpoint))
        {
            checkpoints.Add(checkpoint);
            checkpoints = checkpoints.OrderBy(c => c.checkpointNumber).ToList();
            Debug.Log($"[LapTimer] Registered Checkpoint {checkpoint.checkpointNumber}");
        }
    }
    
    /// <summary>
    /// Called when a mission starts
    /// </summary>
    private void OnMissionStarted(MissionData mission)
    {
        if (mission.missionID == missionID)
        {
            StartLapTimer();
        }
    }
    
    /// <summary>
    /// Start the lap timer and activate first checkpoint
    /// </summary>
    private void StartLapTimer()
    {
        lapStartTime = Time.time;
        lapInProgress = true;
        nextCheckpointNumber = 1;
        allCheckpointsCrossed = false;
        currentLapTime = 0f;
        
        // Reset all checkpoints
        foreach (var checkpoint in checkpoints)
        {
            checkpoint.Reset();
        }
        
        // Activate first checkpoint
        ActivateNextCheckpoint();
        
        Debug.Log($"[LapTimer] Lap started! Cross {checkpoints.Count} checkpoint(s) then finish line.");
    }
    
    /// <summary>
    /// Activate the next checkpoint in sequence
    /// </summary>
    private void ActivateNextCheckpoint()
    {
        var nextCheckpoint = checkpoints.FirstOrDefault(c => c.checkpointNumber == nextCheckpointNumber);
        if (nextCheckpoint != null)
        {
            nextCheckpoint.Activate();
            Debug.Log($"[LapTimer] Checkpoint {nextCheckpointNumber} is now active");
        }
        else
        {
            // All checkpoints crossed!
            allCheckpointsCrossed = true;
            Debug.Log($"[LapTimer] All checkpoints crossed! Finish line is now active.");
        }
    }
    
    /// <summary>
    /// Called when a checkpoint is crossed
    /// </summary>
    public void OnCheckpointCrossed(int checkpointNumber)
    {
        if (checkpointNumber != nextCheckpointNumber)
        {
            Debug.LogWarning($"[LapTimer] Wrong checkpoint! Expected {nextCheckpointNumber}, got {checkpointNumber}");
            return;
        }
        
        // Move to next checkpoint
        nextCheckpointNumber++;
        ActivateNextCheckpoint();
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if player is driving and has vehicle
        if (!IsPlayerVehicle(other) || !IsPlayerDriving())
            return;
        
        // SCENARIO 1: Lap in progress - check if can finish
        if (lapInProgress)
        {
            // Check if all checkpoints are crossed
            if (!allCheckpointsCrossed)
            {
                Debug.Log($"[LapTimer] Finish line locked! Must cross checkpoint {nextCheckpointNumber} first.");
                return;
            }
            
            // Check if minimum time has passed
            float lapTime = Time.time - lapStartTime;
            if (lapTime < minimumLapTime)
            {
                Debug.Log($"[LapTimer] Too fast! Need {minimumLapTime}s minimum, current: {lapTime:F1}s");
                return;
            }
            
            // Complete the lap!
            CompleteLap(lapTime);
        }
        // SCENARIO 2: No lap in progress - check if free roam mode (start new lap automatically)
        else
        {
            // Only auto-start laps in free roam mode (after first lap mission is complete)
            if (MissionManager.Instance != null && MissionManager.Instance.IsMissionCompleted(missionID))
            {
                Debug.Log("[LapTimer] 🏁 Free roam mode - Starting new lap automatically!");
                StartLapTimer();
            }
            else
            {
                Debug.Log("[LapTimer] Waiting for mission to start lap timer.");
            }
        }
    }
    
    /// <summary>
    /// Complete the lap - handles both mission completion and free roam laps
    /// </summary>
    private void CompleteLap(float lapTime)
    {
        lapInProgress = false;
        currentLapTime = lapTime;
        
        // Check if this is a new personal best
        lastLapBeatPersonalBest = lapTime < personalBest;
        
        if (lastLapBeatPersonalBest)
        {
            personalBest = lapTime;
            PlayerPrefs.SetFloat(BEST_LAP_TIME_KEY, personalBest);
            PlayerPrefs.Save();
            Debug.Log($"[LapTimer] 🏆 NEW PERSONAL BEST: {personalBest:F2}s!");
        }
        else
        {
            Debug.Log($"[LapTimer] Lap completed in {lapTime:F2}s (Best: {personalBest:F2}s)");
        }
        
        // Check if this is the first lap (mission still active)
        bool isMissionLap = false;
        if (MissionManager.Instance != null)
        {
            isMissionLap = MissionManager.Instance.IsMissionAvailable(missionID) && 
                          !MissionManager.Instance.IsMissionCompleted(missionID);
        }
        
        if (isMissionLap)
        {
            // FIRST LAP: Complete the mission (mission system awards cash)
            Debug.Log($"[LapTimer] Completing mission: {missionID}");
            MissionManager.Instance.CompleteMission(missionID);
        }
        else
        {
            // FREE ROAM LAP: Award cash directly for personal best improvements
            float cashEarned = baseCashReward;
            
            if (lastLapBeatPersonalBest)
            {
                cashEarned += personalBestBonus;
                Debug.Log($"[LapTimer] 🏆 Free roam lap - Personal best beaten! Awarding ${cashEarned}");
            }
            else
            {
                Debug.Log($"[LapTimer] Free roam lap - Awarding base ${cashEarned}");
            }
            
            // Award cash
            if (PlayerStateManager.Instance != null && PlayerStateManager.Instance.hudController != null)
            {
                PlayerStateManager.Instance.hudController.AddCash(cashEarned);
            }
        }
        
        // Show lap reward popup (works for both mission and free roam laps)
        LapRewardUIController rewardUI = FindFirstObjectByType<LapRewardUIController>();
        if (rewardUI != null)
        {
            float displayCash = isMissionLap ? baseCashReward : (lastLapBeatPersonalBest ? baseCashReward + personalBestBonus : baseCashReward);
            rewardUI.ShowReward(displayCash, lastLapBeatPersonalBest, lapTime);
        }
        else
        {
            Debug.LogWarning("[LapTimer] LapRewardUIController not found! Cash awarded but no UI popup.");
        }
    }
    
    /// <summary>
    /// Public method to manually restart lap timer (for free roam mode)
    /// </summary>
    public void RestartLapTimer()
    {
        if (MissionManager.Instance != null && MissionManager.Instance.IsMissionCompleted(missionID))
        {
            Debug.Log("[LapTimer] Restarting lap timer in free roam mode");
            StartLapTimer();
        }
        else
        {
            Debug.LogWarning("[LapTimer] Cannot restart - first lap mission not yet completed");
        }
    }
    
    /// <summary>
    /// Check if the collider belongs to the player's vehicle
    /// </summary>
    private bool IsPlayerVehicle(Collider other)
    {
        return other.GetComponentInParent<PolyStang.CarController>() != null;
    }
    
    /// <summary>
    /// Check if player is currently in driving state
    /// </summary>
    private bool IsPlayerDriving()
    {
        if (PlayerStateManager.Instance == null)
            return false;
        
        return PlayerStateManager.Instance.CurrentState == PlayerStateManager.PlayerState.Driving;
    }
    
    void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;
        
        Collider col = GetComponent<Collider>();
        if (col == null) return;
        
        // Change color based on whether finish line is active
        Color currentColor = (lapInProgress && !allCheckpointsCrossed) ? lockedColor : gizmoColor;
        
        // Draw finish line zone
        Gizmos.color = currentColor;
        
        if (col is BoxCollider box)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
        }
        else if (col is SphereCollider sphere)
        {
            Gizmos.DrawSphere(transform.position + sphere.center, sphere.radius);
        }
        
        #if UNITY_EDITOR
        // Draw label
        string status;
        if (!lapInProgress)
        {
            // Check if we're in free roam mode
            bool isFreeRoam = MissionManager.Instance != null && 
                             MissionManager.Instance.IsMissionCompleted(missionID);
            
            if (isFreeRoam)
                status = "FINISH LINE\n(Free Roam - Cross to Start Lap)";
            else
                status = "FINISH LINE\n(Waiting for mission start)";
        }
        else if (!allCheckpointsCrossed)
        {
            status = $"FINISH LINE LOCKED\nNeed checkpoint {nextCheckpointNumber}";
        }
        else
        {
            status = $"FINISH LINE ACTIVE\n{currentLapTime:F1}s";
        }
        
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 3f,
            status
        );
        #endif
    }
}