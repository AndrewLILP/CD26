using UnityEngine;

/// <summary>
/// Trigger zone that prompts player to enter a new scene
/// Supports walking, driving, or both modes
/// </summary>
[RequireComponent(typeof(Collider))]
public class SceneTransitionZone : MonoBehaviour
{
    [Header("Scene Configuration")]
    [Tooltip("Name of the scene to load (must be in Build Settings). Leave empty for 'Exit Vehicle Only' mode.")]
    public string targetSceneName;
    
    [Tooltip("Spawn point name in the target scene")]
    public string targetSpawnPointName = "DefaultSpawn";
    
    [Tooltip("Display name for UI prompt (e.g., 'Town', 'Garage', 'Café')")]
    public string locationDisplayName = "Building";
    
    [Header("Behavior Mode")]
    [Tooltip("If true, this zone only exits the vehicle without changing scenes (Scenario 1: Exit Circuit)")]
    public bool exitVehicleOnly = false;
    
    public enum TransitionMode
    {
        WalkingOnly,    // Only works when player is on foot
        DrivingOnly,    // Only works when player is in a vehicle
        Both            // Works in either state
    }
    
    [Header("Interaction Settings")]
    [Tooltip("When can this transition be triggered?")]
    public TransitionMode transitionMode = TransitionMode.Both;
    
    [Tooltip("Should this zone disable/enable GameObjects when entered? (e.g., track barriers)")]
    public bool toggleObjects = false;
    
    [Tooltip("Objects to disable when entering this zone")]
    public GameObject[] objectsToDisable;
    
    [Tooltip("Objects to enable when entering this zone")]
    public GameObject[] objectsToEnable;
    
    [Header("Visual Debugging")]
    [SerializeField] private bool showGizmo = true;
    [SerializeField] private Color gizmoColor = new Color(1f, 0.5f, 0f, 0.3f); // Orange
    
    private bool playerInZone = false;
    private bool promptShown = false;
    
    private void Start()
    {
        // Ensure trigger is set up
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"SceneTransitionZone '{name}': Collider is not a trigger! Setting isTrigger = true.");
            col.isTrigger = true;
        }
        
        // Validate configuration based on mode
        if (exitVehicleOnly)
        {
            // Exit Vehicle Only mode - no scene name needed
            if (transitionMode != TransitionMode.DrivingOnly)
            {
                Debug.LogWarning($"SceneTransitionZone '{name}': exitVehicleOnly is true but transitionMode is not DrivingOnly. Setting to DrivingOnly.");
                transitionMode = TransitionMode.DrivingOnly;
            }
            Debug.Log($"SceneTransitionZone '{name}': Configured as Exit Vehicle Only (no scene transition)");
        }
        else
        {
            // Scene transition mode - scene name required
            if (string.IsNullOrEmpty(targetSceneName))
            {
                Debug.LogError($"SceneTransitionZone '{name}': No target scene name specified and exitVehicleOnly is false!");
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if player entered (either character or vehicle)
        if (IsPlayer(other))
        {
            playerInZone = true;
            CheckAndShowPrompt();
            
            // Toggle objects if enabled
            if (toggleObjects)
            {
                ToggleZoneObjects();
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other))
        {
            playerInZone = false;
            HidePrompt();
        }
    }
    
    private void Update()
    {
        if (!playerInZone) return;
        
        // Check if prompt should be shown (state might have changed)
        CheckAndShowPrompt();
        
        // Check for E key press
        if (Input.GetKeyDown(KeyCode.E) && promptShown)
        {
            TriggerTransition();
        }
    }
    
    /// <summary>
    /// Check if the collider belongs to the player
    /// Uses GetComponentInParent to find components on parent GameObjects
    /// This is critical because wheel/body colliders are children of the car root
    /// </summary>
    private bool IsPlayer(Collider other)
    {
        // Check for character controller (walking) - search up hierarchy
        if (other.GetComponentInParent<StarterAssets.ThirdPersonController>() != null)
        {
            Debug.Log($"[SceneTransitionZone '{name}'] Character detected via collider: {other.name}");
            return true;
        }
        
        // Check for car controller (driving) - search up hierarchy
        if (other.GetComponentInParent<PolyStang.CarController>() != null)
        {
            Debug.Log($"[SceneTransitionZone '{name}'] Vehicle detected via collider: {other.name}");
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Check if transition is valid for current player state
    /// </summary>
    private bool IsTransitionValid()
    {
        if (PlayerStateManager.Instance == null) return false;
        
        var currentState = PlayerStateManager.Instance.CurrentState;
        
        switch (transitionMode)
        {
            case TransitionMode.WalkingOnly:
                return currentState == PlayerStateManager.PlayerState.Walking;
            
            case TransitionMode.DrivingOnly:
                return currentState == PlayerStateManager.PlayerState.Driving;
            
            case TransitionMode.Both:
                return currentState == PlayerStateManager.PlayerState.Walking || 
                       currentState == PlayerStateManager.PlayerState.Driving;
            
            default:
                return false;
        }
    }
    
    /// <summary>
    /// Check if prompt should be shown and display it
    /// </summary>
    private void CheckAndShowPrompt()
    {
        if (!IsTransitionValid())
        {
            HidePrompt();
            return;
        }
        
        // Determine prompt text based on mode
        string promptText;
        if (exitVehicleOnly)
        {
            promptText = $"Press E to {locationDisplayName}";
        }
        else
        {
            promptText = $"Press E to Enter {locationDisplayName}";
        }
        
        // TODO: Show UI prompt with promptText
        // This will be implemented when we add the UI system
        promptShown = true;
        
        Debug.Log($"[SceneTransitionZone '{name}'] {promptText}");
    }
    
    /// <summary>
    /// Hide the interaction prompt
    /// </summary>
    private void HidePrompt()
    {
        // TODO: Hide UI prompt
        promptShown = false;
    }
    
    /// <summary>
    /// Toggle zone objects on/off
    /// </summary>
    private void ToggleZoneObjects()
    {
        foreach (var obj in objectsToDisable)
        {
            if (obj != null)
                obj.SetActive(false);
        }
        
        foreach (var obj in objectsToEnable)
        {
            if (obj != null)
                obj.SetActive(true);
        }
        
        Debug.Log($"[Scene Transition] Toggled {objectsToDisable.Length} objects off, {objectsToEnable.Length} objects on");
    }
    
    /// <summary>
    /// Trigger the scene transition or vehicle exit
    /// </summary>
    private void TriggerTransition()
    {
        if (exitVehicleOnly)
        {
            // Scenario 1: Exit vehicle without changing scenes
            if (PlayerStateManager.Instance == null)
            {
                Debug.LogError("SceneTransitionZone: PlayerStateManager.Instance is null!");
                return;
            }
            
            if (PlayerStateManager.Instance.CurrentState != PlayerStateManager.PlayerState.Driving)
            {
                Debug.LogWarning($"[SceneTransitionZone '{name}'] Exit vehicle requested but player is not driving!");
                return;
            }
            
            Debug.Log($"[SceneTransitionZone '{name}'] Exiting vehicle (staying in current scene)");
            PlayerStateManager.Instance.RequestStateChange(PlayerStateManager.PlayerState.Walking);
        }
        else
        {
            // Scenario 2: Scene transition (with or without vehicle)
            if (SceneTransitionManager.Instance == null)
            {
                Debug.LogError("SceneTransitionZone: SceneTransitionManager.Instance is null!");
                return;
            }
            
            Debug.Log($"[SceneTransitionZone '{name}'] Transitioning to {targetSceneName} (spawn: {targetSpawnPointName})");
            SceneTransitionManager.Instance.LoadScene(targetSceneName, targetSpawnPointName);
        }
    }
    
    private void OnDrawGizmos()
    {
        if (!showGizmo) return;
        
        Collider col = GetComponent<Collider>();
        if (col == null) return;
        
        // Draw zone bounds
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
        string modeText = transitionMode switch
        {
            TransitionMode.WalkingOnly => "👤 Walking",
            TransitionMode.DrivingOnly => "🚗 Driving",
            TransitionMode.Both => "👤🚗 Both",
            _ => ""
        };
        
        string behaviorText = exitVehicleOnly ? "EXIT VEHICLE" : $"→ {targetSceneName}";
        
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 3f,
            $"{locationDisplayName}\n{modeText}\n{behaviorText}"
        );
        #endif
    }
}