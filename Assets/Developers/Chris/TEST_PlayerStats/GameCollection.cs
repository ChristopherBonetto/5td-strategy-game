using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Flags]
public enum UnitQualities
{
    None = 0,
    Melee = 1 << 0,
    Ranged = 1 << 1,
    Infantry = 1 << 2,
    Cavalry = 1 << 3,
}

public enum Units
{
    Farmer,
    Defender,
    Lifter,
    Runner,
}

public enum Buildings
{
    Castle,
    Tower
}

public enum Materials
{
    Gems,
}


[System.Serializable]
public struct UnitInfo
{
    [SerializeField] private UnitStatistics m_OriginalUnitStats;
    public UnitStatistics OriginalUnitStats { get { return m_OriginalUnitStats; } }

    private UnitStatistics m_UnitStatsCopy;
    public UnitStatistics UnitStatsCopy
    {
        get
        {
            return m_UnitStatsCopy;
        }
        set
        {
            m_UnitStatsCopy = value;
        }
    }
}


[System.Serializable]
public struct BuildingInfo
{
    [SerializeField] private BuildingsStatistics m_OriginalBuildingStats;
    public BuildingsStatistics OriginalBuildingStats { get { return m_OriginalBuildingStats; } }

    private BuildingsStatistics m_BuildingStatsCopy;
    public BuildingsStatistics BuildingStatsCopy
    {
        get
        {
            return m_BuildingStatsCopy;
        }
        set
        {
            m_BuildingStatsCopy = value;
        }
    }

    public GameObject BuildingPrefab;
}

[CreateAssetMenu (menuName = "NewGameCollection", fileName = "GameCollection")]
public class GameCollection : ScriptableObject
{
    #region Player

    [SerializeField] private LayerMask m_playerLayer;
    public LayerMask PlayerLayer
    {
        get
        {
            return m_playerLayer;
        }
    }

    [SerializeField] private int m_PlayerQuantityResources;
    public int PlayerQuantityResources { get { return m_PlayerQuantityResources; } private set { } }

    [Space, Header("Player")]
    [SerializeField] private UnitInfo[] m_PlayerUnits;

    
    [Space, SerializeField] private BuildingInfo[] m_PlayerBuildings;

    public Dictionary<Units, UnitInfo> PlayerUnitsDictionary { get; private set; }
    public Dictionary<Buildings, BuildingInfo> PlayerBuildingsDictionary { get; private set; }

    #endregion

    #region AI

    [Space, Header("AI")]
    [SerializeField] private UnitInfo[] m_AIUnits;

    [Space, SerializeField] private BuildingInfo[] m_AIBuildings;

    public Dictionary<Units, UnitInfo> AIUnitsDictionary { get; private set; }
    public Dictionary<Buildings, BuildingInfo> AIBuildingsDictionary { get; private set; }

    #endregion

    private void Awake()
    {

        #region Player

        PlayerUnitsDictionary = new Dictionary<Units, UnitInfo>();
        PlayerBuildingsDictionary = new Dictionary<Buildings, BuildingInfo>();

        for (int i = 0; i < m_PlayerUnits.Length; i++)
        {
            if (!PlayerUnitsDictionary.ContainsKey(m_PlayerUnits[i].OriginalUnitStats.UnitType))
            {
                m_PlayerUnits[i].UnitStatsCopy = Instantiate(m_PlayerUnits[i].OriginalUnitStats);
                PlayerUnitsDictionary.Add(m_PlayerUnits[i].UnitStatsCopy.UnitType, m_PlayerUnits[i]);
            }
            else
            {
                Debug.Log(m_PlayerUnits[i].OriginalUnitStats.UnitType + " can't be added because there is another key with same value");
            }
        }

        for (int i = 0; i < m_PlayerBuildings.Length; i++)
        {
            if (!PlayerBuildingsDictionary.ContainsKey(m_PlayerBuildings[i].OriginalBuildingStats.BuildingType))
            {
                m_PlayerBuildings[i].BuildingStatsCopy = Instantiate(m_PlayerBuildings[i].OriginalBuildingStats);
                PlayerBuildingsDictionary.Add(m_PlayerBuildings[i].BuildingStatsCopy.BuildingType, m_PlayerBuildings[i]);
            }
            else
            {
                Debug.Log(m_PlayerBuildings[i].OriginalBuildingStats.BuildingType + " can't be added because there is another key with same value");
            }
        }

        #endregion

        #region AI

        AIUnitsDictionary = new Dictionary<Units, UnitInfo>();
        AIBuildingsDictionary = new Dictionary<Buildings, BuildingInfo>();

        for (int i = 0; i < m_AIUnits.Length; i++)
        {
            if (!AIUnitsDictionary.ContainsKey(m_AIUnits[i].OriginalUnitStats.UnitType))
            {
                m_AIUnits[i].UnitStatsCopy = Instantiate(m_AIUnits[i].OriginalUnitStats);
                AIUnitsDictionary.Add(m_AIUnits[i].UnitStatsCopy.UnitType, m_AIUnits[i]);
            }
            else
            {
                Debug.Log(m_AIUnits[i].OriginalUnitStats.UnitType + " can't be added because there is another key with same value");
            }
        }

        for (int i = 0; i < m_AIBuildings.Length; i++)
        {
            if (!AIBuildingsDictionary.ContainsKey(m_AIBuildings[i].OriginalBuildingStats.BuildingType))
            {
                m_AIBuildings[i].BuildingStatsCopy = Instantiate(m_AIBuildings[i].OriginalBuildingStats);
                AIBuildingsDictionary.Add(m_AIBuildings[i].BuildingStatsCopy.BuildingType, m_AIBuildings[i]);
            }
            else
            {
                Debug.Log(m_AIBuildings[i].OriginalBuildingStats.BuildingType + " can't be added because there is another key with same value");
            }
        }

        #endregion
    }
}
