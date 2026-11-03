using UnityEngine;

[CreateAssetMenu(fileName = "MissionData", menuName = "Mech/Mission")]
public class MechMissionData : ScriptableObject
{
    public string missionName;
    [TextArea(2, 5)]
    public string description;

    public string sceneName;

    public int reward;
}