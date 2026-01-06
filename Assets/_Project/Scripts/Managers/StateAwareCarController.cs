using UnityEngine;
using PolyStang;

/// <summary>
/// Manages car controller based on player state
/// Only allows driving when in Driving state
/// </summary>
[RequireComponent(typeof(CarController))]
public class StateAwareCarController : MonoBehaviour
{
    private CarController carController;
    private CameraSwitcher cameraSwitcher;
    private AudioSource carAudioSource;
    
    void Start()
    {
        carController = GetComponent<CarController>();
        cameraSwitcher = GetComponent<CameraSwitcher>();
        carAudioSource = GetComponent<AudioSource>();
        
        // Subscribe to state changes
        if (PlayerStateManager.Instance != null)
        {
            PlayerStateManager.Instance.OnStateChanged.AddListener(OnStateChanged);
            
            // Set initial state
            OnStateChanged(PlayerStateManager.Instance.CurrentState);
        }
        else
        {
            Debug.LogError("StateAwareCarController: PlayerStateManager.Instance is null!");
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe to prevent memory leaks
        if (PlayerStateManager.Instance != null)
        {
            PlayerStateManager.Instance.OnStateChanged.RemoveListener(OnStateChanged);
        }
    }
    
    /// <summary>
    /// Called when player state changes
    /// </summary>
    private void OnStateChanged(PlayerStateManager.PlayerState newState)
    {
        bool isDriving = (newState == PlayerStateManager.PlayerState.Driving);
        
        // Enable/disable car scripts based on state
        if (carController != null)
            carController.enabled = isDriving;
        
        if (cameraSwitcher != null)
            cameraSwitcher.enabled = isDriving;
        
        // Enable/disable car audio
        if (carAudioSource != null)
            carAudioSource.enabled = isDriving;
        
        Debug.Log($"Car controller enabled: {isDriving}");
    }
}
