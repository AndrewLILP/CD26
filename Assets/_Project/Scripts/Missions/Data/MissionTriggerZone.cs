using UnityEngine;

/// <summary>
/// Trigger zone that shows mission briefing when player enters (while driving)
/// Attach to a trigger collider near the start line
/// </summary>
[RequireComponent(typeof(Collider))]
public class MissionTriggerZone : MonoBehaviour
{
    [Header("Mission Configuration")]
    [Tooltip("The mission to trigger when entering this zone")]
    public string missionID = "first_lap";
    
    [Tooltip("Display name for UI prompts (e.g., 'For Sale Sign', 'Start Line')")]
    public string locationDisplayName = "Mission";
    
    [Header("Trigger Settings")]
    [Tooltip("Only trigger when driving (not walking)")]
    public bool requireDriving = true;
    
    [Tooltip("Only trigger once per game session")]
    public bool triggerOnce = true;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private Color gizmoColor = new Color(0f, 1f, 1f, 0.3f); // Cyan
    
    private bool hasTriggered = false;
    
    void Start()
    {
        // Ensure this is a trigger
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"MissionTriggerZone '{name}': Collider is not a trigger! Setting isTrigger = true.");
            col.isTrigger = true;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if already triggered
        if (triggerOnce && hasTriggered)
            return;
        
        // Check if it's the player's vehicle
        if (!IsPlayerVehicle(other))
            return;
        
        // Check if player is driving (if required)
        if (requireDriving && !IsPlayerDriving())
            return;
        
        // Trigger the mission
        TriggerMission();
    }
    
    /// <summary>
    /// Check if the collider belongs to the player's vehicle
    /// </summary>
    private bool IsPlayerVehicle(Collider other)
    {
        // Check for car controller in parent hierarchy
        if (other.GetComponentInParent<PolyStang.CarController>() != null)
        {
            return true;
        }
        
        return false;
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
    
    /// <summary>
    /// Trigger the mission briefing
    /// </summary>
    private void TriggerMission()
    {
        if (MissionManager.Instance == null)
        {
            Debug.LogError("MissionTriggerZone: MissionManager not found!");
            return;
        }
        
        // Get the mission
        MissionData mission = MissionManager.Instance.GetMissionByID(missionID);
        if (mission == null)
        {
            Debug.LogError($"MissionTriggerZone: Mission '{missionID}' not found!");
            return;
        }
        
        // Check if mission is available
        if (!MissionManager.Instance.IsMissionAvailable(missionID))
        {
            Debug.LogWarning($"MissionTriggerZone: Mission '{missionID}' is not available yet!");
            return;
        }
        
        // Check if already completed
        if (MissionManager.Instance.IsMissionCompleted(missionID))
        {
            Debug.Log($"MissionTriggerZone: Mission '{missionID}' already completed!");
            return;
        }
        
        
        // TEACHING MOMENT: If walking is required but player is driving, show prompt
        if (requireDriving == false && IsPlayerDriving())
        {
            ShowWalkingRequiredPrompt();
            return;
        }
        // Show mission briefing
        MissionUIController missionUI = FindFirstObjectByType<MissionUIController>();
        if (missionUI != null)
        {
            missionUI.ShowMissionBriefing(mission);
            hasTriggered = true;
            
            Debug.Log($"[MissionTriggerZone] Triggered mission: {mission.missionName}");
        }
        else
        {
            Debug.LogError("MissionTriggerZone: MissionUIController not found!");
        }
    }
    
    /// <summary>
    /// Show teaching prompt when player tries to interact while driving
    /// </summary>
    private void ShowWalkingRequiredPrompt()
    {
        HUDController hud = FindFirstObjectByType<HUDController>();
        if (hud != null)
        {
            hud.ShowWalkingRequiredPrompt(locationDisplayName);
            Debug.Log($"[MissionTriggerZone '{missionID}'] Showing walking-required prompt");
        }
    }
    
    void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;
        
        Collider col = GetComponent<Collider>();
        if (col == null) return;
        
        // Draw trigger zone
        Gizmos.color = gizmoColor;
        
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
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 3f,
            $"MISSION TRIGGER\n{missionID}\n{(requireDriving ? "Driving Only" : "Any State")}"
        );
        #endif
    }
}