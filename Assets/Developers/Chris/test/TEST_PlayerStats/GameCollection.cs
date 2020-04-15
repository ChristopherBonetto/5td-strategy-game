using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;


[System.Serializable]
public struct QuantityOfResources
{
    public ResourceType ResourceType;
    public int ResourceQuantity;
}

[System.Serializable]
public struct UnitInfo
{
    [SerializeField] private UnitsSO m_OriginalUnitStats;
    public UnitsSO OriginalUnitStats { get { return m_OriginalUnitStats; } private set { } }

    private UnitsSO m_UnitStatsCopy;
    public UnitsSO UnitStatsCopy
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
    public string BuildingName;

    [SerializeField] private BuildingsSO m_OriginalBuildingStats;
    public BuildingsSO OriginalBuildingStats { get { return m_OriginalBuildingStats; } }

    private BuildingsSO m_BuildingStatsCopy;
    public BuildingsSO BuildingStatsCopy
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

    [SerializeField] private QuantityOfResources[] m_PlayerStartingResources;
    public QuantityOfResources[] PayerStartingResources { get { return m_PlayerStartingResources; } private set { } }


    [Space, Header("Civilitazion's Units")]
    [SerializeField] private UnitInfo[] m_GameUnits;

    [Space, Header("Civilitazion's Buildings")]
    [SerializeField] private BuildingInfo[] m_GameBuildings;

    public Dictionary<UnitType, UnitInfo> UnitsDictionary { get; private set; }
    public Dictionary<BuildingType, BuildingInfo> BuildingsDictionary { get; private set; }
    public Dictionary<ResourceType, QuantityOfResources> ResourcesValuesDictionary { get; private set; }


    private void Awake()
    {

        UnitsDictionary = new Dictionary<UnitType, UnitInfo>();
        BuildingsDictionary = new Dictionary<BuildingType, BuildingInfo>();
        ResourcesValuesDictionary = new Dictionary<ResourceType, QuantityOfResources>();

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

        for (int i = 0; i < m_PlayerStartingResources.Length; i++)
        {
            if (!ResourcesValuesDictionary.ContainsKey(m_PlayerStartingResources[i].ResourceType))
            {
                ResourcesValuesDictionary.Add(m_PlayerStartingResources[i].ResourceType, m_PlayerStartingResources[i]);
            }
            else
            {
                Debug.Log(m_PlayerStartingResources[i].ResourceType + " can't be added because there is another key with same value");
            }
        }

    }
}
