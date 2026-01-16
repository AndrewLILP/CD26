using UnityEngine;

/// <summary>
/// ScriptableObject that stores mission data
/// Create via: Assets > Create > CD26 > Mission Data
/// </summary>
[CreateAssetMenu(fileName = "NewMission", menuName = "CD26/Mission Data", order = 1)]
public class MissionData : ScriptableObject
{
    [Header("Mission Info")]
    [Tooltip("Unique identifier for this mission")]
    public string missionID = "mission_01";
    
    [Tooltip("Display name shown in UI")]
    public string missionName = "First Lap";
    
    [TextArea(3, 6)]
    [Tooltip("Mission description/objective")]
    public string missionDescription = "Complete one lap around the track to prove you can drive.";
    
    [Header("Rewards")]
    [Tooltip("Cash reward for completing mission")]
    public float cashReward = 500f;
    
    [Header("Prerequisites")]
    [Tooltip("Mission IDs that must be completed before this unlocks")]
    public string[] requiredMissions = new string[0];
    
    [Tooltip("Is this mission available from the start?")]
    public bool availableAtStart = true;
    
    [Header("Mission Type")]
    public MissionType type = MissionType.Driving;
    
    public enum MissionType
    {
        Driving,    // Lap completion, racing
        Dialogue,   // Talk to NPCs
        Delivery,   // Pick up/drop off (future)
        Job         // Career mini-games (future)
    }
}