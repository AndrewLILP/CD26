using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the pause menu overlay with volume control
/// Attach this to the same GameObject as HUDController (the one with UIDocument)
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    [Header("Audio References")]
    [Tooltip("Drag your car's AudioSource here (engine sound)")]
    [SerializeField] private AudioSource carAudioSource;
    
    [Header("Pause Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu"; // Change if different
    
    private VisualElement pauseMenu;
    private Button resumeButton;
    private Button quitButton;
    private Slider volumeSlider;
    private Label volumePercentageLabel;
    
    private bool isPaused = false;
    
    void Start()
    {
        // Get the UIDocument component
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("PauseMenuController: No UIDocument component found!");
            return;
        }
        
        var root = uiDocument.rootVisualElement;
        
        // Cache pause menu elements
        pauseMenu = root.Q<VisualElement>("pause-menu");
        resumeButton = root.Q<Button>("resume-button");
        quitButton = root.Q<Button>("quit-button");
        volumeSlider = root.Q<Slider>("volume-slider");
        volumePercentageLabel = root.Q<Label>("volume-percentage");
        
        // Validate references
        if (pauseMenu == null) Debug.LogError("PauseMenuController: 'pause-menu' not found!");
        if (resumeButton == null) Debug.LogError("PauseMenuController: 'resume-button' not found!");
        if (quitButton == null) Debug.LogError("PauseMenuController: 'quit-button' not found!");
        if (volumeSlider == null) Debug.LogError("PauseMenuController: 'volume-slider' not found!");
        
        // Register button callbacks
        if (resumeButton != null)
            resumeButton.clicked += ResumeGame;
        
        if (quitButton != null)
            quitButton.clicked += QuitToMenu;
        
        // Register volume slider callback
        if (volumeSlider != null)
            volumeSlider.RegisterValueChangedCallback(OnVolumeChanged);
        
        // Initialize volume from AudioSource
        InitializeVolume();
        
        // Ensure menu starts hidden
        if (pauseMenu != null)
            pauseMenu.AddToClassList("hidden");
    }
    
    void Update()
    {
        // Toggle pause with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }
    
    /// <summary>
    /// Initialize volume slider to match current AudioSource volume
    /// </summary>
    private void InitializeVolume()
    {
        if (carAudioSource != null && volumeSlider != null)
        {
            volumeSlider.value = carAudioSource.volume;
            UpdateVolumePercentageLabel(carAudioSource.volume);
        }
        else if (volumeSlider != null)
        {
            // Default to 75% if no AudioSource assigned
            volumeSlider.value = 0.75f;
            UpdateVolumePercentageLabel(0.75f);
        }
    }
    
    /// <summary>
    /// Called when volume slider value changes
    /// </summary>
    private void OnVolumeChanged(ChangeEvent<float> evt)
    {
        float newVolume = evt.newValue;
        
        // Update AudioSource volume
        if (carAudioSource != null)
        {
            carAudioSource.volume = newVolume;
        }
        
        // Update percentage label
        UpdateVolumePercentageLabel(newVolume);
    }
    
    /// <summary>
    /// Updates the volume percentage text
    /// </summary>
    private void UpdateVolumePercentageLabel(float volume)
    {
        if (volumePercentageLabel != null)
        {
            int percentage = Mathf.RoundToInt(volume * 100);
            volumePercentageLabel.text = $"{percentage}%";
        }
    }
    
    /// <summary>
    /// Pause the game and show menu
    /// </summary>
    public void PauseGame()
    {
        if (pauseMenu == null) return;
        
        isPaused = true;
        Time.timeScale = 0f; // Freeze game
        pauseMenu.RemoveFromClassList("hidden"); // Show menu
        
        // Optionally lock cursor
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
        
        Debug.Log("Game paused");
    }
    
    /// <summary>
    /// Resume the game and hide menu
    /// </summary>
    public void ResumeGame()
    {
        if (pauseMenu == null) return;
        
        isPaused = false;
        Time.timeScale = 1f; // Unfreeze game
        pauseMenu.AddToClassList("hidden"); // Hide menu
        
        // Optionally re-lock cursor for driving
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
        
        Debug.Log("Game resumed");
    }
    
    /// <summary>
    /// Quit to main menu
    /// </summary>
    public void QuitToMenu()
    {
        // Unfreeze time before scene change
        Time.timeScale = 1f;
        
        Debug.Log($"Loading main menu: {mainMenuSceneName}");
        
        // Load main menu scene
        // NOTE: You'll need to add your main menu scene to Build Settings
        SceneManager.LoadScene(mainMenuSceneName);
    }
    
    void OnDestroy()
    {
        // Unregister callbacks to prevent memory leaks
        if (resumeButton != null)
            resumeButton.clicked -= ResumeGame;
        
        if (quitButton != null)
            quitButton.clicked -= QuitToMenu;
        
        if (volumeSlider != null)
            volumeSlider.UnregisterValueChangedCallback(OnVolumeChanged);
        
        // Ensure time is unfrozen when scene unloads
        Time.timeScale = 1f;
    }
    
    // === PUBLIC ACCESSORS ===
    
    public bool IsPaused()
    {
        return isPaused;
    }
}
