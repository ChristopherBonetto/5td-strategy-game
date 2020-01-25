using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum EntityQualities
{
    None = 0,
    Melee = 1 << 0,
    Ranged = 1 << 1,
    Infantry = 1 << 2,
    Cavalry = 1 << 3
}

public enum EntityType
{
    Castle,
    Soldier,
    Lancer,
    ArcherMedium,
    ArcherLong,
    Knight,
    BowKnight
}


public enum Materials
{
    Gold,
    Gems
}

[System.Serializable]
public struct QuantityOfResources
{
    public Materials ResourceType;
    public int ResourceQuantity;
}


[System.Serializable]
public struct EntityInfo
{
    [SerializeField] private EntityStatistics m_OriginalEntityStats;
    public EntityStatistics OriginalEntityStats { get { return m_OriginalEntityStats; } }

    private EntityStatistics m_EntityStatsCopy;
    public EntityStatistics EntityStatsCopy
    {
        get
        {
            return m_EntityStatsCopy;
        }
        set
        {
            m_EntityStatsCopy = value;
        }
    }

    public GameObject EntityPrefab;
    public QuantityOfResources[] EntityUpgradeCost;
}


[CreateAssetMenu(menuName = "NewCivilizationInfo", fileName = "Civilization")]
public class CivilizationStatistics : ScriptableObject
{
    [Header("Civilitazion's Info")]
    [SerializeField] private string m_CivilizationName;
    public string CivilizationName
    {
        get
        {
            return m_CivilizationName;
        }
    }

    [SerializeField] private QuantityOfResources[] m_CivilizationQuantityResources;
    public Dictionary<Materials, QuantityOfResources> ResourcesValuesDictionary { get; private set; }

    [Space, Header("Civilitazion's Units")]
    [SerializeField] private EntityInfo[] m_CivilizationEntities;
    public Dictionary<EntityType, EntityInfo> EntitiesDictionary { get; private set; }


    private void Awake()
    {
        EntitiesDictionary = new Dictionary<EntityType, EntityInfo>();
        ResourcesValuesDictionary = new Dictionary<Materials, QuantityOfResources>();

        for (int i = 0; i < m_CivilizationEntities.Length; i++)
        {
            if (!EntitiesDictionary.ContainsKey(m_CivilizationEntities[i].OriginalEntityStats.EntityType))
            {
                m_CivilizationEntities[i].EntityStatsCopy = Instantiate(m_CivilizationEntities[i].OriginalEntityStats);
                EntitiesDictionary.Add(m_CivilizationEntities[i].EntityStatsCopy.EntityType, m_CivilizationEntities[i]);
            }
            else
            {
                Debug.Log(m_CivilizationEntities[i].OriginalEntityStats.EntityType + " can't be added because there is another key with same value");
            }
        }


        for (int i = 0; i < m_CivilizationQuantityResources.Length; i++)
        {
            if (!ResourcesValuesDictionary.ContainsKey(m_CivilizationQuantityResources[i].ResourceType))
            {
                ResourcesValuesDictionary.Add(m_CivilizationQuantityResources[i].ResourceType, m_CivilizationQuantityResources[i]);
            }
            else
            {
                Debug.Log(m_CivilizationQuantityResources[i].ResourceType + " can't be added because there is another key with same value");
            }
        }
    }


}
