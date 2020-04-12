//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;



//public class WorkerActions : MeleeActions
//{
//    public enum WorkerAction
//    {
//        None,
//        RepairOrBuild,
//        Collect,
        
//    }

//    public WorkerAction m_CurrentWorkerState { get; private set; }

//    private GameObject m_ResourceAssigned = null;
//    public GameObject ResourceAssigned
//    {
//        get
//        {
//            return m_ResourceAssigned;
//        }
//        set
//        {
//            m_ResourceAssigned = value;
//        }
//    }
    
//    public int MaxQuantityOfResourcesCollected = 10;
//    private int QuantityOfResourcesCollected = 0;


//    public float CollectSpeed = 1;

//    public int CollectQuantity = 1;
   

    
//    private QuantityOfResources[] m_ResourcesCollected;
//    public QuantityOfResources[] ResourcesCollected
//    {
//        get
//        {
//            return m_ResourcesCollected;
//        }
//        set
//        {
//            m_ResourcesCollected = value;
//        }
//    }

//    private IRepairable m_CanBeRepaired;

//    public int RepairOrBuildAdditionalValue = 1;


//    // Start is called before the first frame update
//    public override void Start()
//    {
//        base.Start();
//        m_CurrentWorkerState = WorkerAction.None;
//        InitializeBagToCollectResource();
        
//    }
    
//    // Update is called once per frame
//    public override void Update()
//    {
//        CheckWorkerActionsConsideringDistance();
        
//    }




//    private void CheckWorkerActionsConsideringDistance()
//    {
//        if (CheckFocussedObjectDistance())
//        {
//            if (FocusObject.layer != gameObject.layer)
//            {
//                if (!FocusObject.GetComponent<ResourceScriptBehaviour>())
//                {
//                    base.Update();
//                }
//                else if (FocusObject.GetComponent<ResourceScriptBehaviour>())
//                {
//                    CollectSystem();
//                    ChangeUnitState(Actions.Collect);
//                }
//            }
//            else if (FocusObject.layer == gameObject.layer)
//            {
//                if (FocusObject.GetComponent<DepositBuildingActions>())
//                {
//                    DepositResources();
//                    FocusObject = m_ResourceAssigned;
//                }
//                else if (FocusObject.GetComponent<BuildingFoundationsActions>())
//                {
//                    if (FocusObject.GetComponent<BuildingFoundationsActions>().m_CanBeRepaired)
//                    {
//                        RepairOrBuild();
//                    }
                    
//                }
//                else if (FocusObject.GetComponent<SheepActions>())
//                {
//                    base.Update();
//                }
//            }
//        }

//        if (!m_CanAttack)
//        {
//            m_CanAttack = Timer(UnitStatisticsSO.TimeToAttack);
//        }

//        if (FocusObject != null)
//        {
//            gameObject.transform.LookAt(new Vector3(FocusObject.transform.position.x, gameObject.transform.position.y, FocusObject.transform.position.z));
//        }
//    }

//    protected void RepairOrBuild()
//    {
//        ChangeUnitState(Actions.Repair);
//        m_CanBeRepaired = FocusObject.GetComponent<IRepairable>() as IRepairable;
//        if (m_CanBeRepaired != null && Timer(1))
//        {
//            m_CanBeRepaired.RepairOrBuild(RepairOrBuildAdditionalValue);
//        }
//    }
    
//    protected void CollectSystem()
//    {
//        if(m_ResourceAssigned == null)
//        {
//            m_ResourceAssigned = FocusObject;
//        }
        
//        CanTakeDamage = FocusObject.GetComponent<IDamageable>() as IDamageable;

//        if (QuantityOfResourcesCollected < MaxQuantityOfResourcesCollected)
//        {
//            if (CanTakeDamage != null && Timer(CollectSpeed))
//            {
//                CollectResource(FocusObject.GetComponent<ResourceScriptBehaviour>().TypeOfResource);
//                CanTakeDamage.TakeDamage(CollectQuantity);
//            }
//        }
//        else
//        {
//            FocusObject = FocusObject.GetComponent<ResourceScriptBehaviour>().ResourceDeposit;
//        }
//    }

//    private void DepositResources()
//    {
//        CivilizationScriptBehaviour.Instance.AddResources(ResourcesCollected);
//        QuantityOfResourcesCollected = 0;
//    }

//    private void CollectResource(Materials TypeOfResource)
//    {
//        for (int i = 0; i < ResourcesCollected.Length; i++)
//        {
//            if (ResourcesCollected[i].ResourceType == TypeOfResource)
//            {
//                ResourcesCollected[i].ResourceQuantity += CollectQuantity;
//                QuantityOfResourcesCollected += CollectQuantity;
//            }
//        }
//    }
    
    
//    private void InitializeBagToCollectResource()
//    {
//        ResourcesCollected = new QuantityOfResources[CivilizationScriptBehaviour.Instance.CivilizationSO.ResourcesValuesDictionary.Count];

//        for (int i = 0; i < ResourcesCollected.Length; i++)
//        {
//            ResourcesCollected[i] = CivilizationScriptBehaviour.Instance.CivilizationSO.CivilizationQuantityResources[i];
//            ResourcesCollected[i].ResourceQuantity = 0;
//        }
//    }

//    public void ChangeMaxStoredResources(int NewValue)
//    {
//        MaxQuantityOfResourcesCollected += NewValue;
//    }

//    public void ChangeSpeedToCollect(int NewValue)
//    {
//        CollectSpeed += NewValue;
//    }

//    public void ChangeCollectedResourceEachHit(int NewValue)
//    {
//        CollectQuantity += NewValue;
//    }

//    public void ChangeAmountOfLifeGivenEachHit(int NewValue)
//    {
//        RepairOrBuildAdditionalValue += NewValue;
//    }
//}
