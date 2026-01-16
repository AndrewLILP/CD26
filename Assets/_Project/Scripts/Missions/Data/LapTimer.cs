using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Tracks lap completion with checkpoint validation
/// Attach to a trigger collider at the finish line
/// </summary>
[RequireComponent(typeof(Collider))]
public class LapTimer : MonoBehaviour
{
    [Header("Lap Configuration")]
    [Tooltip("Mission ID to complete when lap is finished")]
    public string missionID = "first_lap";
    
    [Header("Checkpoint System")]
    [Tooltip("Checkpoints that must be crossed before finish line is valid")]
    private List<Checkpoint> checkpoints = new List<Checkpoint>();
    
    [Header("Timing")]
    [Tooltip("Minimum time before lap can be completed (prevents instant completion)")]
    public float minimumLapTime = 10f;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private Color gizmoColor = new Color(0f, 1f, 0f, 0.3f); // Green
    [SerializeField] private Color lockedColor = new Color(1f, 0f, 0f, 0.3f); // Red
    
    private float lapStartTime = -1f;
    private bool lapInProgress = false;
    private int nextCheckpointNumber = 1;
    private bool allCheckpointsCrossed = false;
    
    void Start()
    {
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
        if (!lapInProgress)
            return;
        
        // Check if all checkpoints are crossed
        if (!allCheckpointsCrossed)
        {
            Debug.Log($"[LapTimer] Finish line locked! Must cross checkpoint {nextCheckpointNumber} first.");
            return;
        }
        
        // Check if it's the player's vehicle
        if (!IsPlayerVehicle(other))
            return;
        
        // Check if player is driving
        if (!IsPlayerDriving())
            return;
        
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
    
    /// <summary>
    /// Complete the lap
    /// </summary>
    private void CompleteLap(float lapTime)
    {
        lapInProgress = false;
        
        Debug.Log($"[LapTimer] Lap completed in {lapTime:F2} seconds!");
        
        // Complete the mission (this awards cash automatically)
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.CompleteMission(missionID);
        }
        
        // Show completion message
        ShowCompletionMessage(lapTime);
    }
    
    /// <summary>
    /// Show lap completion message
    /// </summary>
    private void ShowCompletionMessage(float lapTime)
    {
        Debug.Log($"★★★ MISSION COMPLETE ★★★\nFirst Lap: {lapTime:F2}s\n+$500 earned!");
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
            status = "FINISH LINE\n(Waiting for mission start)";
        else if (!allCheckpointsCrossed)
            status = $"FINISH LINE LOCKED\nNeed checkpoint {nextCheckpointNumber}";
        else
            status = $"FINISH LINE ACTIVE\n{(Time.time - lapStartTime):F1}s";
        
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 3f,
            status
        );
        #endif
    }
}