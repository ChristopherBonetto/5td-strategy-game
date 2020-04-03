using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilizationScriptBehaviour : MonoBehaviour
{
    public static CivilizationScriptBehaviour Instance;
 

    public int m_ShiftedCivilizationLayer { get; private set; }

    public QuantityOfResources[] m_CurrentCivilizationResources { get; private set; }

    private Vector3 StartingCityHallPosition;

    public int CurrentCivilizationMaxPopulation;

    public bool DepositInstantiated = false;
    private float m_Timer = 0f;
    

    private CivilizationStatistics m_CivilizationSO;
    public CivilizationStatistics CivilizationSO
    {
        get
        {
            if (m_CivilizationSO != null) return m_CivilizationSO;
            m_CivilizationSO = Instantiate(m_CivilizationSO);
            return m_CivilizationSO;
        }
        set
        {
            m_CivilizationSO = value;
        }
    }

    public List<GameObject> CreatedUnits = new List<GameObject>();
    public List<GameObject> CreatedBuildings = new List<GameObject>();

    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartGame();
    }
    private void Update()
    {
        
    }



    public int GetGameObjectLayer(LayerMask mask)
    {
        for (int i = 0; i < 32; ++i)
        {
            if (((1 << i) & mask.value) > 0)
            {
                return i;
            }
        }

        return -1;
    }

    private void StartGame()
    {
        m_ShiftedCivilizationLayer = GetGameObjectLayer(CivilizationSO.CivilizationLayer);

        m_CurrentCivilizationResources = new QuantityOfResources[CivilizationSO.ResourcesValuesDictionary.Count];
        CivilizationSO.ResourcesValuesDictionary.Values.CopyTo(m_CurrentCivilizationResources, 0);

        CurrentCivilizationMaxPopulation = CivilizationSO.MaxPopulation;

        StartingCityHallPosition = new Vector3(0, CivilizationSO.BuildingsDictionary[Buildings.CityHall].BuildingPrefab.transform.lossyScale.y / 2, 0);
    }


    public bool CheckResourcesAvailability(QuantityOfResources[] ResourcesToCheck)
    {
        int CivilizationHaveThatResource = 0;

        for (int i = 0; i < ResourcesToCheck.Length; i++)
        {
            for (int j = 0; j < m_CurrentCivilizationResources.Length; j++)
            {
                if (ResourcesToCheck[i].ResourceType == m_CurrentCivilizationResources[j].ResourceType)
                {
                    if(m_CurrentCivilizationResources[j].ResourceQuantity >= ResourcesToCheck[i].ResourceQuantity)
                    {
                        CivilizationHaveThatResource++;
                    }
                }
            }
        }
        if(CivilizationHaveThatResource >= ResourcesToCheck.Length)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DecreaseResources(QuantityOfResources[] QuantityOfResources)
    {
        for (int i = 0; i < QuantityOfResources.Length; i++)
        {
            for (int j = 0; j < m_CurrentCivilizationResources.Length; j++)
            {
                if (QuantityOfResources[i].ResourceType == m_CurrentCivilizationResources[j].ResourceType)
                {
                    m_CurrentCivilizationResources[j].ResourceQuantity -= QuantityOfResources[i].ResourceQuantity;
                }
            }
        }
    }

    public void AddResources(QuantityOfResources[] QuantityOfResources)
    {
        for (int i = 0; i < QuantityOfResources.Length; i++)
        {
            for(int j = 0; j < m_CurrentCivilizationResources.Length; j++)
            {
                if(QuantityOfResources[i].ResourceType == m_CurrentCivilizationResources[j].ResourceType)
                {
                    m_CurrentCivilizationResources[j].ResourceQuantity += QuantityOfResources[i].ResourceQuantity;
                    QuantityOfResources[i].ResourceQuantity = 0;
                }
            }
        }
    }

    //public void InstantiateBuilding(Buildings BuildingType, Vector3 SpawnBuilding, Quaternion SpawnQuaternion)
    //{
    //    if (CivilizationSO.BuildingsDictionary != null)
    //    {
    //        GameObject Building = Instantiate(CivilizationSO.BuildingsDictionary[BuildingType].BuildingPrefab, SpawnBuilding, SpawnQuaternion) as GameObject;
    //        Building.transform.name = CivilizationSO.BuildingsDictionary[BuildingType].BuildingName;
    //        Building.layer = GetGameObjectLayer(CivilizationSO.CivilizationLayer);

    //        switch (BuildingType)
    //        {
    //            case Buildings.CityHall:
    //                Building.GetComponent<CityAllActions>().BuildingStatisticsSO = CivilizationSO.BuildingsDictionary[BuildingType].BuildingStatsCopy;
    //                CivilizationSO.DepositsList.Add(Building);
    //                DepositInstantiated = true;
    //                break;

    //            case Buildings.House:
    //                Building.GetComponent<HouseActions>().BuildingStatisticsSO = CivilizationSO.BuildingsDictionary[BuildingType].BuildingStatsCopy;
    //                break;

    //            case Buildings.MineralDeposit:
    //                Building.GetComponent<MineralDepositActions>().BuildingStatisticsSO = CivilizationSO.BuildingsDictionary[BuildingType].BuildingStatsCopy;
    //                CivilizationSO.DepositsList.Add(Building);
    //                DepositInstantiated = true;
    //                break;

    //            case Buildings.Carpentry:
    //                Building.GetComponent<CarpentryActions>().BuildingStatisticsSO = CivilizationSO.BuildingsDictionary[BuildingType].BuildingStatsCopy;
    //                CivilizationSO.DepositsList.Add(Building);
    //                DepositInstantiated = true;
    //                break;

    //            case Buildings.Plantation:
    //                Building.GetComponent<PlantationActions>().BuildingStatisticsSO = CivilizationSO.BuildingsDictionary[BuildingType].BuildingStatsCopy;
    //                CivilizationSO.DepositsList.Add(Building);
    //                DepositInstantiated = true;
    //                break;

    //            case Buildings.Port:
    //                Building.GetComponent<PortActions>().BuildingStatisticsSO = CivilizationSO.BuildingsDictionary[BuildingType].BuildingStatsCopy;
    //                break;

    //            case Buildings.Barrack:
    //                Building.GetComponent<BarrackActions>().BuildingStatisticsSO = CivilizationSO.BuildingsDictionary[BuildingType].BuildingStatsCopy;
    //                break;

    //            case Buildings.Archery:
    //                Building.GetComponent<ArcheryActions>().BuildingStatisticsSO = CivilizationSO.BuildingsDictionary[BuildingType].BuildingStatsCopy;
    //                break;

    //            case Buildings.Stable:
    //                Building.GetComponent<StableActions>().BuildingStatisticsSO = CivilizationSO.BuildingsDictionary[BuildingType].BuildingStatsCopy;
    //                break;

    //            default:
    //                break;
    //        }
    //    }
    //}


    public bool Timer(float destinationTime)
    {
        m_Timer += Time.deltaTime;
        if (m_Timer >= destinationTime)
        {
            m_Timer = 0f;

            return true;
        }
        else
        {
            return false;
        }
    }
}
