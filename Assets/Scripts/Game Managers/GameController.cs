using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;

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

    public Troop CreateNewTroop(UnitType inUnitType, PlayerType inPlayerType, Vector3 inPosition)
    {
        if (!Collection.UnitsDictionary.ContainsKey(inUnitType))
        {
            Debug.Log("GameCollection don't contain " + inUnitType);
            return null;
        }

        Vector3? closestPoint = RandomPoint(inPosition, 1);

        if(closestPoint != null)
        {
            inPosition = closestPoint.Value;
        }
        else
        {
            Debug.Log("Not finded a point");
            return null;
        }


        if (inPlayerType == PlayerType.Player)
        {
            int cost = Collection.UnitsDictionary[inUnitType].UnitStatsCopy.Cost;

            if (!CheckResourcesAvailability(cost))
            {
                Debug.LogWarning("U don't have resources to create " + inUnitType);
                return null;
            }
            AddResources(-cost);
        }

        GameObject troopBrain = ObjectPooler.Instance.GetUnitBehaviorHandler(inUnitType);
        Troop troopRef = troopBrain.GetComponent<Troop>();

        if (troopBrain == null || troopRef == null)
        {
            Debug.Log(inUnitType + " can't be taken from pool. Pls check Collection or add Troop script to Captain");
            return null;
        }

        troopRef.Agent.Warp(inPosition);
        troopBrain.SetActive(true);

        troopRef.AssignPlayer(inPlayerType);
        troopRef.AssignStats(Collection.UnitsDictionary[inUnitType].UnitStatsCopy);

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

    public BuildingBehaviour CreateNewBuilding(BuildingType inBuildingType, PlayerType inPlayerType, Vector3 inPosition)
    {
        if (!Collection.BuildingsDictionary.ContainsKey(inBuildingType))
        {
            Debug.Log("GameCollection don't contain " + inBuildingType);
            return null;
        }

        if (inPlayerType == PlayerType.Player)
        {
            int cost = Collection.BuildingsDictionary[inBuildingType].BuildingStatsCopy.Cost;

            if (!CheckResourcesAvailability(cost))
            {
                Debug.LogWarning("U don't have resources to create " + inBuildingType);
                return null;
            }
            AddResources(-cost);
        }

        //CheckFreeSpace(inPosition, 1);

        GameObject buildingBrain = ObjectPooler.Instance.GetBuildingBehaviorHandler(inBuildingType);
        BuildingBehaviour buildingRef = buildingBrain.GetComponent<BuildingBehaviour>();

        if (buildingBrain == null || buildingRef == null)
        {
            Debug.Log(inBuildingType + " can't be taken from pool. Pls check Collection or add Troop script to Captain");
            return null;
        }

        buildingBrain.transform.position = inPosition;
        buildingBrain.SetActive(true);

        buildingRef.AssignPlayer(inPlayerType);
        buildingRef.AssignStats(Collection.BuildingsDictionary[inBuildingType].BuildingStatsCopy);

        if(inBuildingType == BuildingType.CASTLE)
        {
            GlobalVariables.Instance.SetVariableValue("Castle",buildingRef.gameObject);
        }

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
    }

    public void SetResources(int inValue)
    {
        m_currentPlayerResources = inValue;
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