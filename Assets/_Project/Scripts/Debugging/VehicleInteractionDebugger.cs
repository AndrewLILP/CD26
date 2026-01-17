using UnityEngine;

/// <summary>
/// DEBUG SCRIPT: Attach to your car to diagnose vehicle interaction issues
/// Remove this script once bugs are fixed
/// </summary>
public class VehicleInteractionDebugger : MonoBehaviour
{
    [Header("Auto-Check on Start")]
    public bool runDiagnosticsOnStart = true;
    
    void Start()
    {
        if (runDiagnosticsOnStart)
        {
            RunFullDiagnostics();
        }
    }
    
    void Update()
    {
        // Press F1 to run diagnostics manually
        if (Input.GetKeyDown(KeyCode.F1))
        {
            RunFullDiagnostics();
        }
        
        // Press F2 to check current state
        if (Input.GetKeyDown(KeyCode.F2))
        {
            CheckCurrentState();
        }
    }
    
    [ContextMenu("Run Full Diagnostics")]
    public void RunFullDiagnostics()
    {
        Debug.Log("=== VEHICLE INTERACTION DIAGNOSTICS ===");
        
        // 1. Check VehicleInteraction script
        VehicleInteraction vehicleInteraction = GetComponent<VehicleInteraction>();
        if (vehicleInteraction == null)
        {
            Debug.LogError("❌ VehicleInteraction script is MISSING from car!");
        }
        else
        {
            Debug.Log("✅ VehicleInteraction script found");
        }
        
        // 2. Check for trigger collider
        Collider[] colliders = GetComponents<Collider>();
        bool hasTrigger = false;
        foreach (var col in colliders)
        {
            if (col.isTrigger)
            {
                hasTrigger = true;
                Debug.Log($"✅ Trigger collider found: {col.GetType().Name}");
            }
        }
        
        if (!hasTrigger)
        {
            Debug.LogError("❌ Car has NO trigger collider! Add a SphereCollider with 'Is Trigger' checked.");
        }
        
        // 3. Check character exists and has "Player" tag
        if (PlayerStateManager.Instance != null && PlayerStateManager.Instance.characterObject != null)
        {
            GameObject character = PlayerStateManager.Instance.characterObject;
            Debug.Log($"✅ Character found: {character.name}");
            
            if (character.CompareTag("Player"))
            {
                Debug.Log("✅ Character has 'Player' tag");
            }
            else
            {
                Debug.LogError($"❌ Character tag is '{character.tag}' but should be 'Player'!");
            }
        }
        else
        {
            Debug.LogError("❌ Character object not found in PlayerStateManager!");
        }
        
        // 4. Check HUDController reference
        if (PlayerStateManager.Instance != null && PlayerStateManager.Instance.hudController != null)
        {
            Debug.Log("✅ HUDController reference exists");
        }
        else
        {
            Debug.LogError("❌ HUDController reference is NULL in PlayerStateManager!");
        }
        
        // 5. Check StateAwareCarController
        StateAwareCarController stateAwareCar = GetComponent<StateAwareCarController>();
        if (stateAwareCar == null)
        {
            Debug.LogWarning("⚠️ StateAwareCarController is MISSING (car will always be active)");
        }
        else
        {
            Debug.Log("✅ StateAwareCarController found");
        }
        
        Debug.Log("=== DIAGNOSTICS COMPLETE ===");
    }
    
    [ContextMenu("Check Current State")]
    public void CheckCurrentState()
    {
        Debug.Log("=== CURRENT STATE CHECK ===");
        
        if (PlayerStateManager.Instance != null)
        {
            Debug.Log($"Player State: {PlayerStateManager.Instance.CurrentState}");
            Debug.Log($"Character Active: {PlayerStateManager.Instance.characterObject?.activeSelf}");
            Debug.Log($"Vehicle Active: {PlayerStateManager.Instance.vehicleObject?.activeSelf}");
        }
        else
        {
            Debug.LogError("PlayerStateManager.Instance is NULL!");
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[VehicleDebug] Trigger ENTER: {other.gameObject.name} (Tag: {other.tag})");
        
        // Check if it's the player
        if (other.CompareTag("Player"))
        {
            Debug.Log("✅ Player detected in trigger!");
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        Debug.Log($"[VehicleDebug] Trigger EXIT: {other.gameObject.name} (Tag: {other.tag})");
    }
}
