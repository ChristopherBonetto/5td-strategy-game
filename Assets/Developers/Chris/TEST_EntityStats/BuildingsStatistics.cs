using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "NewBuilding", fileName = "Building")]
public class BuildingsStatistics : ScriptableObject
{
    public new string BuildingName = "";
    public Buildings BuildingType;
    public int HealthMax = 1;
    public int Defence = 1;
    public int BuildingAttack;
    public float BuildingTimeToAttack;
    public int BuildingViewRadius;
}
