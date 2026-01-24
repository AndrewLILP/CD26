using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Controls the Lap Reward popup display
/// Shows cash earned and message to return to Uncle Ray
/// Works alongside HUDController on the same GameObject
/// SPRINT 4: Core repeatable lap game loop UI
/// </summary>
public class LapRewardUIController : MonoBehaviour
{
    private UIDocument uiDocument;
    
    // UI Elements
    private VisualElement rewardPanel;
    private Label cashEarnedLabel;
    private Label bonusLabel;
    private Label messageLabel;
    private Label lapTimeLabel;
    private Button continueButton;
    
    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        
        if (uiDocument == null)
        {
            Debug.LogError("LapRewardUIController: No UIDocument found!");
            return;
        }
        
        var root = uiDocument.rootVisualElement;
        
        // Cache references
        rewardPanel = root.Q<VisualElement>("lap-reward-panel");
        cashEarnedLabel = root.Q<Label>("cash-earned");
        bonusLabel = root.Q<Label>("bonus-label");
        messageLabel = root.Q<Label>("reward-message");
        lapTimeLabel = root.Q<Label>("lap-time");
        continueButton = root.Q<Button>("continue-button");
        
        // Validate
        if (rewardPanel == null) Debug.LogError("LapRewardUIController: 'lap-reward-panel' not found!");
        if (cashEarnedLabel == null) Debug.LogError("LapRewardUIController: 'cash-earned' not found!");
        if (bonusLabel == null) Debug.LogError("LapRewardUIController: 'bonus-label' not found!");
        if (messageLabel == null) Debug.LogError("LapRewardUIController: 'reward-message' not found!");
        if (lapTimeLabel == null) Debug.LogError("LapRewardUIController: 'lap-time' not found!");
        if (continueButton == null) Debug.LogError("LapRewardUIController: 'continue-button' not found!");
        
        // Register callbacks
        if (continueButton != null)
        {
            continueButton.clicked += OnContinueButtonClicked;
        }
        
        // Start hidden
        HideRewardPanel();
        
        Debug.Log("[LapRewardUI] Initialized");
    }
    
    void Update()
    {
        // Allow Enter key to continue when panel is visible
        if (rewardPanel != null && !rewardPanel.ClassListContains("hidden"))
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnContinueButtonClicked();
            }
        }
    }
    
    void OnDestroy()
    {
        if (continueButton != null)
        {
            continueButton.clicked -= OnContinueButtonClicked;
        }
    }
    
    /// <summary>
    /// Show the lap reward popup
    /// </summary>
    /// <param name="cashEarned">Total cash earned (base + bonus)</param>
    /// <param name="beatPersonalBest">Whether personal best was beaten</param>
    /// <param name="lapTime">The lap time achieved</param>
    public void ShowReward(float cashEarned, bool beatPersonalBest, float lapTime)
    {
        if (rewardPanel == null) return;
        
        // Populate UI
        if (cashEarnedLabel != null)
        {
            cashEarnedLabel.text = $"+${cashEarned:N0}";
        }
        
        if (bonusLabel != null)
        {
            if (beatPersonalBest)
            {
                bonusLabel.text = "NEW PERSONAL BEST!";
                bonusLabel.RemoveFromClassList("hidden");
            }
            else
            {
                bonusLabel.AddToClassList("hidden");
            }
        }
        
        if (messageLabel != null)
        {
            messageLabel.text = "Return to Uncle Ray at the garage";
        }
        
        if (lapTimeLabel != null)
        {
            lapTimeLabel.text = $"Lap Time: {FormatTime(lapTime)}";
        }
        
        // Show panel
        rewardPanel.RemoveFromClassList("hidden");
        
        // Pause game
        Time.timeScale = 0f;
        
        Debug.Log($"[LapRewardUI] Showing reward: ${cashEarned:N0}, Best: {beatPersonalBest}");
    }
    
    /// <summary>
    /// Hide the reward panel
    /// </summary>
    public void HideRewardPanel()
    {
        if (rewardPanel == null) return;
        
        rewardPanel.AddToClassList("hidden");
        
        // Resume game
        Time.timeScale = 1f;
    }
    
    /// <summary>
    /// Called when continue button is clicked
    /// </summary>
    private void OnContinueButtonClicked()
    {
        HideRewardPanel();
        Debug.Log("[LapRewardUI] Player continued - back to free roam");
    }
    
    /// <summary>
    /// Format time as MM:SS.ms
    /// </summary>
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 100f) % 100f);
        
        return $"{minutes:00}:{seconds:00}.{milliseconds:00}";
    }
}
