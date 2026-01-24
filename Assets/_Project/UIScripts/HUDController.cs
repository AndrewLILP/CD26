using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Manages the in-game HUD elements (speedometer, cash display, lap timer, interaction prompts)
/// SPRINT 4 ENHANCED: Lap timer display + NPC dialogue support
/// </summary>
public class HUDController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag your car's Rigidbody here")]
    [SerializeField] private Rigidbody carRigidbody;
    
    [Header("Cash Settings")]
    [SerializeField] private float currentCash = 0f;
    [SerializeField] private float cashGoal = 10000f;
    
    [Header("Speed Settings")]
    [Tooltip("Speed update frequency (lower = smoother but more expensive)")]
    [SerializeField] private float updateInterval = 0.1f;
    
    // UI Elements - Core HUD
    private Label speedValueLabel;
    private Label cashCurrentLabel;
    private Label cashGoalLabel;
    private VisualElement interactionPrompt;
    private Label interactionText;
    
    // UI Elements - Lap Timer
    private VisualElement lapTimerPanel;
    private Label currentLapTimeLabel;
    private Label personalBestLabel;
    
    private float updateTimer;
    
    void Start()
    {
        // Get the UIDocument component
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("HUDController: No UIDocument component found!");
            return;
        }
        
        var root = uiDocument.rootVisualElement;
        
        // Cache UI element references
        speedValueLabel = root.Q<Label>("speed-value");
        cashCurrentLabel = root.Q<Label>("cash-current");
        cashGoalLabel = root.Q<Label>("cash-goal");
        interactionPrompt = root.Q<VisualElement>("interaction-prompt");
        interactionText = root.Q<Label>("interaction-text");
        
        // Cache lap timer elements
        lapTimerPanel = root.Q<VisualElement>("lap-timer-panel");
        currentLapTimeLabel = root.Q<Label>("current-lap-time");
        personalBestLabel = root.Q<Label>("personal-best-time");
        
        // Validate references
        if (speedValueLabel == null) Debug.LogError("HUDController: 'speed-value' label not found!");
        if (cashCurrentLabel == null) Debug.LogError("HUDController: 'cash-current' label not found!");
        if (cashGoalLabel == null) Debug.LogError("HUDController: 'cash-goal' label not found!");
        if (interactionPrompt == null) Debug.LogError("HUDController: 'interaction-prompt' not found!");
        if (interactionText == null) Debug.LogError("HUDController: 'interaction-text' not found!");
        if (lapTimerPanel == null) Debug.LogError("HUDController: 'lap-timer-panel' not found!");
        if (currentLapTimeLabel == null) Debug.LogError("HUDController: 'current-lap-time' not found!");
        if (personalBestLabel == null) Debug.LogError("HUDController: 'personal-best-time' not found!");
     
        // Initialize displays
        UpdateCashDisplay();
        HideInteractionPrompt();
        HideLapTimer();
    }
    
    void Update()
    {
        // Update speedometer at intervals (optimization)
        updateTimer += Time.deltaTime;
        if (updateTimer >= updateInterval)
        {
            UpdateSpeedometer();
            UpdateLapTimer();
            updateTimer = 0f;
        }
    }
    
    /// <summary>
    /// Updates the speedometer display based on car velocity
    /// </summary>
    private void UpdateSpeedometer()
    {
        if (carRigidbody == null || speedValueLabel == null) return;
        
        // Convert velocity to km/h
        float speedMS = carRigidbody.linearVelocity.magnitude;
        float speedKMH = speedMS * 3.6f;
        
        speedValueLabel.text = Mathf.RoundToInt(speedKMH).ToString();
    }
    
    /// <summary>
    /// Updates the lap timer display during active laps
    /// </summary>
    private void UpdateLapTimer()
    {
        // Find LapTimer in scene
        LapTimer lapTimer = FindFirstObjectByType<LapTimer>();
        if (lapTimer == null) return;
        
        if (lapTimer.LapInProgress)
        {
            ShowLapTimer();
            
            // Update current lap time
            if (currentLapTimeLabel != null)
            {
                currentLapTimeLabel.text = $"Current: {FormatTime(lapTimer.CurrentLapTime)}";
            }
            
            // Update personal best
            if (personalBestLabel != null)
            {
                if (lapTimer.PersonalBest == Mathf.Infinity)
                {
                    personalBestLabel.text = "Best: --:--.-";
                }
                else
                {
                    personalBestLabel.text = $"Best: {FormatTime(lapTimer.PersonalBest)}";
                }
            }
        }
        else
        {
            HideLapTimer();
        }
    }
    
    /// <summary>
    /// Format time as MM:SS.ms
    /// </summary>
    /// 
    
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((timeInSeconds * 100f) % 100f);
        
        return $"{minutes:00}:{seconds:00}.{milliseconds:00}";
    }
    
    /// <summary>
    /// Show the lap timer panel
    /// </summary>
    private void ShowLapTimer()
    {
        if (lapTimerPanel != null)
        {
            lapTimerPanel.RemoveFromClassList("hidden");
        }
    }
    
    /// <summary>
    /// Hide the lap timer panel
    /// </summary>
    private void HideLapTimer()
    {
        if (lapTimerPanel != null)
        {
            lapTimerPanel.AddToClassList("hidden");
        }
    }
    
    /// <summary>
    /// Updates the cash display with current amount and goal
    /// </summary>
    private void UpdateCashDisplay()
    {
        if (cashCurrentLabel == null || cashGoalLabel == null) return;
        
        cashCurrentLabel.text = $"${currentCash:N0}";
        cashGoalLabel.text = $"/ ${cashGoal:N0} Goal";
    }
    
    // === PUBLIC METHODS ===
    
    public void AddCash(float amount)
    {
        currentCash += amount;
        UpdateCashDisplay();
        Debug.Log($"Cash added: ${amount:N0}. Total: ${currentCash:N0}");
    }
    
    public bool SpendCash(float amount)
    {
        if (currentCash >= amount)
        {
            currentCash -= amount;
            UpdateCashDisplay();
            Debug.Log($"Cash spent: ${amount:N0}. Remaining: ${currentCash:N0}");
            return true;
        }
        else
        {
            Debug.LogWarning($"Not enough cash! Have: ${currentCash:N0}, Need: ${amount:N0}");
            return false;
        }
    }
    
    public void SetCashGoal(float newGoal)
    {
        cashGoal = newGoal;
        UpdateCashDisplay();
    }
    
    public float GetCurrentCash()
    {
        return currentCash;
    }
    
    public bool HasMetGoal()
    {
        return currentCash >= cashGoal;
    }
    
    public void SetCash(float amount)
    {
        currentCash = amount;
        UpdateCashDisplay();
        Debug.Log($"Cash set to: ${currentCash:N0}");
    }
    
    // === INTERACTION PROMPTS ===
    
    public void ShowEnterPrompt()
    {
        if (interactionPrompt == null || interactionText == null) return;
        
        interactionText.text = "Press E to Enter Vehicle";
        interactionPrompt.RemoveFromClassList("hidden");
    }

    public void ShowExitPrompt()
    {
        if (interactionPrompt == null || interactionText == null) return;
        
        interactionText.text = "Press E to Exit Vehicle";
        interactionPrompt.RemoveFromClassList("hidden");
    }

    public void HideInteractionPrompt()
    {
        if (interactionPrompt == null) return;
        
        interactionPrompt.AddToClassList("hidden");
    }
    
    /// <summary>
    /// Show "Press E to Talk to [NPC Name]" prompt (NEW for Sprint 4)
    /// </summary>
    public void ShowTalkPrompt(string npcName)
    {
        if (interactionPrompt == null || interactionText == null) return;
        
        interactionText.text = $"Press E to Talk to {npcName}";
        interactionPrompt.RemoveFromClassList("hidden");
    }
}

