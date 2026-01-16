using UnityEngine;

/// <summary>
/// Individual checkpoint for lap validation
/// Must be crossed in order before finish line is valid
/// </summary>
[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    [Header("Checkpoint Configuration")]
    [Tooltip("Order in sequence (1 = first checkpoint, 2 = second, etc.)")]
    public int checkpointNumber = 1;
    
    [Tooltip("Reference to the LapTimer managing this lap")]
    public LapTimer lapTimer;
    
    [Header("Visual Feedback")]
    [Tooltip("Optional: Object to disable when checkpoint is crossed")]
    public GameObject visualIndicator;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private Color inactiveColor = new Color(1f, 0.5f, 0f, 0.3f); // Orange
    [SerializeField] private Color activeColor = new Color(0f, 1f, 0f, 0.5f); // Green
    [SerializeField] private Color completedColor = new Color(0.5f, 0.5f, 0.5f, 0.2f); // Gray
    
    private bool isActive = false;
    private bool isCompleted = false;
    
    void Start()
    {
        // Ensure this is a trigger
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"Checkpoint '{name}': Collider is not a trigger! Setting isTrigger = true.");
            col.isTrigger = true;
        }
        
        // Auto-find LapTimer if not assigned
        if (lapTimer == null)
        {
            lapTimer = FindFirstObjectByType<LapTimer>();
            if (lapTimer != null)
            {
                Debug.Log($"Checkpoint {checkpointNumber}: Auto-found LapTimer");
            }
        }
        
        // Register with LapTimer
        if (lapTimer != null)
        {
            lapTimer.RegisterCheckpoint(this);
        }
        else
        {
            Debug.LogError($"Checkpoint {checkpointNumber}: No LapTimer assigned or found!");
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Only active checkpoints can be crossed
        if (!isActive || isCompleted)
            return;
        
        // Check if it's the player's vehicle
        if (!IsPlayerVehicle(other))
            return;
        
        // Check if player is driving
        if (!IsPlayerDriving())
            return;
        
        // Checkpoint crossed!
        CrossCheckpoint();
    }
    
    /// <summary>
    /// Mark this checkpoint as crossed
    /// </summary>
    private void CrossCheckpoint()
    {
        isCompleted = true;
        isActive = false;
        
        // Hide visual indicator
        if (visualIndicator != null)
        {
            visualIndicator.SetActive(false);
        }
        
        // Notify LapTimer
        if (lapTimer != null)
        {
            lapTimer.OnCheckpointCrossed(checkpointNumber);
        }
        
        Debug.Log($"[Checkpoint {checkpointNumber}] ✓ Crossed!");
    }
    
    /// <summary>
    /// Activate this checkpoint (called by LapTimer)
    /// </summary>
    public void Activate()
    {
        if (isCompleted)
            return;
        
        isActive = true;
        
        // Show visual indicator
        if (visualIndicator != null)
        {
            visualIndicator.SetActive(true);
        }
        
        Debug.Log($"[Checkpoint {checkpointNumber}] Now active");
    }
    
    /// <summary>
    /// Reset checkpoint for new lap
    /// </summary>
    public void Reset()
    {
        isActive = false;
        isCompleted = false;
        
        if (visualIndicator != null)
        {
            visualIndicator.SetActive(false);
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
        
        // Choose color based on state
        Color gizmoColor = inactiveColor;
        if (isCompleted)
            gizmoColor = completedColor;
        else if (isActive)
            gizmoColor = activeColor;
        
        // Draw checkpoint zone
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
        string status = isCompleted ? "✓ COMPLETED" : (isActive ? "★ ACTIVE" : "○ WAITING");
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 4f,
            $"CHECKPOINT {checkpointNumber}\n{status}"
        );
        #endif
    }
}