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
    Worker,
    Soldier,
    Lancer,
    ArcherMedium,
    ArcherLong,
    Knight,
    BowKnight,
}

public enum Buildings
{
    CityHall,
    House,
    Carpentry,
    MineralDeposit,
    Plantation,
    Port,
    Barrack,
    Stable,
    Archery,
    Tower
}

public enum Materials
{
    Food,
    Stone,
    Wood,
    Gold
}

[System.Serializable]
public struct QuantityOfResources
{
    public Materials ResourceType;
    public int ResourceQuantity;
}


[System.Serializable]
public struct UnitInfo
{
    public string UnitName;
    
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
    
    public GameObject UnitPrefab;
    public QuantityOfResources[] UnitCost;
}


[System.Serializable]
public struct BuildingInfo
{
    public string BuildingName;

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
    public QuantityOfResources[] BuildingCost;
}

[CreateAssetMenu (menuName = "NewContainerCivilizationInfo", fileName = "CivilizationInfo")]
public class CivilizationStatistics : ScriptableObject
{
    [Header("Civilitazion's Info")]
    [SerializeField] private string m_CivilizationName;
    private int m_NumberOfPopulation = 0;
    public int NumberOfPopulation
    {
        get
        {
            return m_NumberOfPopulation;
        }
        set
        {
            m_NumberOfPopulation = value;
        }
    }
    [SerializeField, Range(1, 10)] public int MaxPopulation = 1;
    [SerializeField] private LayerMask m_CivilizationLayer;
    public LayerMask CivilizationLayer
    {
        get
        {
            return m_CivilizationLayer;
        }
    }

    [SerializeField] private Mesh m_BuildingFoundationsCivilizationMesh;
    public Mesh BuildingFoundationsCivilizationMesh
    {
        get
        {
            return m_BuildingFoundationsCivilizationMesh;
        }
        private set
        {
            m_BuildingFoundationsCivilizationMesh = value;
        }
    }


    public List<GameObject> DepositsList = new List<GameObject>();

    [SerializeField] private QuantityOfResources[] m_CivilizationQuantityResources;
    public QuantityOfResources[] CivilizationQuantityResources { get { return m_CivilizationQuantityResources; } private set { } }

    [Space, Header("Civilitazion's Units")]
    [SerializeField] private UnitInfo[] m_CivilizationUnits;

    [Space, Header("Civilitazion's Buildings")]
    [SerializeField] private BuildingInfo[] m_CivilizationBuildings;

    public Dictionary<Units, UnitInfo> UnitsDictionary { get; private set; }
    public Dictionary<Buildings, BuildingInfo> BuildingsDictionary { get; private set; }
    public Dictionary<Materials, QuantityOfResources> ResourcesValuesDictionary { get; private set; }

    private void Awake()
    {
        UnitsDictionary = new Dictionary<Units, UnitInfo>();
        BuildingsDictionary = new Dictionary<Buildings, BuildingInfo>();
        ResourcesValuesDictionary = new Dictionary<Materials, QuantityOfResources>();

        for (int i = 0; i < m_CivilizationUnits.Length; i++)
        {
            if (!UnitsDictionary.ContainsKey(m_CivilizationUnits[i].OriginalUnitStats.UnitType))
            {
                m_CivilizationUnits[i].UnitStatsCopy = Instantiate(m_CivilizationUnits[i].OriginalUnitStats);
                UnitsDictionary.Add(m_CivilizationUnits[i].UnitStatsCopy.UnitType, m_CivilizationUnits[i]);
            }
            else
            {
                Debug.Log(m_CivilizationUnits[i].OriginalUnitStats.UnitType + " can't be added because there is another key with same value");
            }
        }

        for (int i = 0; i < m_CivilizationBuildings.Length; i++)
        {
            if (!BuildingsDictionary.ContainsKey(m_CivilizationBuildings[i].OriginalBuildingStats.BuildingType))
            {
                m_CivilizationBuildings[i].BuildingStatsCopy = Instantiate(m_CivilizationBuildings[i].OriginalBuildingStats);
                BuildingsDictionary.Add(m_CivilizationBuildings[i].BuildingStatsCopy.BuildingType, m_CivilizationBuildings[i]);
            }
            else
            {
                Debug.Log(m_CivilizationBuildings[i].OriginalBuildingStats.BuildingType + " can't be added because there is another key with same value");
            }
        }

        for (int i = 0; i < m_CivilizationQuantityResources.Length; i++)
        {
            if(!ResourcesValuesDictionary.ContainsKey(m_CivilizationQuantityResources[i].ResourceType))
            {
                ResourcesValuesDictionary.Add(m_CivilizationQuantityResources[i].ResourceType, m_CivilizationQuantityResources[i]);
            }
            else
            {
                Debug.Log(m_CivilizationQuantityResources[i].ResourceType + " can't be added because there is another key with same value");
            }
        }
    }

    
}
