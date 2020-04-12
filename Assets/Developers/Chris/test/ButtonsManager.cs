//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class ButtonsManager : MonoBehaviour
//{
//    public static ButtonsManager Instance;
//    [SerializeField] private GameObject CivilizationManager;
//    [SerializeField] private GameObject MouseManagerToSelectUnits;

//    [SerializeField] private GameObject m_FoundationsBuildingPrefab;

    
//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    public void ChooseCivilization(CivilizationStatistics civilizationToEquip)
//    {
//        GameObject Civilization = Instantiate(CivilizationManager) as GameObject;
//        Civilization.transform.name = civilizationToEquip.name + "Manager";
        
//        CivilizationScriptBehaviour.Instance.CivilizationSO = ScriptableObject.Instantiate(civilizationToEquip);
//        GameObject MouseSelectionManager = Instantiate(MouseManagerToSelectUnits);
//        UIManager.Instance.InstantiateCivilizationButton();
//        UIManager.Instance.ActivateOrDeactivateObjectUI(UIManager.Instance.PanelToChooseCivilization, false);
//    }

//    public void CreateNewUnit(UnitStatistics SOUnitType)
//    {
//        if (CivilizationScriptBehaviour.Instance.CivilizationSO.NumberOfPopulation + SOUnitType.PopolationsValue <= CivilizationScriptBehaviour.Instance.CurrentCivilizationMaxPopulation)
//        {
//            if (CivilizationScriptBehaviour.Instance.CheckResourcesAvailability(CivilizationScriptBehaviour.Instance.CivilizationSO.UnitsDictionary[SOUnitType.UnitType].UnitCost))
//            {
//                CivilizationScriptBehaviour.Instance.DecreaseResources(CivilizationScriptBehaviour.Instance.CivilizationSO.UnitsDictionary[SOUnitType.UnitType].UnitCost);
//                GameObject BuildingReference = MouseSelectionManager.Instance.m_BuildingRef.gameObject;

//                if (SOUnitType != null && BuildingReference != null)
//                {
//                    MouseSelectionManager.Instance.m_BuildingRef.AddUnitToList(SOUnitType.UnitType);
//                }
//            }
//        }       
//    }

//    public void CreateNewBuilding(BuildingsStatistics SOBuildingType)
//    {
//        if (CivilizationScriptBehaviour.Instance.CheckResourcesAvailability(CivilizationScriptBehaviour.Instance.CivilizationSO.BuildingsDictionary[SOBuildingType.BuildingType].BuildingCost))
//        {
//            m_FoundationsBuildingPrefab.transform.localScale = CivilizationScriptBehaviour.Instance.CivilizationSO.BuildingsDictionary[SOBuildingType.BuildingType].BuildingPrefab.transform.localScale;
//            m_FoundationsBuildingPrefab.GetComponent<MeshFilter>().sharedMesh = CivilizationScriptBehaviour.Instance.CivilizationSO.BuildingFoundationsCivilizationMesh;
//            m_FoundationsBuildingPrefab.transform.name = "Foundations" + CivilizationScriptBehaviour.Instance.CivilizationSO.BuildingsDictionary[SOBuildingType.BuildingType].BuildingStatsCopy.BuildingName;
//            m_FoundationsBuildingPrefab.gameObject.layer = CivilizationScriptBehaviour.Instance.m_ShiftedCivilizationLayer;

//            MouseSelectionManager.Instance.FillCurrentFoundationBuildingReference(m_FoundationsBuildingPrefab, CivilizationScriptBehaviour.Instance.CivilizationSO.BuildingsDictionary[SOBuildingType.BuildingType].BuildingStatsCopy.BuildingType);
//        }
//    }

//    public void ActivateCreateBuildingPanel()
//    {
//        if (!UIManager.Instance.CreateBuildingsPanel.gameObject.active)
//        {
//            UIManager.Instance.ActivateOrDeactivateObjectUI(UIManager.Instance.CreateBuildingsPanel, true);
//        }
//        else
//        {
//            UIManager.Instance.ActivateOrDeactivateObjectUI(UIManager.Instance.CreateBuildingsPanel, false);
//        }
        
//    }

//    public void DismissUnitBuilding()
//    {
//        if (MouseSelectionManager.Instance.CurrentSelectedObject.GetComponent<UnitsScriptBehaviour>())
//        {
//            MouseSelectionManager.Instance.CurrentSelectedObject.GetComponent<UnitsScriptBehaviour>().Death();            
//        }
//        else if (MouseSelectionManager.Instance.CurrentSelectedObject.GetComponent<BuildingsScriptBehaviour>())
//        {
//            MouseSelectionManager.Instance.CurrentSelectedObject.GetComponent<BuildingsScriptBehaviour>().Death();
//        }
//        else
//        {
//            Destroy(MouseSelectionManager.Instance.CurrentSelectedObject);
//        }
        
//        UIManager.Instance.DeactivateAllPanels();
//        MouseSelectionManager.Instance.CurrentSelectedObject = null;
//        MouseSelectionManager.Instance.ClearSelection();
//    }

//    public void Upgrade(UpgradeStatistics SOUpgrade)
//    {
        
//        if (CivilizationScriptBehaviour.Instance.CheckResourcesAvailability(SOUpgrade.UpgradeCost))
//        {
//            CivilizationScriptBehaviour.Instance.DecreaseResources(SOUpgrade.UpgradeCost);
            
//            if(MouseSelectionManager.Instance.m_BuildingRef.GetComponent<BuildingsScriptBehaviour>() != null)
//            {
//                BuildingsScriptBehaviour RefToBuildings = MouseSelectionManager.Instance.m_BuildingRef.GetComponent<BuildingsScriptBehaviour>();
//                if(RefToBuildings.RefToUpgrade == null)
//                {
//                    RefToBuildings.AssignUpgradeToDo(SOUpgrade);
//                }
//            }
            
//        }
//    }


//}
