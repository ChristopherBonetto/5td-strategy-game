using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;




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

    public GameObject PlayerCastle;

    private float m_timer = 0f;



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
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    CreateNewEntity(UnitType.DEFENDER, PlayerType.Player, Vector3.zero);
        //}

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    CreateNewEntity(UnitType.PEASANT, PlayerType.AI, Vector3.zero);
        //}
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

        m_currentPlayerResources = new QuantityOfResources[m_collection.ResourcesValuesDictionary.Count];
        m_collection.ResourcesValuesDictionary.Values.CopyTo(m_currentPlayerResources, 0);
    }

    //public void CreateNewEntity(UnitType inEntityType, PlayerType inPlayerType, Vector3 inPosition)
    //{
    //    if (!Collection.GameEntitiesDictionary.ContainsKey(inEntityType))
    //    {
    //        Debug.Log("GameCollection don't contain " + inEntityType);
    //        return;
    //    }

    //    GameObject tempEntity = Instantiate(Collection.GameEntitiesDictionary[inEntityType].EntityPrefab, inPosition, Quaternion.identity);
    //    Entity tempRef = tempEntity.GetComponent<Entity>();

    //    if(tempRef != null)
    //    {
    //        tempRef.AssignStats(Collection.GameEntitiesDictionary[inEntityType]);
    //    }

    //    tempRef.AssignPlayer(inPlayerType);
    //}

    #region Resources

    public bool CheckResourcesAvailability(QuantityOfResources[] ResourcesToCheck)
    {
        int CivilizationHaveThatResource = 0;

        for (int i = 0; i < ResourcesToCheck.Length; i++)
        {
            for (int j = 0; j < m_currentPlayerResources.Length; j++)
            {
                if (ResourcesToCheck[i].ResourceType == m_currentPlayerResources[j].ResourceType)
                {
                    if (m_currentPlayerResources[j].ResourceQuantity >= ResourcesToCheck[i].ResourceQuantity)
                    {
                        CivilizationHaveThatResource++;
                    }
                }
            }
        }
        if (CivilizationHaveThatResource >= ResourcesToCheck.Length)
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
            for (int j = 0; j < m_currentPlayerResources.Length; j++)
            {
                if (QuantityOfResources[i].ResourceType == m_currentPlayerResources[j].ResourceType)
                {
                    m_currentPlayerResources[j].ResourceQuantity -= QuantityOfResources[i].ResourceQuantity;
                }
            }
        }
    }

    public void AddResources(QuantityOfResources[] QuantityOfResources)
    {
        for (int i = 0; i < QuantityOfResources.Length; i++)
        {
            for (int j = 0; j < m_currentPlayerResources.Length; j++)
            {
                if (QuantityOfResources[i].ResourceType == m_currentPlayerResources[j].ResourceType)
                {
                    m_currentPlayerResources[j].ResourceQuantity += QuantityOfResources[i].ResourceQuantity;
                    QuantityOfResources[i].ResourceQuantity = 0;
                }
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