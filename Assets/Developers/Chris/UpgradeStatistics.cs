using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "NewUpgrade", fileName = "Upgrade")]
public class UpgradeStatistics : ScriptableObject
{
    [Header("UnitsToUpgrade")] public UnitUpgrade[] UnitsToUpgrade;

    [Header("BuildingToUpgrade"),Space] public BuildingUpgrade[] BuildingsToUpgrade;

    [Header("WorkerUpgrade"), Space] public WorkerUpgrade WorkerUpgrade;

    [Space]public float TimeToUpgrade;
    [Header("UpgradeCost"),Space] public QuantityOfResources[] UpgradeCost;
}

[System.Serializable]
public struct UnitUpgrade
{
    public Units TypeOfUnitToUpgrade;
    public float DecreaseTimeToInstantiate;
    public int IncreaseHealthMax;
    public int IncreaseAttack;
    public int IncreaseDefence;
    public float DecreaseTimeToAttack;
    public float IncreaseMovementSpeed;
    public int IncreaseViewRadius;

    [Space] public float IncreaseBulletSpeed;
}

[System.Serializable]
public struct BuildingUpgrade
{
    public Buildings TypeOfBuildingToUpgrade;
    public float DecreaseTimeToBuild;
    public int IncreaseHealthMax;
    public int IncreaseAttack;
    public int IncreaseDefence;
    public float DecreaseTimeToAttack;
    public int IncreaseViewRadius;
}

[System.Serializable]
public struct WorkerUpgrade
{
    public bool CanUpgradeWorker;
    public int IncreaseBagToStoreResources;
    public int IncreaseSpeedToCollect;
    public int IncreaseResourcesCollected;
    public int IncreaseRepairValue;
}



