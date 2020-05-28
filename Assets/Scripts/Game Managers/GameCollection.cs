using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;


[System.Serializable]
public struct UnitInfo
{
    [SerializeField] private UnitsStatsSO m_OriginalUnitStats;
    public UnitsStatsSO OriginalUnitStats { get { return m_OriginalUnitStats; } private set { } }

    private UnitsStatsSO m_UnitStatsCopy;
    public UnitsStatsSO UnitStatsCopy
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
    [SerializeField] private int m_unitPoolQuantity;
    public int UnitPoolQuantity { get { return m_unitPoolQuantity; } private set { } }
}

[System.Serializable]
public struct BuildingInfo
{
    [SerializeField] private BuildingsStatsSO m_OriginalBuildingStats;
    public BuildingsStatsSO OriginalBuildingStats { get { return m_OriginalBuildingStats; } }

    private BuildingsStatsSO m_BuildingStatsCopy;
    public BuildingsStatsSO BuildingStatsCopy
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
    [SerializeField] private int m_buildingPoolQuantity;
    public int BuildingPoolQuantity { get { return m_buildingPoolQuantity; } private set { } }
}


[CreateAssetMenu(menuName = "NewGameCollection", fileName = "GameCollection")]
public class GameCollection : ScriptableObject
{

    [SerializeField] private LayerMask m_playerLayer;
    public LayerMask PlayerLayer
    {
        get
        {
            return m_playerLayer;
        }
    }

    [SerializeField] private LayerMask m_aiLayer;
    public LayerMask AILayer
    {
        get
        {
            return m_aiLayer;
        }
    }

    [Space, Header("Civilitazion's Units")]
    [SerializeField] private UnitInfo[] m_GameUnits;

    [Space, Header("Civilitazion's Buildings")]
    [SerializeField] private BuildingInfo[] m_GameBuildings;

    public Dictionary<UnitType, UnitInfo> UnitsDictionary { get; private set; }
    public Dictionary<BuildingType, BuildingInfo> BuildingsDictionary { get; private set; }

    private void Awake()
    {

        UnitsDictionary = new Dictionary<UnitType, UnitInfo>();
        BuildingsDictionary = new Dictionary<BuildingType, BuildingInfo>();

        for (int i = 0; i < m_GameUnits.Length; i++)
        {
            if (!UnitsDictionary.ContainsKey(m_GameUnits[i].OriginalUnitStats.UnitType))
            {
                m_GameUnits[i].UnitStatsCopy = Instantiate(m_GameUnits[i].OriginalUnitStats);
                UnitsDictionary.Add(m_GameUnits[i].UnitStatsCopy.UnitType, m_GameUnits[i]);
            }
            else
            {
                Debug.Log(m_GameUnits[i].OriginalUnitStats.UnitType + " can't be added because there is another key with same value");
            }
        }

        for (int i = 0; i < m_GameBuildings.Length; i++)
        {
            if (!BuildingsDictionary.ContainsKey(m_GameBuildings[i].OriginalBuildingStats.BuildingType))
            {
                m_GameBuildings[i].BuildingStatsCopy = Instantiate(m_GameBuildings[i].OriginalBuildingStats);
                BuildingsDictionary.Add(m_GameBuildings[i].BuildingStatsCopy.BuildingType, m_GameBuildings[i]);
            }
            else
            {
                Debug.Log(m_GameBuildings[i].OriginalBuildingStats.BuildingType + " can't be added because there is another key with same value");
            }
        }

    }
}
