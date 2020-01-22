using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilizationAction : MonoBehaviour
{
    public static CivilizationAction Instance;

    public QuantityOfResources[] m_CurrentCivilizationResources { get; private set; }
    
    [SerializeField] private CivilizationStatistics m_selectedCivilizationSO;
    public CivilizationStatistics CurrentCivilizationSO = null;


    private void Awake()
    {
        Instance = this;

        CurrentCivilizationSO = Instantiate(m_selectedCivilizationSO);
    }

    // Start is called before the first frame update
    void Start()
    {
        CopyResourcesFromCivilization();
    }
    
    private void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            Debug.Log("First resources quantity" + CurrentCivilizationSO.ResourcesValuesDictionary[0].ResourceQuantity);
            Debug.Log("First unit name" + CurrentCivilizationSO.UnitsDictionary[0].UnitName);
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
