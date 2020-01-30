using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilizationAction : MonoBehaviour
{
    //public QuantityOfResources[] m_CurrentCivilizationResources { get; private set; }
    
    //[SerializeField] private CivilizationStatistics m_selectedCivilizationSO;
    //public CivilizationStatistics CurrentCivilizationSO { get; private set; }

    //public Transform[] CivilizationUnitsSpawnPoint;
    //public Transform CivilizationCastleSpawnPoint;

    //public CastleActions CivilizationCastle;

    //public List<UnitActions> CivilizationUnits = new List<UnitActions>();

    //public virtual void Awake()
    //{
    //    CurrentCivilizationSO = Instantiate(m_selectedCivilizationSO);
    //}

    //// Start is called before the first frame update
    //public virtual void Start()
    //{
    //    CopyResourcesFromCivilization();

    //    TestStartingInstantiate();
    //}

    //#region Instantiate Castle and Soldiers

    //public virtual void TestStartingInstantiate()
    //{
    //    InstantiateCastle();

    //    InstantiateCivilizationTroups();
    //}

    //public virtual void InstantiateCastle()
    //{
    //    if(CurrentCivilizationSO.CivilizationCastle.gameObject != null && CivilizationCastleSpawnPoint != null)
    //    {
    //        GameObject LevelCastle = Instantiate(CurrentCivilizationSO.CivilizationCastle, CivilizationCastleSpawnPoint.position, Quaternion.identity);
    //        CastleActions tempRef = LevelCastle.GetComponent<CastleActions>();

    //        if (tempRef != null)
    //        {
    //            CivilizationCastle = tempRef;
    //            CivilizationCastle.RefreshHp(CurrentCivilizationSO.CastleHp);
    //            GameManager.Instance.SetObjective(CivilizationCastle);
    //        }
    //    }
    //    else
    //    {

    //    }
    //}

    //public virtual void InstantiateCivilizationTroups()
    //{
    //    for (int i = 0; i < CivilizationUnitsSpawnPoint.Length; i++)
    //    {
    //        InstantiateEntityFromType(EntityType.Soldier, CivilizationUnitsSpawnPoint[i]);
    //    }
    //}

    //#endregion

    //#region Instantiate Methods

    //public virtual void InstantiateEntityFromType(EntityType inEntityType, Transform inPos)
    //{
    //    if (CurrentCivilizationSO.EntitiesDictionary.ContainsKey(inEntityType))
    //    {
    //        if(CurrentCivilizationSO.EntitiesDictionary[inEntityType].EntityPrefab != null && inPos != null)
    //        {
    //            InstantiateEntity(inEntityType, inPos);
    //        }
    //    }
    //}

    //public virtual void InstantiateEntity(EntityType inEntityType, Transform inPos)
    //{
    //    GameObject entity = Instantiate(CurrentCivilizationSO.EntitiesDictionary[inEntityType].EntityPrefab, inPos.position, Quaternion.identity);
    //    entity.transform.name = CurrentCivilizationSO.EntitiesDictionary[inEntityType].EntityStatsCopy.EntityName;
    //    UnitActions tempUnitRef = entity.GetComponent<UnitActions>();

    //    if (tempUnitRef != null)
    //    {
    //        tempUnitRef.EntityStatisticsSO = CurrentCivilizationSO.EntitiesDictionary[inEntityType].EntityStatsCopy;
    //        AddUnitsToList(tempUnitRef);
    //    }
    //}

    //public virtual void AddUnitsToList(UnitActions inUnit)
    //{
    //    if (!CivilizationUnits.Contains(inUnit))
    //    {
    //        CivilizationUnits.Add(inUnit);
    //    }
    //}

    //#endregion

    //#region Resources

    //private void CopyResourcesFromCivilization()
    //{
    //    m_CurrentCivilizationResources = new QuantityOfResources[CurrentCivilizationSO.ResourcesValuesDictionary.Count];
    //    CurrentCivilizationSO.ResourcesValuesDictionary.Values.CopyTo(m_CurrentCivilizationResources, 0);
    //}

    //public bool CheckResourcesAvailability(QuantityOfResources[] ResourcesToCheck)
    //{
    //    int CivilizationHaveThatResource = 0;

    //    for (int i = 0; i < ResourcesToCheck.Length; i++)
    //    {
    //        for (int j = 0; j < m_CurrentCivilizationResources.Length; j++)
    //        {
    //            if (ResourcesToCheck[i].ResourceType == m_CurrentCivilizationResources[j].ResourceType)
    //            {
    //                if (m_CurrentCivilizationResources[j].ResourceQuantity >= ResourcesToCheck[i].ResourceQuantity)
    //                {
    //                    CivilizationHaveThatResource++;
    //                }
    //            }
    //        }
    //    }
    //    if (CivilizationHaveThatResource >= ResourcesToCheck.Length)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    //public void DecreaseResources(QuantityOfResources[] QuantityOfResources)
    //{
    //    for (int i = 0; i < QuantityOfResources.Length; i++)
    //    {
    //        for (int j = 0; j < m_CurrentCivilizationResources.Length; j++)
    //        {
    //            if (QuantityOfResources[i].ResourceType == m_CurrentCivilizationResources[j].ResourceType)
    //            {
    //                m_CurrentCivilizationResources[j].ResourceQuantity -= QuantityOfResources[i].ResourceQuantity;
    //            }
    //        }
    //    }
    //}

    //public void AddResources(QuantityOfResources[] QuantityOfResources)
    //{
    //    for (int i = 0; i < QuantityOfResources.Length; i++)
    //    {
    //        for (int j = 0; j < m_CurrentCivilizationResources.Length; j++)
    //        {
    //            if (QuantityOfResources[i].ResourceType == m_CurrentCivilizationResources[j].ResourceType)
    //            {
    //                m_CurrentCivilizationResources[j].ResourceQuantity += QuantityOfResources[i].ResourceQuantity;
    //                QuantityOfResources[i].ResourceQuantity = 0;
    //            }
    //        }
    //    }
    //}

    //#endregion

}
