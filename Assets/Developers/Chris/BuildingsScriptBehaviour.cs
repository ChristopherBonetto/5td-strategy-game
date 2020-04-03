//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.AI;

//public class BuildingsScriptBehaviour : MonoBehaviour, IDamageable, ISelectionable, IRepairable
//{
//    private BuildingsStatistics m_BuildingStatisticsSO;
//    public BuildingsStatistics BuildingStatisticsSO
//    {
//        get
//        {           
//            return m_BuildingStatisticsSO;
//        }
//        set
//        {
//            m_BuildingStatisticsSO = value;
//        }
//    }

//    public IShowSelectionableInfo m_BuildingShowSelectionableInfo { get; protected set; }

//    protected int m_BuildingCurrentHp;
//    public bool m_CanBeRepaired { get; protected set; } = false;
//    protected List<GameObject> m_WorkersAssignedList = new List<GameObject>();

//    public List<Units> CreateUnitsList { get; private set; } = new List<Units>();

//    [SerializeField] protected bool m_CanCreateUnit;
//    [SerializeField] protected GameObject SpawnPoint;
//    public bool m_FindSpawnPointPosition { get; protected set; } = false;
//    protected Vector3 m_OffSetToInstantiate;

//    protected Renderer m_NewSpawnPointRendered;

//    public UpgradeStatistics RefToUpgrade { get; protected set; }

//    public virtual void Awake()
//    {
//        m_BuildingShowSelectionableInfo = new ShowSelectedObject();
//        if (SpawnPoint != null)
//        {
//            m_CanCreateUnit = true;
//        }

//    }

//    // Start is called before the first frame update
//    public virtual void Start()
//    {
//        UpdateCurrentHp();
//        CivilizationScriptBehaviour.Instance.CreatedBuildings.Add(this.gameObject);
//        InstantiateSpawnPoint();

//    }

//    // Update is called once per frame
//    public virtual void Update()
//    {
//        CheckUnitQueue();
//        UpgradeToDo();
//        RandomNavmeshLocation("Walkable");
        
//    }



//    public void UpdateCurrentHp()
//    {
//        m_BuildingCurrentHp = BuildingStatisticsSO.HealthMax;
//    }

//    public void ShowHideSpawnPoint()
//    {
//        if (m_NewSpawnPointRendered != null)
//        {
//            if (MouseSelectionManager.Instance.CurrentSelectedObject == this.gameObject && m_FindSpawnPointPosition == false)
//            {
//                if (m_NewSpawnPointRendered.enabled)
//                {
//                    m_NewSpawnPointRendered.enabled = false;
//                }
//                else
//                {
//                    m_NewSpawnPointRendered.enabled = true;
//                }

//            }
//        }

//    }

//    public virtual void InstantiateSpawnPoint()
//    {

//        if (m_CanCreateUnit)
//        {
//            m_NewSpawnPointRendered = SpawnPoint.GetComponent<Renderer>();
//            m_NewSpawnPointRendered.enabled = false;
//            m_FindSpawnPointPosition = true;
//        }
//    }



//    public virtual void RandomNavmeshLocation(string AreaName)
//    {
//        if (m_FindSpawnPointPosition)
//        {
//            NavMeshHit hit;
//            int RightMask = 1 << NavMesh.GetAreaFromName(AreaName);

//            float radius = gameObject.transform.localScale.x + 4f;
//            Vector3 randomDirection = Random.insideUnitSphere * radius;
//            randomDirection += transform.position;


//            if (NavMesh.SamplePosition(randomDirection, out hit, radius, RightMask))
//            {
//                if (hit.distance >= gameObject.transform.localScale.x + 1f)
//                {
//                    SpawnPoint.transform.position = hit.position;
//                    ShowHideSpawnPoint();
//                    m_FindSpawnPointPosition = false;
//                }
//            }
//        }
//    }

//    public virtual void CheckUnitQueue()
//    {
//        if (CreateUnitsList.Count > 0 && m_FindSpawnPointPosition == false)
//        {
//            if (CivilizationScriptBehaviour.Instance.CivilizationSO.NumberOfPopulation + CivilizationScriptBehaviour.Instance.CivilizationSO.UnitsDictionary[CreateUnitsList[0]].UnitStatsCopy.PopolationsValue <= CivilizationScriptBehaviour.Instance.CurrentCivilizationMaxPopulation)
//            {
//                if (CivilizationScriptBehaviour.Instance.Timer(CivilizationScriptBehaviour.Instance.CivilizationSO.UnitsDictionary[CreateUnitsList[0]].UnitStatsCopy.TimeOfInstantiate))
//                {
//                    InstantiateUnit(CreateUnitsList[0]);
//                    CreateUnitsList.Remove(CreateUnitsList[0]);
//                }
//            }
//        }

//    }

//    public void AddUnitToList(Units NewUnitType)
//    {
//        if (CivilizationScriptBehaviour.Instance.CivilizationSO.UnitsDictionary != null)
//        {
//            CreateUnitsList.Add(NewUnitType);
//        }
//    }

//    public void AddWorkerToList(GameObject Worker)
//    {
//        if (!m_WorkersAssignedList.Contains(Worker))
//        {
//            m_WorkersAssignedList.Add(Worker);
//        }
//    }

//    protected void InstantiateUnit(Units UnitType)
//    {
//        if (CivilizationScriptBehaviour.Instance.CivilizationSO.UnitsDictionary != null)
//        {
//            GameObject unit = Instantiate(CivilizationScriptBehaviour.Instance.CivilizationSO.UnitsDictionary[UnitType].UnitPrefab, SpawnPoint.transform.position, Quaternion.identity) as GameObject;
//            unit.transform.name = CivilizationScriptBehaviour.Instance.CivilizationSO.UnitsDictionary[UnitType].UnitName;
//            CivilizationScriptBehaviour.Instance.AddRemoveCivilizationValue(CivilizationScriptBehaviour.Instance.CivilizationSO.UnitsDictionary[UnitType].UnitStatsCopy.PopolationsValue);
//            UIManager.Instance.RefreshPopulation();

//            unit.GetComponent<UnitsScriptBehaviour>().UnitStatisticsSO = CivilizationScriptBehaviour.Instance.CivilizationSO.UnitsDictionary[UnitType].UnitStatsCopy;

//            CivilizationScriptBehaviour.Instance.CreatedUnits.Add(unit);

            
//            unit.layer = CivilizationScriptBehaviour.Instance.GetGameObjectLayer(CivilizationScriptBehaviour.Instance.CivilizationSO.CivilizationLayer);
//        }

//    }

//    public void AssignUpgradeToDo(UpgradeStatistics NewUpgrade)
//    {
//        RefToUpgrade = NewUpgrade;
//    }

//    public void UpgradeToDo()
//    {
//        if (RefToUpgrade == null) return;

//        if (CivilizationScriptBehaviour.Instance.Timer(RefToUpgrade.TimeToUpgrade))
//        {
//            if (RefToUpgrade.UnitsToUpgrade.Length > 0)
//            {
//                UpgradeUnitsStats();
//            }
//            if (RefToUpgrade.BuildingsToUpgrade.Length > 0)
//            {
//                UpgradeBuildingsStats();
//            }
//            if (RefToUpgrade.WorkerUpgrade.CanUpgradeWorker)
//            {
//                UpgradeWorkerStats(Units.Worker);
//            }

//            if (MouseSelectionManager.Instance.CurrentSelectedObject.GetComponent<UnitsScriptBehaviour>())
//            {
//                MouseSelectionManager.Instance.CurrentSelectedObject.GetComponent<UnitsScriptBehaviour>().ShowInfoPanels();
//            }
//            if (MouseSelectionManager.Instance.CurrentSelectedObject.GetComponent<BuildingsScriptBehaviour>())
//            {
//                MouseSelectionManager.Instance.CurrentSelectedObject.GetComponent<BuildingsScriptBehaviour>().ShowInfoPanels();
//            }
//            RefToUpgrade = null;
//        }
//    }

//    public void UpgradeUnitsStats()
//    {
//        for (int i = 0; i < RefToUpgrade.UnitsToUpgrade.Length; i++)
//        {
//            if (CivilizationScriptBehaviour.Instance.CivilizationSO.UnitsDictionary.ContainsKey(RefToUpgrade.UnitsToUpgrade[i].TypeOfUnitToUpgrade))
//            {
//                CivilizationScriptBehaviour.Instance.CivilizationSO.UnitsDictionary[RefToUpgrade.UnitsToUpgrade[i].TypeOfUnitToUpgrade].UnitStatsCopy.Attack += RefToUpgrade.UnitsToUpgrade[i].IncreaseAttack;
//                CivilizationScriptBehaviour.Instance.CivilizationSO.UnitsDictionary[RefToUpgrade.UnitsToUpgrade[i].TypeOfUnitToUpgrade].UnitStatsCopy.Defence += RefToUpgrade.UnitsToUpgrade[i].IncreaseDefence;
//                CivilizationScriptBehaviour.Instance.CivilizationSO.UnitsDictionary[RefToUpgrade.UnitsToUpgrade[i].TypeOfUnitToUpgrade].UnitStatsCopy.MovementSpeed += RefToUpgrade.UnitsToUpgrade[i].IncreaseMovementSpeed;
//                CivilizationScriptBehaviour.Instance.CivilizationSO.UnitsDictionary[RefToUpgrade.UnitsToUpgrade[i].TypeOfUnitToUpgrade].UnitStatsCopy.ViewRadius += RefToUpgrade.UnitsToUpgrade[i].IncreaseViewRadius;
//                CivilizationScriptBehaviour.Instance.CivilizationSO.UnitsDictionary[RefToUpgrade.UnitsToUpgrade[i].TypeOfUnitToUpgrade].UnitStatsCopy.TimeOfInstantiate -= RefToUpgrade.UnitsToUpgrade[i].DecreaseTimeToInstantiate;
//                CivilizationScriptBehaviour.Instance.CivilizationSO.UnitsDictionary[RefToUpgrade.UnitsToUpgrade[i].TypeOfUnitToUpgrade].UnitStatsCopy.TimeToAttack -= RefToUpgrade.UnitsToUpgrade[i].DecreaseTimeToAttack;

//                if(RefToUpgrade.UnitsToUpgrade[i].IncreaseHealthMax > 0)
//                {
//                    CivilizationScriptBehaviour.Instance.CivilizationSO.UnitsDictionary[RefToUpgrade.UnitsToUpgrade[i].TypeOfUnitToUpgrade].UnitStatsCopy.HealthMax += RefToUpgrade.UnitsToUpgrade[i].IncreaseHealthMax;
//                    UpgradeUnitHp(RefToUpgrade.UnitsToUpgrade[i].TypeOfUnitToUpgrade);
//                }

//                if(RefToUpgrade.UnitsToUpgrade[i].IncreaseBulletSpeed > 0)
//                {
//                    UpgradeUnitArrowSpeed(RefToUpgrade.UnitsToUpgrade[i].TypeOfUnitToUpgrade, RefToUpgrade.UnitsToUpgrade[i].IncreaseBulletSpeed);
//                }

                
//            }
            
//        }
        
//    }

//    public void UpgradeUnitHp(Units UnitType)
//    {
//        foreach(GameObject unit in CivilizationScriptBehaviour.Instance.CreatedUnits)
//        {
//            UnitsScriptBehaviour UnitRef = unit.GetComponent<UnitsScriptBehaviour>();
//            if(UnitRef.UnitStatisticsSO.UnitType == UnitType)
//            {
//                UnitRef.UpdateCurrentHp();
//            }
//        }
//    }

//    public void UpgradeUnitArrowSpeed(Units UnitType, float ValueToAdd)
//    {
//        foreach (GameObject unit in CivilizationScriptBehaviour.Instance.CreatedUnits)
//        {
//            if (unit.GetComponent<RangedActions>())
//            {
//                RangedActions UnitRef = unit.GetComponent<RangedActions>();

//                if (UnitRef.UnitStatisticsSO.UnitType == UnitType)
//                {
//                    UnitRef.BulletSpeed += ValueToAdd;
//                }
//            }
//        }
//    }

//    public void UpgradeBuildingHp(Buildings BuildingType)
//    {
//        foreach(GameObject building in CivilizationScriptBehaviour.Instance.CreatedBuildings)
//        {
//            BuildingsScriptBehaviour BuildingRef = building.GetComponent<BuildingsScriptBehaviour>();
//            if(BuildingRef.BuildingStatisticsSO.BuildingType == BuildingType)
//            {
//                BuildingRef.UpdateCurrentHp();
//            }
//        }
//    }

//    public void UpgradeBuildingsStats()
//    {
//        for (int i = 0; i < RefToUpgrade.BuildingsToUpgrade.Length; i++)
//        {
//            if (CivilizationScriptBehaviour.Instance.CivilizationSO.BuildingsDictionary.ContainsKey(RefToUpgrade.BuildingsToUpgrade[i].TypeOfBuildingToUpgrade))
//            {
//                CivilizationScriptBehaviour.Instance.CivilizationSO.BuildingsDictionary[RefToUpgrade.BuildingsToUpgrade[i].TypeOfBuildingToUpgrade].BuildingStatsCopy.TimeOfCostruction -= RefToUpgrade.BuildingsToUpgrade[i].DecreaseTimeToBuild;
//                CivilizationScriptBehaviour.Instance.CivilizationSO.BuildingsDictionary[RefToUpgrade.BuildingsToUpgrade[i].TypeOfBuildingToUpgrade].BuildingStatsCopy.BuildingAttack += RefToUpgrade.BuildingsToUpgrade[i].IncreaseAttack;
//                CivilizationScriptBehaviour.Instance.CivilizationSO.BuildingsDictionary[RefToUpgrade.BuildingsToUpgrade[i].TypeOfBuildingToUpgrade].BuildingStatsCopy.Defence += RefToUpgrade.BuildingsToUpgrade[i].IncreaseDefence;
//                CivilizationScriptBehaviour.Instance.CivilizationSO.BuildingsDictionary[RefToUpgrade.BuildingsToUpgrade[i].TypeOfBuildingToUpgrade].BuildingStatsCopy.BuildingTimeToAttack -= RefToUpgrade.BuildingsToUpgrade[i].DecreaseTimeToAttack;
//                CivilizationScriptBehaviour.Instance.CivilizationSO.BuildingsDictionary[RefToUpgrade.BuildingsToUpgrade[i].TypeOfBuildingToUpgrade].BuildingStatsCopy.BuildingViewRadius += RefToUpgrade.BuildingsToUpgrade[i].IncreaseViewRadius;

//                if(RefToUpgrade.BuildingsToUpgrade[i].IncreaseHealthMax > 0)
//                {
//                    CivilizationScriptBehaviour.Instance.CivilizationSO.BuildingsDictionary[RefToUpgrade.BuildingsToUpgrade[i].TypeOfBuildingToUpgrade].BuildingStatsCopy.HealthMax += RefToUpgrade.BuildingsToUpgrade[i].IncreaseHealthMax;
//                    UpgradeBuildingHp(RefToUpgrade.BuildingsToUpgrade[i].TypeOfBuildingToUpgrade);
//                }
//            }
//        }
        
//    }

//    public void UpgradeWorkerStats(Units UnitType)
//    {
//        foreach (GameObject unit in CivilizationScriptBehaviour.Instance.CreatedUnits)
//        {
//            if (unit.GetComponent<WorkerActions>())
//            {
//                WorkerActions UnitRef = unit.GetComponent<WorkerActions>();

//                if (UnitRef.UnitStatisticsSO.UnitType == UnitType)
//                {
//                    UnitRef.ChangeAmountOfLifeGivenEachHit(RefToUpgrade.WorkerUpgrade.IncreaseRepairValue);
//                    UnitRef.ChangeCollectedResourceEachHit(RefToUpgrade.WorkerUpgrade.IncreaseResourcesCollected);
//                    UnitRef.ChangeMaxStoredResources(RefToUpgrade.WorkerUpgrade.IncreaseBagToStoreResources);
//                    UnitRef.ChangeSpeedToCollect(RefToUpgrade.WorkerUpgrade.IncreaseSpeedToCollect);                    
//                }
//            }
//        }
//    }


//    public virtual bool TakeDamage(int Damage)
//    {
//        Damage = Mathf.Clamp(Damage, 0, BuildingStatisticsSO.HealthMax + BuildingStatisticsSO.Defence);
//        m_CanBeRepaired = true;
//        if (m_BuildingCurrentHp <= Damage)
//        {
//            m_BuildingCurrentHp -= Damage;

//            Death();
//            return true;
//        }
//        else
//        {
//            m_BuildingCurrentHp -= Damage;
//            if (gameObject == MouseSelectionManager.Instance.CurrentSelectedObject)
//            {
//                ShowInfoPanels();
//            }
//            return false;
//        }
//    }

//    public virtual void Death()
//    {
//        foreach (GameObject Worker in m_WorkersAssignedList)
//        {
//            if (Worker.GetComponent<UnitsScriptBehaviour>().FocusObject == this.gameObject)
//            {
//                Worker.GetComponent<NavMeshAgent>().SetDestination(Worker.transform.position);
//                Worker.GetComponent<UnitsScriptBehaviour>().FocusObject = null;
//            }
//        }
//        m_WorkersAssignedList.Clear();
//        if (gameObject == MouseSelectionManager.Instance.CurrentSelectedObject)
//        {
//            MouseSelectionManager.Instance.ClearSelection();
//        }

//        Destroy(this.gameObject);
//    }

//    public virtual void ShowInfoPanels()
//    {
//        m_BuildingShowSelectionableInfo.ShowSelectionableInfo(gameObject.transform.name, m_BuildingCurrentHp, BuildingStatisticsSO.HealthMax, BuildingStatisticsSO.Defence, BuildingStatisticsSO.BuildingSprite, 0, 0, 0, 0, BuildingStatisticsSO.BuildingButtons);
//    }

//    public virtual void RepairOrBuild(int AddHealthValue)
//    {
//        if (m_CanBeRepaired && m_BuildingCurrentHp < BuildingStatisticsSO.HealthMax)
//        {
//            AddHealthValue = Mathf.Clamp(AddHealthValue, 0, BuildingStatisticsSO.HealthMax);
//            m_BuildingCurrentHp += AddHealthValue;

//            if (gameObject == MouseSelectionManager.Instance.CurrentSelectedObject)
//            {
//                ShowInfoPanels();
//            }
//        }
//        else if (m_BuildingCurrentHp >= BuildingStatisticsSO.HealthMax)
//        {
//            foreach (GameObject Worker in m_WorkersAssignedList)
//            {
//                if (Worker.GetComponent<UnitsScriptBehaviour>().FocusObject == this.gameObject)
//                {
//                    Worker.GetComponent<NavMeshAgent>().SetDestination(Worker.transform.position);
//                    Worker.GetComponent<UnitsScriptBehaviour>().FocusObject = null;
//                }
//            }
//            m_WorkersAssignedList.Clear();
//            m_CanBeRepaired = false;
//        }
//    }


//}
