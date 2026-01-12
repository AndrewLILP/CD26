using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Central state machine that manages player states (Driving, Walking, InBuilding)
/// Singleton pattern - access via PlayerStateManager.Instance
/// </summary>
public class PlayerStateManager : MonoBehaviour
{
    #region Singleton
    public static PlayerStateManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }
    #endregion

    #region State Definition
    public enum PlayerState
    {
        Driving,    // Player is controlling a vehicle
        Walking,    // Player is on foot (third-person)
        InBuilding  // Player is inside a building (for Sprint 4)
    }
    #endregion

    #region State Management
    [Header("Current State")]
    [SerializeField] private PlayerState currentState = PlayerState.Walking; // Start on foot by default
    
    public PlayerState CurrentState => currentState;

    /// <summary>
    /// Events fired when state changes
    /// Other systems can subscribe to react to state changes
    /// </summary>
    public UnityEvent<PlayerState> OnStateChanged = new UnityEvent<PlayerState>();
    
    /// <summary>
    /// Events for specific state transitions
    /// </summary>
    public UnityEvent OnEnterDriving = new UnityEvent();
    public UnityEvent OnExitDriving = new UnityEvent();
    public UnityEvent OnEnterWalking = new UnityEvent();
    public UnityEvent OnExitWalking = new UnityEvent();
    public UnityEvent OnEnterBuilding = new UnityEvent();
    public UnityEvent OnExitBuilding = new UnityEvent();
    #endregion

    #region References (Set in Phase 3)
    [Header("Player References")]
    [Tooltip("The car GameObject with CarController")]
    public GameObject vehicleObject;
    
    [Tooltip("The walking character GameObject with ThirdPersonController")]
    public GameObject characterObject;
    
    [Header("UI References")]
    [Tooltip("HUD controller to show/hide speed display")]
    public HUDController hudController;
    
    [Header("Timing")]
    [Tooltip("Delay before disabling GameObjects (allows camera blend)")]
    [SerializeField] private float disableDelay = 0.3f;
    
    [Header("Character Spawn Settings")]
    [Tooltip("Distance from car to spawn character when exiting")]
    [SerializeField] private float exitDistance = 2.0f;
    
    [Tooltip("Direction offset from car (1 = right side, -1 = left side)")]
    [SerializeField] private float exitSideOffset = 1.5f;
    
    // Private tracking
    private Vector3 lastCarPosition;
    private Quaternion lastCarRotation;
    #endregion

    void Start()
    {
        // Make character and vehicle persist across scenes
        // NOTE: DontDestroyOnLoad only works on root GameObjects, so we get the root transform
        if (characterObject != null)
        {
            Transform characterRoot = characterObject.transform.root;
            DontDestroyOnLoad(characterRoot.gameObject);
            Debug.Log($"PlayerStateManager: Character root '{characterRoot.name}' set to DontDestroyOnLoad");
        }
        
        if (vehicleObject != null)
        {
            Transform vehicleRoot = vehicleObject.transform.root;
            DontDestroyOnLoad(vehicleRoot.gameObject);
            Debug.Log($"PlayerStateManager: Vehicle root '{vehicleRoot.name}' set to DontDestroyOnLoad");
        }
    }

    #region State Transition Methods
    
    /// <summary>
    /// Request a state change (with validation)
    /// </summary>
    public void RequestStateChange(PlayerState newState)
    {
        if (currentState == newState)
        {
            Debug.LogWarning($"Already in {newState} state");
            return;
        }

        // Exit current state
        ExitState(currentState);
        
        // Change state
        PlayerState previousState = currentState;
        currentState = newState;
        
        // Enter new state
        EnterState(newState);
        
        // Fire events
        OnStateChanged?.Invoke(newState);
        
        Debug.Log($"State changed: {previousState} â†’ {newState}");
    }

    /// <summary>
    /// Enter a new state (activate appropriate systems)
    /// </summary>
    private void EnterState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Driving:
                EnterDrivingState();
                break;
            case PlayerState.Walking:
                EnterWalkingState();
                break;
            case PlayerState.InBuilding:
                EnterBuildingState();
                break;
        }
    }

    /// <summary>
    /// Exit current state (deactivate systems)
    /// </summary>
    private void ExitState(PlayerState state)
    {
        switch (state)
        {
            case PlayerState.Driving:
                ExitDrivingState();
                break;
            case PlayerState.Walking:
                ExitWalkingState();
                break;
            case PlayerState.InBuilding:
                ExitBuildingState();
                break;
        }
    }

    #endregion

    #region State-Specific Logic

    private void EnterDrivingState()
    {
        Debug.Log("Entering Driving state");
        
        // Fire event FIRST (camera switches to high priority)
        OnEnterDriving?.Invoke();
        
        // Activate vehicle immediately
        if (vehicleObject != null)
        {
            vehicleObject.SetActive(true);
            // Store car's position when entering (for when we exit)
            lastCarPosition = vehicleObject.transform.position;
            lastCarRotation = vehicleObject.transform.rotation;
        }
        
        // Deactivate character with delay (allows camera blend)
        if (characterObject != null)
            StartCoroutine(DelayedDisable(characterObject));
        
        // Show speedometer in HUD (Phase 3)
        // hudController?.ShowSpeedometer();
    }

    private void ExitDrivingState()
    {
        Debug.Log("Exiting Driving state");
        OnExitDriving?.Invoke();
    }

    private void EnterWalkingState()
    {
        Debug.Log("Entering Walking state");
        
        // Fire event FIRST (camera switches to high priority)
        OnEnterWalking?.Invoke();
        
        // Activate character immediately
        if (characterObject != null)
        {
            characterObject.SetActive(true);
            
            // If we have a stored car position, spawn character near it
            if (vehicleObject != null && lastCarPosition != Vector3.zero)
            {
                // Calculate spawn position (to the right of car)
                Vector3 spawnPosition = lastCarPosition + (lastCarRotation * Vector3.right * exitSideOffset);
                spawnPosition.y = lastCarPosition.y; // Keep at ground level
                
                // Use StateAwareCharacterController to set position (handles CharacterController properly)
                var characterController = characterObject.GetComponent<StateAwareCharacterController>();
                if (characterController != null)
                {
                    characterController.SetPosition(spawnPosition);
                    Debug.Log($"Character spawned at: {spawnPosition}");
                }
                else
                {
                    // Fallback: direct transform set
                    characterObject.transform.position = spawnPosition;
                }
            }
        }
        
        // Deactivate vehicle with delay (allows camera blend + keeps cameras alive)
        if (vehicleObject != null)
        {
            // Update car position before disabling (in case it moved since entering driving)
            lastCarPosition = vehicleObject.transform.position;
            lastCarRotation = vehicleObject.transform.rotation;
            
            StartCoroutine(DelayedDisable(vehicleObject));
        }
        
        // Hide speedometer in HUD (Phase 3)
        // hudController?.HideSpeedometer();
    }

    private void ExitWalkingState()
    {
        Debug.Log("Exiting Walking state");
        OnExitWalking?.Invoke();
    }

    private void EnterBuildingState()
    {
        Debug.Log("Entering Building state (Sprint 4)");
        
        // Fire event first
        OnEnterBuilding?.Invoke();
        
        // Deactivate both vehicle and character with delay
        if (vehicleObject != null)
            StartCoroutine(DelayedDisable(vehicleObject));
        if (characterObject != null)
            StartCoroutine(DelayedDisable(characterObject));
        
        // Switch to fixed building camera (Sprint 4)
    }

    private void ExitBuildingState()
    {
        Debug.Log("Exiting Building state");
        OnExitBuilding?.Invoke();
    }

    #endregion

    #region Debug Helpers (Remove in final build)

    void Update()
    {
        // Track car position while driving (so we know where to spawn character)
        if (currentState == PlayerState.Driving && vehicleObject != null)
        {
            lastCarPosition = vehicleObject.transform.position;
            lastCarRotation = vehicleObject.transform.rotation;
        }
        
        // Debug key bindings for testing state transitions
        // Remove these once vehicle interaction is implemented in Phase 3
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            RequestStateChange(PlayerState.Driving);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RequestStateChange(PlayerState.Walking);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            RequestStateChange(PlayerState.InBuilding);
        }
    }

    // Visualize current state in Inspector
    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 24;
        style.normal.textColor = Color.white;
        
        GUI.Label(new Rect(10, 10, 400, 30), $"Current State: {currentState}", style);
        GUI.Label(new Rect(10, 40, 400, 30), "Press 1=Driving, 2=Walking, 3=Building", style);
    }

    #endregion

    #region Public Helper Methods

    public bool IsDriving() => currentState == PlayerState.Driving;
    public bool IsWalking() => currentState == PlayerState.Walking;
    public bool IsInBuilding() => currentState == PlayerState.InBuilding;

    #endregion

    #region Helper Methods

    /// <summary>
    /// Coroutine to disable a GameObject after a delay
    /// This allows camera transitions to complete smoothly
    /// </summary>
    private System.Collections.IEnumerator DelayedDisable(GameObject target)
    {
        yield return new WaitForSeconds(disableDelay);
        target.SetActive(false);
        Debug.Log($"Delayed disable: {target.name}");
    }

    #endregion
}