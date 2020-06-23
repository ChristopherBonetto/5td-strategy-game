using UnityEngine;
using Types;
using UnityEngine.AI;
using System.Collections.Generic;
using System;

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

    private Dictionary<Type, List<EntityBehavior>> m_inGameAllyEntitiesDictionary = new Dictionary<Type, List<EntityBehavior>>();

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

    #region Troop methods

    public Troop CreateNewTroop(UnitType inUnitType, PlayerType inPlayerType, Vector3 inPosition, bool inGratis)
    {
        if (!Collection.UnitsDictionary.ContainsKey(inUnitType))
        {
            Debug.Log("GameCollection don't contain " + inUnitType);
            return null;
        }

        Vector3? closestPoint = RandomPoint(inPosition, 1, inPosition);

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
        troopRef.StopTree(false);

        if(inPlayerType == PlayerType.Player)
        {
            AddEntityToDictionary(troopRef);
        }

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

    /// <summary>
    /// Get icon from unit stats type.
    /// </summary>
    public Sprite GetIcon(UnitType unitType)
    {
        return Collection.UnitsDictionary[unitType].OriginalUnitStats.Icon;
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

        int cost = Collection.BuildingsDictionary[inBuildingType].BuildingStatsCopy.Cost;
        if (!CheckResourcesAvailability(cost))
        {
            Debug.LogWarning("U don't have resources to create " + inBuildingType);
            return null;
        }
        AddResources(-cost);

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

        buildingRef.StopTree(false);

        if(buildingRef.EntityPlayerType == PlayerType.Player)
        {
            AddEntityToDictionary(buildingRef);
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

    /// <summary>
    /// Get icon from building stats type.
    /// </summary>
    public Sprite GetIcon(BuildingType unitType)
    {
        return Collection.BuildingsDictionary[unitType].OriginalBuildingStats.Icon;
    }

    #endregion

    public void AddEntityToDictionary(EntityBehavior entity)
    {
        Type entityType;

        if(m_inGameAllyEntitiesDictionary == null)
        {
            m_inGameAllyEntitiesDictionary = new Dictionary<Type, List<EntityBehavior>>();
        }

        if (entity is BuildingBehaviour)
        {
            entityType = typeof(BuildingBehaviour);

            if (!m_inGameAllyEntitiesDictionary.ContainsKey(entityType))
            {
                List<EntityBehavior> entityList = new List<EntityBehavior>();
                entityList.Add(entity);
                Debug.Log("Created new list for buildings " + entity.transform.name);
                m_inGameAllyEntitiesDictionary.Add(entityType, entityList);
            }
            else
            {
                Debug.Log("Added new building " + entity.transform.name);
                m_inGameAllyEntitiesDictionary[entityType].Add(entity);
            }
        }
        else if(entity is Troop)
        {
            entityType = typeof(Troop);

            if (!m_inGameAllyEntitiesDictionary.ContainsKey(entityType))
            {
                List<EntityBehavior> entityList = new List<EntityBehavior>();
                entityList.Add(entity);
                Debug.Log("Created new list for troops " + entity.transform.name);
                m_inGameAllyEntitiesDictionary.Add(entityType, entityList);
            }
            else
            {
                Debug.Log("Added new troop " + entity.transform.name);
                m_inGameAllyEntitiesDictionary[entityType].Add(entity);
            }
        }
    }

    public void DebugDictionary()
    {
        foreach(Type key in m_inGameAllyEntitiesDictionary.Keys)
        {
            Debug.Log(m_inGameAllyEntitiesDictionary[key].ToString() + " contains : " + m_inGameAllyEntitiesDictionary[key].Count);
        }
    }

    public void ClearDictionary()
    {
        if(m_inGameAllyEntitiesDictionary != null)
        {
            m_inGameAllyEntitiesDictionary.Clear();
        }
    }

    public EntityBehavior TakeEntityFromDictionary(Type key, int inIndex)
    {
        if (m_inGameAllyEntitiesDictionary == null || !m_inGameAllyEntitiesDictionary.ContainsKey(key)) return null;

        if (inIndex >= m_inGameAllyEntitiesDictionary[key].Count) return null;

        EntityBehavior wantedEntity = m_inGameAllyEntitiesDictionary[key][inIndex];

        if (wantedEntity != null)
        {
            InputReaderManager.Instance.SelectEntity(wantedEntity);
            return wantedEntity;
        }
        return null;
    }

    public EntityBehavior TakeEntityFromDictionary(Type key)
    {
        if (m_inGameAllyEntitiesDictionary == null || !m_inGameAllyEntitiesDictionary.ContainsKey(key)) return null;

        Debug.LogWarning(m_inGameAllyEntitiesDictionary[key].Count);

        EntityBehavior previousEntity = InputReaderManager.Instance.CurrentEntity;
        int index = 0;

        if(previousEntity != null && previousEntity is Troop && previousEntity.EntityPlayerType == PlayerType.Player)
        {
            index = m_inGameAllyEntitiesDictionary[key].IndexOf(previousEntity) + 1;

            if(index >= m_inGameAllyEntitiesDictionary[key].Count)
            {
                index = 0;
            }

            Debug.Log(index);
        }

        EntityBehavior wantedEntity = m_inGameAllyEntitiesDictionary[key][index];

        InputReaderManager.Instance.SelectEntity(wantedEntity);

        return null;
    }


    public void RemoveFromDictionary(EntityBehavior entity)
    {
        Type entityType = null;

        if(entity is Troop)
        {
            entityType = typeof(Troop);
        }
        else if(entity is BuildingBehaviour)
        {
            entityType = typeof(BuildingBehaviour);
        }

        if(entityType != null && m_inGameAllyEntitiesDictionary.ContainsKey(entityType))
        {
            m_inGameAllyEntitiesDictionary[entityType].Remove(entity);
        }
    }

    #endregion

    public Vector3? RandomPoint(Vector3 center, float range, Vector3 agentPosition)
    {
        NavMeshHit closestPoint = new NavMeshHit();
        float lowestDistance = Mathf.Infinity;

        for (int i = 0; i < 10; i++)
        {
            NavMeshHit hit;
            Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range;

            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                float distance = Vector3.Distance(randomPoint, agentPosition);

                if (distance < lowestDistance)
                {
                    lowestDistance = distance;
                    closestPoint = hit;
                }
            }
        }
        return closestPoint.position;
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


}