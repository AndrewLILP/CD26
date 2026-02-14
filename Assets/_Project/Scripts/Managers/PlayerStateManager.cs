using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Central state machine that manages player states (Driving, Walking, InBuilding)
/// FIXED: Properly cancels delayed disable coroutines to prevent race conditions
/// </summary>
public class PlayerStateManager : MonoBehaviour
{
    #region Singleton
    public static PlayerStateManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(transform.root.gameObject);
    }
    #endregion

    #region State Definition
    public enum PlayerState
    {
        Driving,
        Walking,
        InBuilding
    }
    #endregion

    #region State Management
    [Header("Current State")]
    [SerializeField] private PlayerState currentState = PlayerState.Walking;
    
    public PlayerState CurrentState => currentState;

    public UnityEvent<PlayerState> OnStateChanged = new UnityEvent<PlayerState>();
    
    public UnityEvent OnEnterDriving = new UnityEvent();
    public UnityEvent OnExitDriving = new UnityEvent();
    public UnityEvent OnEnterWalking = new UnityEvent();
    public UnityEvent OnExitWalking = new UnityEvent();
    public UnityEvent OnEnterBuilding = new UnityEvent();
    public UnityEvent OnExitBuilding = new UnityEvent();
    #endregion

    #region References
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
    [SerializeField] private float disableDelay = 0.5f;
    
    [Header("Character Spawn Settings")]
    [Tooltip("Distance from car to spawn character when exiting")]
    [SerializeField] private float exitDistance = 2.0f;
    
    [Tooltip("Direction offset from car (1 = right side, -1 = left side)")]
    [SerializeField] private float exitSideOffset = 1.5f;
    
    // Private tracking
    private Vector3 lastCarPosition;
    private Quaternion lastCarRotation;
    
    // CRITICAL FIX: Track coroutines to cancel them
    private Coroutine vehicleDisableCoroutine;
    private Coroutine characterDisableCoroutine;
    #endregion

    void Start()
    {
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
    
    public void RequestStateChange(PlayerState newState)
    {
        if (currentState == newState)
        {
            Debug.LogWarning($"Already in {newState} state");
            return;
        }

        // CRITICAL: Cancel any pending disable coroutines before state change
        CancelPendingDisables();

        ExitState(currentState);
        
        PlayerState previousState = currentState;
        currentState = newState;
        
        EnterState(newState);
        
        OnStateChanged?.Invoke(newState);
        
        Debug.Log($"State changed: {previousState} → {newState}");
    }

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
        
        OnEnterDriving?.Invoke();
        
        // Activate vehicle immediately
        if (vehicleObject != null)
        {
            vehicleObject.SetActive(true);
            lastCarPosition = vehicleObject.transform.position;
            lastCarRotation = vehicleObject.transform.rotation;
        }
        
        // Deactivate character with delay
        if (characterObject != null)
        {
            characterDisableCoroutine = StartCoroutine(DelayedDisable(characterObject));
        }
    }

    private void ExitDrivingState()
    {
        Debug.Log("Exiting Driving state");
        OnExitDriving?.Invoke();
    }

    private void EnterWalkingState()
    {
        Debug.Log("Entering Walking state");
        
        OnEnterWalking?.Invoke();
        
        // Activate character immediately
        if (characterObject != null)
        {
            characterObject.SetActive(true);
            
            // Spawn character near car if we have a stored position
            if (vehicleObject != null && lastCarPosition != Vector3.zero)
            {
                Vector3 spawnPosition = lastCarPosition + (lastCarRotation * Vector3.right * exitSideOffset);
                spawnPosition.y = lastCarPosition.y;
                
                var characterController = characterObject.GetComponent<StateAwareCharacterController>();
                if (characterController != null)
                {
                    characterController.SetPosition(spawnPosition);
                    Debug.Log($"Character spawned at: {spawnPosition}");
                }
                else
                {
                    characterObject.transform.position = spawnPosition;
                    Debug.Log($"Character spawned at: {spawnPosition} (fallback)");
                }
            }
        }
        
        if (vehicleObject != null)
            {
                lastCarPosition = vehicleObject.transform.position;
                lastCarRotation = vehicleObject.transform.rotation;
                // vehicleDisableCoroutine = StartCoroutine(DelayedDisable(vehicleObject)); // REMOVED
                Debug.Log("Vehicle kept active (scripts disabled by StateAwareCarController)");
            }
    }

    private void ExitWalkingState()
    {
        Debug.Log("Exiting Walking state");
        OnExitWalking?.Invoke();
    }

    private void EnterBuildingState()
    {
        Debug.Log("Entering Building state (Sprint 4)");
        
        OnEnterBuilding?.Invoke();
        
        // Deactivate both with delay
        if (vehicleObject != null)
            vehicleDisableCoroutine = StartCoroutine(DelayedDisable(vehicleObject));
        if (characterObject != null)
            characterDisableCoroutine = StartCoroutine(DelayedDisable(characterObject));
    }

    private void ExitBuildingState()
    {
        Debug.Log("Exiting Building state");
        OnExitBuilding?.Invoke();
    }

    #endregion

    #region Debug Helpers

    void Update()
    {
        if (currentState == PlayerState.Driving && vehicleObject != null)
        {
            lastCarPosition = vehicleObject.transform.position;
            lastCarRotation = vehicleObject.transform.rotation;
        }
        
        // Debug key bindings
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
    /// CRITICAL FIX: Cancel any pending disable coroutines before state transitions
    /// This prevents race conditions where a delayed disable fires after re-enabling
    /// </summary>
    private void CancelPendingDisables()
    {
        if (vehicleDisableCoroutine != null)
        {
            StopCoroutine(vehicleDisableCoroutine);
            vehicleDisableCoroutine = null;
            Debug.Log("Cancelled pending vehicle disable");
        }
        
        if (characterDisableCoroutine != null)
        {
            StopCoroutine(characterDisableCoroutine);
            characterDisableCoroutine = null;
            Debug.Log("Cancelled pending character disable");
        }
    }

    /// <summary>
    /// Coroutine to disable a GameObject after a delay
    /// </summary>
    private System.Collections.IEnumerator DelayedDisable(GameObject target)
    {
        yield return new WaitForSeconds(disableDelay);
        
        // Double-check we're still supposed to disable this
        // (in case state changed during the delay)
        bool shouldDisable = false;
        
        if (target == vehicleObject && currentState != PlayerState.Driving)
        {
            shouldDisable = true;
        }
        else if (target == characterObject && currentState != PlayerState.Walking)
        {
            shouldDisable = true;
        }
        
        if (shouldDisable)
        {
            target.SetActive(false);
            Debug.Log($"Delayed disable: {target.name}");
        }
        else
        {
            Debug.Log($"Skipped delayed disable: {target.name} (state changed)");
        }
        
        // Clear the coroutine reference
        if (target == vehicleObject)
            vehicleDisableCoroutine = null;
        else if (target == characterObject)
            characterDisableCoroutine = null;
    }

    #endregion
}