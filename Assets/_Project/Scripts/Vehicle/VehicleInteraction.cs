using UnityEngine;

/// <summary>
/// Handles vehicle entry/exit interactions
/// Attach this to your vehicle GameObject with a trigger collider
/// </summary>
[RequireComponent(typeof(Collider))]
public class VehicleInteraction : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag your HUDController here")]
    [SerializeField] private HUDController hudController;
    
    [Header("Interaction Settings")]
    [Tooltip("Distance from car where prompt appears")]
    [SerializeField] private float interactionRadius = 3.0f;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    
    private bool playerNearby = false;
    private GameObject playerCharacter = null;
    
    void Start()
    {
        // Ensure this GameObject has a trigger collider
        Collider[] colliders = GetComponents<Collider>();
        bool hasTrigger = false;
        foreach (var col in colliders)
        {
            if (col.isTrigger)
            {
                hasTrigger = true;
                break;
            }
        }
        
        if (!hasTrigger)
        {
            Debug.LogWarning("VehicleInteraction: No trigger collider found! Adding SphereCollider as trigger.");
            SphereCollider triggerCollider = gameObject.AddComponent<SphereCollider>();
            triggerCollider.isTrigger = true;
            triggerCollider.radius = interactionRadius;
        }
        
        if (hudController == null)
        {
            hudController = FindObjectOfType<HUDController>();
            if (hudController == null)
            {
                Debug.LogError("VehicleInteraction: HUDController not found!");
            }
        }
    }
    
    void Update()
    {
        // Check for E key press
        if (Input.GetKeyDown(KeyCode.E))
        {
            HandleInteraction();
        }
        
        // Update prompt visibility based on state
        UpdatePromptDisplay();
    }
    
    /// <summary>
    /// Handle E key press based on current state
    /// </summary>
    private void HandleInteraction()
    {
        if (PlayerStateManager.Instance == null) return;
        
        PlayerStateManager.PlayerState currentState = PlayerStateManager.Instance.CurrentState;
        
        if (currentState == PlayerStateManager.PlayerState.Walking && playerNearby)
        {
            // Player is walking and near car → Enter vehicle
            PlayerStateManager.Instance.RequestStateChange(PlayerStateManager.PlayerState.Driving);
            Debug.Log("VehicleInteraction: Player entered vehicle via E key");
        }
        else if (currentState == PlayerStateManager.PlayerState.Driving)
        {
            // Player is driving → Exit vehicle
            PlayerStateManager.Instance.RequestStateChange(PlayerStateManager.PlayerState.Walking);
            Debug.Log("VehicleInteraction: Player exited vehicle via E key");
        }
    }
    
    /// <summary>
    /// Update prompt display based on proximity and state
    /// </summary>
    private void UpdatePromptDisplay()
    {
        if (hudController == null || PlayerStateManager.Instance == null) return;
        
        PlayerStateManager.PlayerState currentState = PlayerStateManager.Instance.CurrentState;
        
        if (currentState == PlayerStateManager.PlayerState.Walking && playerNearby)
        {
            // Show "Press E to Enter" when walking near car
            hudController.ShowEnterPrompt();
        }
        else if (currentState == PlayerStateManager.PlayerState.Driving)
        {
            // Show "Press E to Exit" when driving
            hudController.ShowExitPrompt();
        }
        else
        {
            // Hide prompt when walking but not near car
            hudController.HideInteractionPrompt();
        }
    }
    
    /// <summary>
    /// Detect when player enters interaction radius
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        // Check if it's the player character
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            playerCharacter = other.gameObject;
            Debug.Log("VehicleInteraction: Player entered interaction radius");
        }
    }
    
    /// <summary>
    /// Detect when player leaves interaction radius
    /// </summary>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            playerCharacter = null;
            
            // Hide prompt when leaving car
            if (hudController != null && PlayerStateManager.Instance.CurrentState == PlayerStateManager.PlayerState.Walking)
            {
                hudController.HideInteractionPrompt();
            }
            
            Debug.Log("VehicleInteraction: Player left interaction radius");
        }
    }
    
    /// <summary>
    /// Draw interaction radius in Scene view
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (showDebugGizmos)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
}
