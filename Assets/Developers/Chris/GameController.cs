using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using UnityEngine.AI;
using HF.Unit;
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
    public QuantityOfResources[] m_currentPlayerResources { get; private set; }

    #region Behavior Cycle

    private void Awake()
    {
        Collection = Instantiate(m_gameCollection);
    }
    void Start()
    {
        Initialize();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            CreateEntity();
        }
    }

    #endregion

    #region Generic method

    private void Initialize()
    {
        m_playerLayer = GetGameObjectLayer(Collection.PlayerLayer);
        m_aiLayer = GetGameObjectLayer(Collection.AILayer);

        m_currentPlayerResources = new QuantityOfResources[Collection.ResourcesValuesDictionary.Count];
        Collection.ResourcesValuesDictionary.Values.CopyTo(m_currentPlayerResources, 0);
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

    public void CreateEntity()
    {
        CreateNewTroop(UnitType.DEFENDER, PlayerType.Player, new Vector3(-10, 0f, -10));
        CreateNewTroop(UnitType.WARRIOR, PlayerType.AI, new Vector3(10, 0f, 10));
        CreateNewTroop(UnitType.WARRIOR, PlayerType.AI, new Vector3(10, 0f, 20));

        CreateNewBuilding(BuildingType.TOWER, PlayerType.Player, new Vector3(15, 0, 15));
        CreateNewBuilding(BuildingType.CASTLE, PlayerType.Player, new Vector3(-10, 0, -30));
    }

    #region Unit methods

    public Troop CreateNewTroop(UnitType inUnitType, PlayerType inPlayerType, Vector3 inPosition)
    {
        if (!Collection.UnitsDictionary.ContainsKey(inUnitType))
        {
            Debug.Log("GameCollection don't contain " + inUnitType);
            return null;
        }

        if (inPlayerType == PlayerType.Player)
        {
            if (!CheckResourcesAvailability(Collection.UnitsDictionary[inUnitType].UnitStatsCopy.Cost))
            {
                Debug.Log("u need more resources");
                return null;
            }
            DecreaseResources(Collection.UnitsDictionary[inUnitType].UnitStatsCopy.Cost);
        }

        //CheckFreeSpace(inPosition, 1);

        GameObject troopBrain = ObjectPooler.Instance.GetUnitBehaviorHandler(inUnitType);
        Troop troopRef = troopBrain.GetComponent<Troop>();

        if (troopBrain == null || troopRef == null)
        {
            Debug.Log(inUnitType + " can't be taken from pool. Pls check Collection or add Troop script to Captain");
            return null;
        }

        troopBrain.transform.position = inPosition;
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
            if (!CheckResourcesAvailability(Collection.BuildingsDictionary[inBuildingType].BuildingStatsCopy.Cost))
            {
                Debug.Log("u need more resources");
                return null;
            }
            DecreaseResources(Collection.BuildingsDictionary[inBuildingType].BuildingStatsCopy.Cost);
        }

        //CheckFreeSpace(inPosition, 1);

        GameObject buildingBrain = ObjectPooler.Instance.GetBuildingBehaviorHandler(inBuildingType);
        BuildingBehaviour tempRef = buildingBrain.GetComponent<BuildingBehaviour>();

        if (buildingBrain == null || tempRef == null)
        {
            Debug.LogError(inBuildingType + " can't be taken from pool. Pls check Collection or add Building Behavior script");
            return null;
        }

        buildingBrain.transform.position = inPosition;
        buildingBrain.SetActive(true);

        tempRef.AssignPlayer(inPlayerType);
        tempRef.AssignStats(Collection.BuildingsDictionary[inBuildingType].BuildingStatsCopy);

        if(inBuildingType == BuildingType.CASTLE)
        {
            var castle = GlobalVariables.Instance.GetVariable("Castle");
            castle.SetValue(tempRef.gameObject);
        }

        return tempRef;
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

    //public void CheckFreeSpace(Vector3 inPos, float inRadius)
    //{
    //    int layerToCheck = 9 << 10;

    //    Collider[] collider = Physics.OverlapSphere(inPos, inRadius, layerToCheck);

    //    EntityBehavior entity = null;

    //    for (int i = 0; i < collider.Length; i++)
    //    {
    //        EntityBehavior tempEntity = collider[i].GetComponentInParent<EntityBehavior>();

    //        if (tempEntity != entity)
    //        {
    //            entity = tempEntity;
    //            var command = new TeleportCommand(entity, new Vector3(10,0,10));
    //            entity.ExecuteCommand(command);
    //        }
    //    }
    //}

    //protected virtual Vector3 RandomNavmeshLocation(float radius)
    //{
    //    Vector3 randomDirection = Random.insideUnitSphere * radius;
    //    randomDirection += transform.position;
    //    NavMeshHit hit;
    //    Vector3 finalPosition = Vector3.zero;
    //    if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
    //    {
    //        finalPosition = hit.position;
    //    }

    //    return finalPosition;
    //}

    #region Resources system

    public bool CheckResourcesAvailability(QuantityOfResources ResourcesToCheck)
    {
        for (int j = 0; j < m_currentPlayerResources.Length; j++)
        {
            if (m_currentPlayerResources[j].ResourceType == ResourcesToCheck.ResourceType)
            {
                if (m_currentPlayerResources[j].ResourceQuantity >= ResourcesToCheck.ResourceQuantity)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void DecreaseResources(QuantityOfResources inResource)
    {
        for (int j = 0; j < m_currentPlayerResources.Length; j++)
        {
            if (m_currentPlayerResources[j].ResourceType == inResource.ResourceType)
            {
                if (m_currentPlayerResources[j].ResourceQuantity >= inResource.ResourceQuantity)
                {
                    m_currentPlayerResources[j].ResourceQuantity -= inResource.ResourceQuantity;
                }
            }
        }
    }

    public void AddResources(QuantityOfResources inResource)
    {
        for (int j = 0; j < m_currentPlayerResources.Length; j++)
        {
            if (m_currentPlayerResources[j].ResourceType == inResource.ResourceType)
            {
                m_currentPlayerResources[j].ResourceQuantity += inResource.ResourceQuantity;
            }
        }
    }

    #endregion

}