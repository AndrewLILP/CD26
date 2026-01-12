using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Singleton manager for scene transitions and state persistence
/// Handles saving/loading player state across scenes
/// </summary>
public class SceneTransitionManager : MonoBehaviour
{
    #region Singleton
    public static SceneTransitionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // Use transform.root to persist the root GameObject (works even if this is a child)
        DontDestroyOnLoad(transform.root.gameObject);
        
        Debug.Log($"[SceneTransitionManager] Initialized - Root '{transform.root.name}' set to DontDestroyOnLoad");
    }
    #endregion

    void Start()
    {
        Debug.Log($"[SceneTransitionManager] Start() called in scene: {SceneManager.GetActiveScene().name}");
    }

    #region Saved State
    [Header("Persistent State")]
    [SerializeField] private float savedCash = 0f;
    [SerializeField] private PlayerStateManager.PlayerState savedPlayerState = PlayerStateManager.PlayerState.Walking;
    [SerializeField] private string lastSceneName = "";
    [SerializeField] private string pendingSpawnPointName = "";
    
    private bool isTransitioning = false;
    #endregion

    #region Scene Loading
    
    /// <summary>
    /// Load a new scene and spawn player at specified spawn point
    /// </summary>
    public void LoadScene(string sceneName, string spawnPointName = "DefaultSpawn")
    {
        if (isTransitioning)
        {
            Debug.LogWarning("[SceneTransitionManager] Already transitioning, ignoring request");
            return;
        }
        
        Debug.Log($"[SceneTransitionManager] Loading scene: {sceneName}, spawn: {spawnPointName}");
        
        // Save current state before transitioning
        SavePlayerState();
        
        // Store pending spawn point
        pendingSpawnPointName = spawnPointName;
        
        // Start async scene load
        StartCoroutine(LoadSceneAsync(sceneName));
    }
    
    /// <summary>
    /// Asynchronously load scene with loading screen support
    /// </summary>
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        isTransitioning = true;
        
        // Optional: Show loading screen here
        // TODO: Fade to black / show loading UI
        
        yield return new WaitForSeconds(0.3f); // Brief fade delay
        
        // Start loading scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        if (asyncLoad == null)
        {
            Debug.LogError($"[SceneTransitionManager] Failed to load scene: {sceneName}");
            isTransitioning = false;
            yield break;
        }
        
        // Wait until scene is fully loaded (no longer preventing activation)
        while (!asyncLoad.isDone)
        {
            float progress = asyncLoad.progress / 0.9f; // AsyncOperation progress caps at 0.9
            Debug.Log($"[SceneTransitionManager] Loading: {Mathf.RoundToInt(progress * 100)}%");
            yield return null;
        }
        
        Debug.Log($"[SceneTransitionManager] Scene loaded: {sceneName}");
        
        // CRITICAL: Wait multiple frames for scene to fully initialize
        // This ensures all Awake() and Start() methods have run
        Debug.Log("[SceneTransitionManager] Waiting for scene initialization...");
        yield return new WaitForSeconds(0.2f);
        yield return new WaitForEndOfFrame();
        Debug.Log("[SceneTransitionManager] Scene initialization complete, spawning player...");
        
        // Now spawn player at designated spawn point
        try
        {
            SpawnPlayerAtPoint(pendingSpawnPointName);
            Debug.Log("[SceneTransitionManager] SpawnPlayerAtPoint completed successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[SceneTransitionManager] ERROR in SpawnPlayerAtPoint: {e.Message}\n{e.StackTrace}");
        }
        
        // Optional: Hide loading screen
        // TODO: Fade in from black
        
        Debug.Log("[SceneTransitionManager] Transition complete, setting isTransitioning = false");
        isTransitioning = false;
    }
    
    #endregion

    #region Player State Management
    
    /// <summary>
    /// Save current player state before scene transition
    /// </summary>
    private void SavePlayerState()
    {
        if (PlayerStateManager.Instance == null)
        {
            Debug.LogWarning("[SceneTransitionManager] PlayerStateManager not found, skipping state save");
            return;
        }
        
        // Save player state
        savedPlayerState = PlayerStateManager.Instance.CurrentState;
        
        // Save cash from HUD (if available)
        HUDController hud = FindFirstObjectByType<HUDController>();
        if (hud != null)
        {
            savedCash = hud.GetCurrentCash();
        }
        
        // Save current scene name
        lastSceneName = SceneManager.GetActiveScene().name;
        
        Debug.Log($"[SceneTransitionManager] State saved: {savedPlayerState}, Cash: ${savedCash}, Scene: {lastSceneName}");
    }
    
    /// <summary>
    /// Restore player state after scene load
    /// </summary>
    private void RestorePlayerState()
    {
        if (PlayerStateManager.Instance == null)
        {
            Debug.LogWarning("[SceneTransitionManager] PlayerStateManager not found, skipping state restore");
            return;
        }
        
        // Restore cash to HUD
        HUDController hud = FindFirstObjectByType<HUDController>();
        if (hud != null && savedCash >= 0)
        {
            hud.SetCash(savedCash);
        }
        
        // Restore player state (this will activate correct GameObject and camera)
        PlayerStateManager.Instance.RequestStateChange(savedPlayerState);
        
        Debug.Log($"[SceneTransitionManager] State restored: {savedPlayerState}, Cash: ${savedCash}");
    }
    
    #endregion

    #region Spawn System
    
    /// <summary>
    /// Spawn player at specified spawn point in current scene
    /// </summary>
    private void SpawnPlayerAtPoint(string spawnPointName)
    {
        Debug.Log("=== SPAWN PLAYER AT POINT CALLED ===");
        Debug.Log($"[SceneTransitionManager] SpawnPlayerAtPoint called for: {spawnPointName}");
        Debug.Log($"[SceneTransitionManager] Current scene: {SceneManager.GetActiveScene().name}");
        Debug.Log($"[SceneTransitionManager] Saved player state: {savedPlayerState}");
        
        // Find all spawn points in scene
        Debug.Log("[SceneTransitionManager] Searching for SceneSpawnPoint components...");
        SceneSpawnPoint[] spawnPoints = FindObjectsByType<SceneSpawnPoint>(FindObjectsSortMode.None);
        
        Debug.Log($"[SceneTransitionManager] Found {spawnPoints.Length} spawn points in scene");
        
        if (spawnPoints.Length == 0)
        {
            Debug.LogError($"[SceneTransitionManager] No spawn points found in scene '{SceneManager.GetActiveScene().name}'! Add SceneSpawnPoint components.");
            // Still try to restore state even without spawn points
            RestorePlayerState();
            return;
        }
        
        // Log all available spawn points for debugging
        foreach (var spawn in spawnPoints)
        {
            Debug.Log($"[SceneTransitionManager] Available spawn: '{spawn.spawnPointName}' at {spawn.transform.position}");
        }
        
        // Find matching spawn point
        SceneSpawnPoint targetSpawn = null;
        foreach (var spawn in spawnPoints)
        {
            if (spawn.spawnPointName == spawnPointName)
            {
                targetSpawn = spawn;
                break;
            }
        }
        
        // Fallback to first spawn point if not found
        if (targetSpawn == null)
        {
            Debug.LogWarning($"[SceneTransitionManager] Spawn point '{spawnPointName}' not found, using first available spawn: '{spawnPoints[0].spawnPointName}'");
            targetSpawn = spawnPoints[0];
        }
        else
        {
            Debug.Log($"[SceneTransitionManager] Found matching spawn point: '{targetSpawn.spawnPointName}'");
        }
        
        // Spawn player based on saved state
        if (savedPlayerState == PlayerStateManager.PlayerState.Driving)
        {
            SpawnPlayerDriving(targetSpawn);
        }
        else
        {
            SpawnPlayerWalking(targetSpawn);
        }
        
        // Restore state after spawning
        RestorePlayerState();
    }
    
    /// <summary>
    /// Spawn player in walking mode
    /// </summary>
    private void SpawnPlayerWalking(SceneSpawnPoint spawnPoint)
    {
        Debug.Log("=== SPAWN PLAYER WALKING CALLED ===");
        Debug.Log($"[SceneTransitionManager] SpawnPlayerWalking called for spawn: {spawnPoint.spawnPointName}");
        
        if (PlayerStateManager.Instance == null)
        {
            Debug.LogError("[SceneTransitionManager] PlayerStateManager.Instance is null!");
            return;
        }
        
        Debug.Log("[SceneTransitionManager] PlayerStateManager.Instance found");
        
        if (PlayerStateManager.Instance.characterObject == null)
        {
            Debug.LogError("[SceneTransitionManager] Character object not set in PlayerStateManager!");
            return;
        }
        
        Debug.Log($"[SceneTransitionManager] Character object found: {PlayerStateManager.Instance.characterObject.name}");
        
        GameObject character = PlayerStateManager.Instance.characterObject;
        Vector3 targetPosition = spawnPoint.GetSpawnPosition();
        Quaternion targetRotation = spawnPoint.GetSpawnRotation();
        
        Debug.Log($"[SceneTransitionManager] Target position: {targetPosition}");
        Debug.Log($"[SceneTransitionManager] Character current position: {character.transform.position}");
        Debug.Log($"[SceneTransitionManager] Moving character to spawn point...");
        
        // Disable CharacterController temporarily to allow teleportation
        CharacterController cc = character.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            Debug.Log("[SceneTransitionManager] CharacterController disabled for teleport");
        }
        else
        {
            Debug.LogWarning("[SceneTransitionManager] No CharacterController found on character");
        }
        
        // Move character to spawn point
        character.transform.position = targetPosition;
        character.transform.rotation = targetRotation;
        
        Debug.Log($"[SceneTransitionManager] Character transform.position SET to: {character.transform.position}");
        Debug.Log($"[SceneTransitionManager] Verifying position after set: {character.transform.position}");
        
        // Re-enable CharacterController
        if (cc != null)
        {
            cc.enabled = true;
            Debug.Log("[SceneTransitionManager] CharacterController re-enabled");
            Debug.Log($"[SceneTransitionManager] Final position after re-enable: {character.transform.position}");
        }
        
        Debug.Log($"[SceneTransitionManager] Spawned player (walking) at '{spawnPoint.spawnPointName}' ({targetPosition})");
        Debug.Log("=== SPAWN PLAYER WALKING COMPLETE ===");
    }
    
    /// <summary>
    /// Spawn player in driving mode (with vehicle)
    /// </summary>
    private void SpawnPlayerDriving(SceneSpawnPoint spawnPoint)
    {
        Debug.Log($"[SceneTransitionManager] SpawnPlayerDriving called");
        
        if (PlayerStateManager.Instance == null)
        {
            Debug.LogError("[SceneTransitionManager] PlayerStateManager.Instance is null!");
            return;
        }
        
        if (PlayerStateManager.Instance.vehicleObject == null)
        {
            Debug.LogError("[SceneTransitionManager] Vehicle object not set in PlayerStateManager!");
            return;
        }
        
        GameObject vehicle = PlayerStateManager.Instance.vehicleObject;
        Vector3 targetPosition = spawnPoint.GetSpawnPosition();
        Quaternion targetRotation = spawnPoint.GetSpawnRotation();
        
        Debug.Log($"[SceneTransitionManager] Moving vehicle to: {targetPosition}");
        
        // Move vehicle to spawn point
        vehicle.transform.position = targetPosition;
        vehicle.transform.rotation = targetRotation;
        
        // Reset vehicle physics
        Rigidbody rb = vehicle.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            Debug.Log("[SceneTransitionManager] Vehicle physics reset");
        }
        
        Debug.Log($"[SceneTransitionManager] Spawned player (driving) at '{spawnPoint.spawnPointName}' ({targetPosition})");
    }
    
    #endregion

    #region Public Helper Methods
    
    /// <summary>
    /// Check if currently transitioning between scenes
    /// </summary>
    public bool IsTransitioning()
    {
        return isTransitioning;
    }
    
    /// <summary>
    /// Get the last scene name (useful for "return to previous scene")
    /// </summary>
    public string GetLastSceneName()
    {
        return lastSceneName;
    }
    
    /// <summary>
    /// Reload current scene (useful for restart)
    /// </summary>
    public void ReloadCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        LoadScene(currentScene, "DefaultSpawn");
    }
    
    #endregion
}