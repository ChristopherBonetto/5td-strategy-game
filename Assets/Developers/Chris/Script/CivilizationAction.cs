using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilizationAction : MonoBehaviour
{
    public QuantityOfResources[] m_CurrentCivilizationResources { get; private set; }
    
    [SerializeField] private CivilizationStatistics m_selectedCivilizationSO;
    public CivilizationStatistics CurrentCivilizationSO = null;

    public Transform[] CivilizationSpawnPoint;

    private void Awake()
    {
        CurrentCivilizationSO = Instantiate(m_selectedCivilizationSO);
    }

    // Start is called before the first frame update
    void Start()
    {
        CopyResourcesFromCivilization();
    }
    

    public void InstantiateEntityFromType(EntityType inEntityType, Transform inPos)
    {
        if (CurrentCivilizationSO.EntitiesDictionary.ContainsKey(inEntityType))
        {
            if(CurrentCivilizationSO.EntitiesDictionary[inEntityType].EntityPrefab != null && inPos != null)
            {
                InstantiateEntity(inEntityType, inPos);
            }
        }
    }

    public void InstantiateEntity(EntityType inEntityType, Transform inPos)
    {
        GameObject entity = Instantiate(CurrentCivilizationSO.EntitiesDictionary[inEntityType].EntityPrefab, inPos.position, Quaternion.identity);
        entity.transform.name = CurrentCivilizationSO.EntitiesDictionary[inEntityType].EntityStatsCopy.EntityName;
        UnitActions entityAI = entity.GetComponent<UnitActions>();

        if (entityAI != null && entityAI is UnitActions)
        {
            entityAI.EntityStatisticsSO = CurrentCivilizationSO.EntitiesDictionary[inEntityType].EntityStatsCopy;
        }
    }

    #region Resources

    private void CopyResourcesFromCivilization()
    {
        m_CurrentCivilizationResources = new QuantityOfResources[CurrentCivilizationSO.ResourcesValuesDictionary.Count];
        CurrentCivilizationSO.ResourcesValuesDictionary.Values.CopyTo(m_CurrentCivilizationResources, 0);
    }

    public bool CheckResourcesAvailability(QuantityOfResources[] ResourcesToCheck)
    {
        int CivilizationHaveThatResource = 0;

        for (int i = 0; i < ResourcesToCheck.Length; i++)
        {
            for (int j = 0; j < m_CurrentCivilizationResources.Length; j++)
            {
                if (ResourcesToCheck[i].ResourceType == m_CurrentCivilizationResources[j].ResourceType)
                {
                    if (m_CurrentCivilizationResources[j].ResourceQuantity >= ResourcesToCheck[i].ResourceQuantity)
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
            for (int j = 0; j < m_CurrentCivilizationResources.Length; j++)
            {
                if (QuantityOfResources[i].ResourceType == m_CurrentCivilizationResources[j].ResourceType)
                {
                    m_CurrentCivilizationResources[j].ResourceQuantity -= QuantityOfResources[i].ResourceQuantity;
                }
            }
        }
    }

    public void AddResources(QuantityOfResources[] QuantityOfResources)
    {
        for (int i = 0; i < QuantityOfResources.Length; i++)
        {
            for (int j = 0; j < m_CurrentCivilizationResources.Length; j++)
            {
                if (QuantityOfResources[i].ResourceType == m_CurrentCivilizationResources[j].ResourceType)
                {
                    m_CurrentCivilizationResources[j].ResourceQuantity += QuantityOfResources[i].ResourceQuantity;
                    QuantityOfResources[i].ResourceQuantity = 0;
                }
            }
        }
    }

    #endregion

}
