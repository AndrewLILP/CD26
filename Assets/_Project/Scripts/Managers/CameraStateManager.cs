using UnityEngine;
using Cinemachine;

/// <summary>
/// Manages camera priority transitions during player state changes
/// Supports FreeLook primary camera with optional top-down alternate view
/// Ensures smooth camera blending without black screens
/// </summary>
public class CameraStateManager : MonoBehaviour
{
    [Header("Camera References")]
    [Tooltip("Primary driving camera (FreeLook or VirtualCamera) - the main view")]
    public CinemachineVirtualCameraBase drivingCamera;
    
    [Tooltip("Alternate top-down GPS-style camera (optional - toggled with CameraSwitcher)")]
    public CinemachineVirtualCamera topDownCamera;
    
    [Tooltip("Walking camera (FreeLook or VirtualCamera for character)")]
    public CinemachineVirtualCamera walkingCamera;
    
    [Header("Priority Settings")]
    [SerializeField] private int activePriority = 10;
    [SerializeField] private int alternatePriority = 5; // For top-down when not active
    [SerializeField] private int inactivePriority = 0;
    
    private CinemachineVirtualCameraBase currentActiveCamera;
    private bool isTopDownActive = false;
    
    void Start()
    {
        // Subscribe to state manager events
        if (PlayerStateManager.Instance != null)
        {
            PlayerStateManager.Instance.OnEnterDriving.AddListener(ActivateDrivingCamera);
            PlayerStateManager.Instance.OnEnterWalking.AddListener(ActivateWalkingCamera);
        }
        else
        {
            Debug.LogError("CameraStateManager: PlayerStateManager.Instance is null!");
        }
        
        // Set initial camera state (start with walking camera)
        ActivateWalkingCamera();
    }
    
    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (PlayerStateManager.Instance != null)
        {
            PlayerStateManager.Instance.OnEnterDriving.RemoveListener(ActivateDrivingCamera);
            PlayerStateManager.Instance.OnEnterWalking.RemoveListener(ActivateWalkingCamera);
        }
    }
    
    /// <summary>
    /// Switch to driving camera (primary FreeLook by default)
    /// </summary>
    public void ActivateDrivingCamera()
    {
        if (drivingCamera == null)
        {
            Debug.LogError("CameraStateManager: Driving camera not assigned!");
            return;
        }
        
        // Set primary driving camera to high priority
        drivingCamera.Priority = activePriority;
        
        // Set top-down to standby (lower priority but ready to switch)
        if (topDownCamera != null)
            topDownCamera.Priority = alternatePriority;
        
        // Set walking camera to inactive
        if (walkingCamera != null)
            walkingCamera.Priority = inactivePriority;
        
        currentActiveCamera = drivingCamera;
        isTopDownActive = false;
        
        Debug.Log("Camera: Switched to Driving (Primary)");
    }
    
    /// <summary>
    /// Switch to walking camera
    /// </summary>
    public void ActivateWalkingCamera()
    {
        if (walkingCamera == null)
        {
            Debug.LogError("CameraStateManager: Walking camera not assigned!");
            return;
        }
        
        // Set walking camera to high priority FIRST (prevents black screen)
        walkingCamera.Priority = activePriority;
        
        // Lower both driving cameras to inactive
        if (drivingCamera != null)
            drivingCamera.Priority = inactivePriority;
        if (topDownCamera != null)
            topDownCamera.Priority = inactivePriority;
        
        currentActiveCamera = walkingCamera;
        isTopDownActive = false;
        
        Debug.Log("Camera: Switched to Walking");
    }
    
    /// <summary>
    /// Toggle between primary driving camera and top-down view
    /// Called by CameraSwitcher when player presses camera switch button
    /// </summary>
    public void ToggleDrivingView()
    {
        if (drivingCamera == null || topDownCamera == null)
        {
            Debug.LogWarning("CameraStateManager: Cannot toggle - cameras not assigned!");
            return;
        }
        
        // Only toggle if we're in driving state
        if (PlayerStateManager.Instance == null || 
            PlayerStateManager.Instance.CurrentState != PlayerStateManager.PlayerState.Driving)
        {
            Debug.LogWarning("CameraStateManager: Cannot toggle camera - not in driving state!");
            return;
        }
        
        if (isTopDownActive)
        {
            // Switch back to primary driving camera
            drivingCamera.Priority = activePriority;
            topDownCamera.Priority = alternatePriority;
            currentActiveCamera = drivingCamera;
            isTopDownActive = false;
            
            Debug.Log("Camera: Switched to Primary Driving View");
        }
        else
        {
            // Switch to top-down view
            topDownCamera.Priority = activePriority;
            drivingCamera.Priority = alternatePriority;
            currentActiveCamera = topDownCamera;
            isTopDownActive = true;
            
            Debug.Log("Camera: Switched to Top-Down View");
        }
    }
    
    /// <summary>
    /// Get the currently active camera
    /// </summary>
    public CinemachineVirtualCameraBase GetActiveCamera()
    {
        return currentActiveCamera;
    }
    
    /// <summary>
    /// Check if currently using top-down view
    /// </summary>
    public bool IsTopDownActive()
    {
        return isTopDownActive;
    }
}
