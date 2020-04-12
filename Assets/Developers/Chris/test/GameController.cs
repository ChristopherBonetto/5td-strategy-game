using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerType
{
    Player,
    AI
}


public class GameController : Singleton<GameController>
{
    new public static GameController Instance
    {
        get
        {
            if (applicationIsQuitting)
                return null;

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (GameController)FindObjectOfType(typeof(GameController));


                    if (_instance == null)
                    {
                        GameObject outGO = Instantiate(Resources.Load<GameObject>("Managers/GameController"));
                        _instance = outGO.GetComponent<GameController>();

                        DontDestroyOnLoad(_instance);
                    }
                    else
                        DontDestroyOnLoad(_instance);
                }

                return _instance;
            }
        }
    }


    public GameObject PlayerCastle;

    public int m_playerLayer { get; private set; }
    public int m_aiLayer { get; private set; }

    public int m_CurrentPlayerResources { get; private set; }

    private float m_Timer = 0f;

    [SerializeField] private GameCollection m_gameCollection;

    private GameCollection m_gameCollectionCopy;
    public GameCollection GameCollectionCopy
    {
        get
        {
            return m_gameCollectionCopy;
        }
        set
        {
            m_gameCollectionCopy = value;
        }
    }

    private void Awake()
    {
        GameCollectionCopy = Instantiate(m_gameCollection);
    }
    // Start is called before the first frame update
    void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CreateNewEntity(EntityType.Defender, PlayerType.Player, Vector3.zero);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            CreateNewEntity(EntityType.Farmer, PlayerType.AI, Vector3.zero);
        }
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
        m_playerLayer = GetGameObjectLayer(GameCollectionCopy.PlayerLayer);
        m_aiLayer = GetGameObjectLayer(GameCollectionCopy.AILayer);

        m_CurrentPlayerResources = GameCollectionCopy.PlayerQuantityResources;
    }

    public void CreateNewEntity(EntityType inEntityType, PlayerType inPlayerType, Vector3 inPosition)
    {
        if (!GameCollectionCopy.GameEntitiesDictionary.ContainsKey(inEntityType))
        {
            Debug.Log("GameCollection don't contain " + inEntityType);
            return;
        }

        GameObject tempEntity = Instantiate(GameCollectionCopy.GameEntitiesDictionary[inEntityType].EntityPrefab, inPosition, Quaternion.identity);
        Entity tempRef = tempEntity.GetComponent<Entity>();

        if(tempRef != null)
        {
            tempRef.AssignStats(GameCollectionCopy.GameEntitiesDictionary[inEntityType]);
        }

        tempRef.AssignPlayer(inPlayerType);
    }


        #region Resources

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

    #endregion

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