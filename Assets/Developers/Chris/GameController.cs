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
            CreateNewEntity(UnitType.DEFENDER, PlayerType.Player, new Vector3(0,0.5f,0));
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            CreateNewEntity(UnitType.PEASANT, PlayerType.AI, new Vector3(0, 0.5f, 0));
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

    public void CreateNewEntity(UnitType inEntityType, PlayerType inPlayerType, Vector3? inPosition = null)
    {
        if (!Collection.UnitsDictionary.ContainsKey(inEntityType))
        {
            Debug.Log("GameCollection don't contain " + inEntityType);
            return;
        }

        GameObject troop = ObjectPooler.SharedInstance.GetPooledObject("Troop");
        TroopBehavior tempRef = troop.GetComponent<TroopBehavior>();

        if (tempRef != null)
        {
            tempRef.AssignPlayer(inPlayerType);
            tempRef.AssignStats(Collection.UnitsDictionary[inEntityType].UnitStatsCopy);
        }
        else
        {
            Debug.LogError("Prefab need TroopsBehavior script");
            return;
        }

        troop.SetActive(true);
    }

    


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