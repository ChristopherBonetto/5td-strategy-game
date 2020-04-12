using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EntityType
{
    Farmer,
    Defender,
    Lifter,
    Runner,
}

public enum Materials
{
    Gems,
}


[CreateAssetMenu(menuName = "NewGameCollection", fileName = "GameCollection")]
public class GameCollection : ScriptableObject
{

    [SerializeField] private LayerMask m_playerLayer;
    public LayerMask PlayerLayer
    {
        get
        {
            return m_playerLayer;
        }
    }

    [SerializeField] private LayerMask m_aiLayer;
    public LayerMask AILayer
    {
        get
        {
            return m_aiLayer;
        }
    }

    [SerializeField] private int m_playerQuantityResources;

    public int PlayerQuantityResources { get { return m_playerQuantityResources; } private set { } }

    [Space, Header("Player")]
    [SerializeField] private EntityStatistics[] m_gameEntities;

    public Dictionary<EntityType, EntityStatistics> GameEntitiesDictionary { get; private set; }


    private void Awake()
    {

        GameEntitiesDictionary = new Dictionary<EntityType, EntityStatistics>();

        for (int i = 0; i < m_gameEntities.Length; i++)
        {
            if (!GameEntitiesDictionary.ContainsKey(m_gameEntities[i].EntityType))
            {
                EntityStatistics tempCopy = Instantiate(m_gameEntities[i]);
                GameEntitiesDictionary.Add(m_gameEntities[i].EntityType, tempCopy);
            }
            else
            {
                Debug.Log(m_gameEntities[i].EntityType + " can't be added because there is another key with same value");
            }
        }

    }
}
