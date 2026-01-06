using UnityEngine;
using StarterAssets;

/// <summary>
/// Manages third-person character based on player state
/// Only allows movement when in Walking state
/// </summary>
[RequireComponent(typeof(ThirdPersonController))]
[RequireComponent(typeof(StarterAssetsInputs))]
public class StateAwareCharacterController : MonoBehaviour
{
    private ThirdPersonController characterController;
    private StarterAssetsInputs inputController;
    
    void Start()
    {
        characterController = GetComponent<ThirdPersonController>();
        inputController = GetComponent<StarterAssetsInputs>();
        
        // Subscribe to state changes
        if (PlayerStateManager.Instance != null)
        {
            PlayerStateManager.Instance.OnStateChanged.AddListener(OnStateChanged);
            
            // Set initial state
            OnStateChanged(PlayerStateManager.Instance.CurrentState);
        }
        else
        {
            Debug.LogError("StateAwareCharacterController: PlayerStateManager.Instance is null!");
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
        bool isWalking = (newState == PlayerStateManager.PlayerState.Walking);
        
        // Enable/disable character scripts based on state
        if (characterController != null)
            characterController.enabled = isWalking;
        
        if (inputController != null)
            inputController.enabled = isWalking;
        
        Debug.Log($"Character controller enabled: {isWalking}");
    }
    
    /// <summary>
    /// Optional: Set character position when entering walking state
    /// Call this from PlayerStateManager when exiting vehicle
    /// </summary>
    public void SetPosition(Vector3 position)
    {
        // Disable CharacterController temporarily to teleport
        if (characterController != null)
        {
            var cc = GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                transform.position = position;
                cc.enabled = true;
            }
            else
            {
                transform.position = position;
            }
        }
        else
        {
            transform.position = position;
        }
    }
}
