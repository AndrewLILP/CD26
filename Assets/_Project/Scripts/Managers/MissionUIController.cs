using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Controls the Mission UI panel display
/// Works alongside HUDController on the same GameObject
/// </summary>
public class MissionUIController : MonoBehaviour
{
    private UIDocument uiDocument;
    
    // UI Elements
    private VisualElement missionPanel;
    private Label missionName;
    private Label missionDescription;
    private Label missionReward;
    private Button acceptButton;
    
    private MissionData currentDisplayedMission;
    
    void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        
        if (uiDocument == null)
        {
            Debug.LogError("MissionUIController: No UIDocument found!");
            return;
        }
        
        var root = uiDocument.rootVisualElement;
        
        // Cache references
        missionPanel = root.Q<VisualElement>("mission-panel");
        missionName = root.Q<Label>("mission-name");
        missionDescription = root.Q<Label>("mission-description");
        missionReward = root.Q<Label>("mission-reward");
        acceptButton = root.Q<Button>("mission-accept-button");
        
        // Validate
        if (missionPanel == null) Debug.LogError("MissionUIController: 'mission-panel' not found!");
        if (missionName == null) Debug.LogError("MissionUIController: 'mission-name' not found!");
        if (missionDescription == null) Debug.LogError("MissionUIController: 'mission-description' not found!");
        if (missionReward == null) Debug.LogError("MissionUIController: 'mission-reward' not found!");
        if (acceptButton == null) Debug.LogError("MissionUIController: 'mission-accept-button' not found!");
        
        // Register callbacks
        if (acceptButton != null)
        {
            acceptButton.clicked += OnAcceptButtonClicked;
        }
        
        // Subscribe to MissionManager events
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.OnMissionUnlocked.AddListener(OnMissionUnlocked);
        }
        
        // Start hidden
        HideMissionPanel();
        
        Debug.Log("[MissionUI] Initialized");
    }
    
    void Update()
    {
        // Accept with Enter key when panel is visible
        if (missionPanel != null && !missionPanel.ClassListContains("hidden"))
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                OnAcceptButtonClicked();
            }
        }
    }
    
    void OnDestroy()
    {
        if (acceptButton != null)
        {
            acceptButton.clicked -= OnAcceptButtonClicked;
        }
        
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.OnMissionUnlocked.RemoveListener(OnMissionUnlocked);
        }
    }
    
    private void OnMissionUnlocked(MissionData mission)
    {
        // Auto-show first available mission
        //if (MissionManager.Instance.GetCurrentMission() == null)
        //{
          //  ShowMissionBriefing(mission);
        //}
        Debug.Log($"[MissionUI] Mission unlocked: {mission.missionName}");
    }
    
    public void ShowMissionBriefing(MissionData mission)
    {
        if (mission == null || missionPanel == null) return;
        
        currentDisplayedMission = mission;
        
        // Populate UI
        missionName.text = mission.missionName.ToUpper();
        missionDescription.text = mission.missionDescription;
        missionReward.text = $"${mission.cashReward:N0}";
        
        // Show panel
        missionPanel.RemoveFromClassList("hidden");
        
        // Pause game
        Time.timeScale = 0f;
        
        Debug.Log($"[MissionUI] Showing briefing: {mission.missionName}");
    }
    
    public void HideMissionPanel()
    {
        if (missionPanel == null) return;
        
        missionPanel.AddToClassList("hidden");
        
        // Resume game
        Time.timeScale = 1f;
    }
    
    private void OnAcceptButtonClicked()
    {
        if (currentDisplayedMission == null || MissionManager.Instance == null)
            return;
        
        // Start the mission
        MissionManager.Instance.StartMission(currentDisplayedMission.missionID);
        
        // Hide panel
        HideMissionPanel();
        
        Debug.Log($"[MissionUI] Player accepted: {currentDisplayedMission.missionName}");
    }
}