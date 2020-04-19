using System.Collections;
using System.Collections.Generic;
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
    public QuantityOfResources[] m_currentPlayerResources { get; private set; }

    private float m_timer = 0f;

    //must be take from assets folder.
    [SerializeField] private GameObject m_troopsContainer;

    [SerializeField] Transform[] m_enemySpawnPoints;
    [SerializeField] Transform m_castle;


    private void Awake()
    {
        Collection = Instantiate(m_gameCollection);
    }
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            CreateNewTroop(UnitType.DEFENDER, PlayerType.Player, new Vector3(0,0.5f,0));
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            CreateNewTroop(UnitType.PEASANT, PlayerType.AI, new Vector3(0, 0.5f, 0));
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

    private void Initialize()
    {
        m_playerLayer = GetGameObjectLayer(Collection.PlayerLayer);
        m_aiLayer = GetGameObjectLayer(Collection.AILayer);

        m_currentPlayerResources = new QuantityOfResources[Collection.ResourcesValuesDictionary.Count];
        Collection.ResourcesValuesDictionary.Values.CopyTo(m_currentPlayerResources, 0);
    }

    public void CreateNewTroop(UnitType inEntityType, PlayerType inPlayerType, Vector3 inPosition)
    {
        if (!Collection.UnitsDictionary.ContainsKey(inEntityType))
        {
            Debug.Log("GameCollection don't contain " + inEntityType);
            return;
        }

        if (!CheckResourcesAvailability(Collection.UnitsDictionary[inEntityType].UnitStatsCopy.Cost))
        {
            Debug.Log("u need more resources");
            return;
        }

        DecreaseResources(Collection.UnitsDictionary[inEntityType].UnitStatsCopy.Cost);

        //CheckFreeSpace(inPosition, 1);

        GameObject troop = ObjectPooler.SharedInstance.GetPooledObject("Troop");
        troop.SetActive(true);

        if (troop == null)
        {
            Debug.Log("can't take troop container because in pool it can't expand");
            return;
        }
        TroopBehavior tempRef = troop.GetComponent<TroopBehavior>();

        if (tempRef == null)
        {
            Debug.LogError("Prefab need TroopsBehavior script");
            return;
        }

        tempRef.AssignPlayer(inPlayerType);
        tempRef.AssignStats(Collection.UnitsDictionary[inEntityType].UnitStatsCopy);
    }

    public UnitInfo? SearchUnit(UnitType inType)
    {
        if (Collection.UnitsDictionary.ContainsKey(inType))
        {
            return Collection.UnitsDictionary[inType];
        }
        return null;
    }

    //public void CheckFreeSpace(Vector3 inPos, float inRadius)
    //{
    //    int layerToCheck = 9 << 10;

    //    Collider[] collider = Physics.OverlapSphere(inPos, inRadius, layerToCheck);

    //    EntityBehavior entity = null;

    //    for(int i = 0; i < collider.Length; i++)
    //    {
    //        EntityBehavior tempEntity = collider[i].GetComponentInParent<EntityBehavior>();
            
    //        if(tempEntity != entity)
    //        {
    //            entity = tempEntity;
    //            var command = new TeleportCommand(entity new Vector3(10,0,10));
    //            entity.ExecuteCommand(command);
    //        }
    //    }
    //}



    #region Resources

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


    public bool Timer(float destinationTime)
    {
        m_timer += Time.deltaTime;
        if (m_timer >= destinationTime)
        {
            m_timer = 0f;

            return true;
        }
        else
        {
            return false;
        }
    }
    
}