using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum UnitQualities
{
    None = 0,
    Melee = 1 << 0,
    Ranged = 1 << 1,
    Infantry = 1 << 2,
    Cavalry = 1 << 3
}

public enum Units
{
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
public struct UnitInfo
{
    public string UnitName;

    [SerializeField] private UnitStatistics m_OriginalUnitStats;
    public UnitStatistics OriginalUnitStats { get { return m_OriginalUnitStats; } }

    private UnitStatistics m_UnitStatsCopy;
    public UnitStatistics UnitStatsCopy
    {
        get
        {
            return m_UnitStatsCopy;
        }
        set
        {
            m_UnitStatsCopy = value;
        }
    }

    public GameObject UnitPrefab;
    public QuantityOfResources[] UnitUpgradeCost;
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
    [SerializeField] private UnitInfo[] m_CivilizationUnits;
    public Dictionary<Units, UnitInfo> UnitsDictionary { get; private set; }



    private void Awake()
    {
        UnitsDictionary = new Dictionary<Units, UnitInfo>();
        ResourcesValuesDictionary = new Dictionary<Materials, QuantityOfResources>();

        for (int i = 0; i < m_CivilizationUnits.Length; i++)
        {
            if (!UnitsDictionary.ContainsKey(m_CivilizationUnits[i].OriginalUnitStats.UnitType))
            {
                m_CivilizationUnits[i].UnitStatsCopy = Instantiate(m_CivilizationUnits[i].OriginalUnitStats);
                UnitsDictionary.Add(m_CivilizationUnits[i].UnitStatsCopy.UnitType, m_CivilizationUnits[i]);
            }
            else
            {
                Debug.Log(m_CivilizationUnits[i].OriginalUnitStats.UnitType + " can't be added because there is another key with same value");
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
