using UnityEngine;

[CreateAssetMenu(fileName = "MissionData", menuName = "Mech/Mission")]
public class MechMissionData : ScriptableObject
{
    public string missionName;
    public string description;

    public string sceneName;

    public int reward;
}