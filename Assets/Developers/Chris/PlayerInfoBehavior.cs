using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoBehavior : Singleton<PlayerInfoBehavior>
{
    new public static PlayerInfoBehavior Instance
    {
        get
        {
            if (applicationIsQuitting)
                return null;

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (PlayerInfoBehavior)FindObjectOfType(typeof(PlayerInfoBehavior));


                    if (_instance == null)
                    {
                        GameObject outGO = Instantiate(Resources.Load<GameObject>("Managers/PlayerInfoBehavior"));
                        _instance = outGO.GetComponent<PlayerInfoBehavior>();

                        DontDestroyOnLoad(_instance);
                    }
                    else
                        DontDestroyOnLoad(_instance);
                }

                return _instance;
            }
        }
    }

    public GameObject CastlePosition;

    public int m_playerLayer;

    public int m_CurrentPlayerResources { get; private set; }

    private float m_Timer = 0f;

    [SerializeField] private GameCollection m_desideredPlayerInfo;

    private GameCollection m_PlayerInfoSO;
    public GameCollection PlayerInfoSO
    {
        get
        {
            if (m_PlayerInfoSO != null) return m_PlayerInfoSO;
            m_PlayerInfoSO = Instantiate(m_PlayerInfoSO);
            return m_PlayerInfoSO;
        }
        set
        {
            m_PlayerInfoSO = value;
        }
    }

    private void Awake()
    {
        PlayerInfoSO = Instantiate(m_desideredPlayerInfo);
    }
    // Start is called before the first frame update
    void Start()
    {
        StartGame();
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
        m_playerLayer = GetGameObjectLayer(PlayerInfoSO.PlayerLayer);
        m_CurrentPlayerResources = PlayerInfoSO.PlayerQuantityResources;
    }


    public bool CheckResourcesAvailability(int ResourcesToCheck)
    {
        if (ResourcesToCheck <= m_CurrentPlayerResources)
        {
            return true;
        }
        return false;
    }

    public void DecreaseResources(int QuantityOfResources)
    {
        m_CurrentPlayerResources -= QuantityOfResources;
    }

    public void AddResources(int QuantityOfResources)
    {
        m_CurrentPlayerResources += QuantityOfResources;
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