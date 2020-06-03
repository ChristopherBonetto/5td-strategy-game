using UnityEngine;
using Types;
using UnityEngine.AI;

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

    public int m_playerLayer { get; private set; }
    public int m_aiLayer { get; private set; }

    [SerializeField] private GameCollection m_gameCollection;

    private GameCollection m_collection;
    public GameCollection Collection
    {
        get
        {
            return m_collection;
        }
        set
        {
            m_collection = value;
        }
    }
    public int m_currentPlayerResources;

    #region Behavior Cycle

    private void Awake()
    {
        Collection = Instantiate(m_gameCollection);
    }
    void Start()
    {
        Initialize();
    }
   

    #endregion

    #region Generic method

    private void Initialize()
    {
        m_playerLayer = GetGameObjectLayer(Collection.PlayerLayer);
        m_aiLayer = GetGameObjectLayer(Collection.AILayer);
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

    #endregion

    #region Find and create new entities

    #region Unit methods

    public Troop CreateNewTroop(UnitType inUnitType, PlayerType inPlayerType, Vector3 inPosition, bool inGratis)
    {
        if (!Collection.UnitsDictionary.ContainsKey(inUnitType))
        {
            Debug.Log("GameCollection don't contain " + inUnitType);
            return null;
        }

        Vector3? closestPoint = RandomPoint(inPosition, 1);

        if (closestPoint != null)
        {
            inPosition = closestPoint.Value;
        }
        else
        {
            Debug.Log("Not finded a point");
            return null;
        }
        

        if (!inGratis)
        {
            int cost = Collection.UnitsDictionary[inUnitType].UnitStatsCopy.Cost;

            if (!CheckResourcesAvailability(cost))
            {
                Debug.LogWarning("U don't have resources to create " + inUnitType);
                return null;
            }
            AddResources(-cost);
        }

        GameObject troop;
        Troop troopRef;

        if (inPlayerType == PlayerType.Player)
        {
            troop = ObjectPooler.Instance.GetPooledObject("AllyTroop");
        }
        else
        {
            troop = ObjectPooler.Instance.GetPooledObject("EnemyTroop");
        }

        troopRef = troop.GetComponent<Troop>();


        if (troop == null || troopRef == null)
        {
            Debug.Log(inUnitType + " can't be taken from pool. Pls check Collection or add Troop script to Captain");
            return null;
        }

        troopRef.Agent.Warp(inPosition);
        troop.SetActive(true);

        troopRef.AssignStats(Collection.UnitsDictionary[inUnitType].UnitStatsCopy);
        troopRef.AssignPlayer(inPlayerType);

        return troopRef;
    }

    public UnitInfo? SearchUnitInfoInDictionary(UnitType inType)
    {
        if (Collection.UnitsDictionary.ContainsKey(inType))
        {
            return Collection.UnitsDictionary[inType];
        }
        return null;
    }



    #endregion

    #region Building Methods

    public BuildingBehaviour CreateNewBuilding(BuildingType inBuildingType, Vector3 inPosition)
    {
        if (!Collection.BuildingsDictionary.ContainsKey(inBuildingType))
        {
            Debug.Log("GameCollection don't contain " + inBuildingType);
            return null;
        }

        GameObject building = ObjectPooler.Instance.GetPooledObject("Building");
        BuildingBehaviour buildingRef = building.GetComponent<BuildingBehaviour>();

        if (building == null || buildingRef == null)
        {
            Debug.Log(inBuildingType + " can't be taken from pool. Pls check Collection or add Troop script to Captain");
            return null;
        }

        building.transform.position = inPosition;
        building.SetActive(true);
        building.gameObject.layer = m_playerLayer;
        
        buildingRef.AssignStats(Collection.BuildingsDictionary[inBuildingType].BuildingStatsCopy);

        return buildingRef;
    }

    public BuildingInfo? SearchBuildingInfoInDictionary(BuildingType inType)
    {
        if (Collection.BuildingsDictionary.ContainsKey(inType))
        {
            return Collection.BuildingsDictionary[inType];
        }
        return null;

    }

    #endregion

    #endregion

    public Vector3? RandomPoint(Vector3 center, float range)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return null;
    }

    #region Resources system

    public bool CheckResourcesAvailability(int inCost)
    {
        return m_currentPlayerResources >= inCost;
    }

    public void AddResources(int inValue)
    {
        m_currentPlayerResources += inValue;

        HFEventManager.TriggerEvent<int, bool>(HFEventID.OnGemChanged, m_currentPlayerResources, inValue > 0);
        
    }

    public void SetResources(int inValue)
    {
        m_currentPlayerResources = inValue;
        HFEventManager.TriggerEvent<int, bool>(HFEventID.OnGemChanged, m_currentPlayerResources, false);
    }

    #endregion


    /// <summary>
    /// Get icon from unit stats type.
    /// </summary>
    public Sprite GetIcon(UnitType unitType)
    {
        return Collection.UnitsDictionary[unitType].OriginalUnitStats.Icon;
    }

    /// <summary>
    /// Get icon from building stats type.
    /// </summary>
    public Sprite GetIcon(BuildingType unitType)
    {
        return Collection.BuildingsDictionary[unitType].OriginalBuildingStats.Icon;
    }

}