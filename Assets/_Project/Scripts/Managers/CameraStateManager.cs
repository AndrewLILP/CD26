using UnityEngine;
using Cinemachine;

/// <summary>
/// Manages camera priority transitions during player state changes
/// Ensures smooth camera blending without black screens
/// </summary>
public class CameraStateManager : MonoBehaviour
{
    [Header("Camera References")]
    [Tooltip("Driving cameras (Rear and Front virtual cameras on car)")]
    public CinemachineVirtualCamera rearDrivingCamera;
    public CinemachineVirtualCamera frontDrivingCamera;
    
    [Tooltip("Walking camera (FreeLook or Follow for character)")]
    public CinemachineVirtualCamera walkingCamera;
    
    [Header("Priority Settings")]
    [SerializeField] private int activePriority = 10;
    [SerializeField] private int inactivePriority = 0;
    
    [Header("Transition Settings")]
    [Tooltip("Delay before disabling GameObjects (allows camera blend to complete)")]
    [SerializeField] private float transitionDelay = 0.3f;
    
    private CinemachineVirtualCamera currentActiveCamera;
    
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
    /// Switch to driving camera (rear camera by default)
    /// </summary>
    public void ActivateDrivingCamera()
    {
        if (rearDrivingCamera == null)
        {
            Debug.LogError("CameraStateManager: Rear driving camera not assigned!");
            return;
        }
        
        // Set driving camera to high priority
        rearDrivingCamera.Priority = activePriority;
        if (frontDrivingCamera != null)
            frontDrivingCamera.Priority = inactivePriority; // Keep front camera ready but inactive
        
        // Set walking camera to low priority
        if (walkingCamera != null)
            walkingCamera.Priority = inactivePriority;
        
        currentActiveCamera = rearDrivingCamera;
        
        Debug.Log("Camera: Switched to Driving (Rear)");
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
        
        // Then lower driving camera priorities
        if (rearDrivingCamera != null)
            rearDrivingCamera.Priority = inactivePriority;
        if (frontDrivingCamera != null)
            frontDrivingCamera.Priority = inactivePriority;
        
        currentActiveCamera = walkingCamera;
        
        Debug.Log("Camera: Switched to Walking");
    }
    
    /// <summary>
    /// Get the currently active camera
    /// </summary>
    public CinemachineVirtualCamera GetActiveCamera()
    {
        return currentActiveCamera;
    }
    
    /// <summary>
    /// Manual camera switch (for debugging or special cases)
    /// </summary>
    public void SwitchToCamera(CinemachineVirtualCamera targetCamera)
    {
        if (targetCamera == null) return;
        
        // Lower all camera priorities
        if (rearDrivingCamera != null) rearDrivingCamera.Priority = inactivePriority;
        if (frontDrivingCamera != null) frontDrivingCamera.Priority = inactivePriority;
        if (walkingCamera != null) walkingCamera.Priority = inactivePriority;
        
        // Raise target camera priority
        targetCamera.Priority = activePriority;
        currentActiveCamera = targetCamera;
    }
}
