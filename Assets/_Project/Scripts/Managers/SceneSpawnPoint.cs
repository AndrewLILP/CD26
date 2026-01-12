using UnityEngine;

/// <summary>
/// Marks spawn points in scenes for player spawning after scene transitions
/// Place these in your scenes where the player should appear
/// </summary>
public class SceneSpawnPoint : MonoBehaviour
{
    [Header("Spawn Point Configuration")]
    [Tooltip("Unique name for this spawn point (e.g., 'TownEntrance', 'TrackGarage')")]
    public string spawnPointName = "DefaultSpawn";
    
    [Tooltip("What state should the player be in when spawning here?")]
    public PlayerStateManager.PlayerState spawnState = PlayerStateManager.PlayerState.Walking;
    
    [Header("Visual Debugging")]
    [Tooltip("Show gizmo in editor")]
    [SerializeField] private bool showGizmo = true;
    [SerializeField] private Color gizmoColor = Color.green;
    [SerializeField] private float gizmoRadius = 1f;
    
    /// <summary>
    /// Get the spawn position (handles both walking and driving)
    /// </summary>
    public Vector3 GetSpawnPosition()
    {
        return transform.position;
    }
    
    /// <summary>
    /// Get the spawn rotation
    /// </summary>
    public Quaternion GetSpawnRotation()
    {
        return transform.rotation;
    }
    
    /// <summary>
    /// Get the spawn state this point is designed for
    /// </summary>
    public PlayerStateManager.PlayerState GetSpawnState()
    {
        return spawnState;
    }
    
    private void OnDrawGizmos()
    {
        if (!showGizmo) return;
        
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmoRadius);
        
        // Draw forward direction arrow
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);
        
        #if UNITY_EDITOR
        // Draw label
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 2f, 
            $"Spawn: {spawnPointName}\n({spawnState})"
        );
        #endif
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw larger sphere when selected
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, gizmoRadius * 1.5f);
    }
}
