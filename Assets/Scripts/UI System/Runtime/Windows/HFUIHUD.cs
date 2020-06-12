using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace HF.Refactoring
{
    public class HFUIHUD : HFUIWindow
    {
        public override HFUIWindowID ID => HFUIWindowID.HUD;

        [Header("Tutorial Field")]
        public HFTutorialPopUp Popup;


        [Header("Generic buttons")]
        /// <summary>
        /// "Call next wave" button.
        /// </summary>
        public HFButton ButtonCallNextWave;

        [Header("Specialization")]
        public GameObject UnitSelectedContainer;

        [Tooltip("Those icons will be shown when a troop is selected")]
        public Sprite[] TroopIconsSpecializations;
        [Tooltip("Those icons will be shown when a building is selected")]
        public Sprite[] BuildingIconsSpecializations;

        [Header("ID Field")]
        public Text TextDescription;

        public Image EntitySelectedIcon;

        public HFSpecializationB[] SpecializationButtons;
        public SlidingSpecializationBar SpecializationBar;
        public HFSpecializationB UpgradeButton;

        [Header("Castle input field")]
        public GameObject CastleCommandContainer;
        public HFSpecializationB SpawnTroopButton;

        private void OnEnable()
        {
            HFEventManager.SubscribeTo(HFEventID.OnWaveBeginned, OnNewWaveBegin);
            HFEventManager.SubscribeTo(HFEventID.OnWaveCleared, OnWaveCleared);
            HFEventManager.SubscribeTo<EntityBehavior, int>(HFEventID.OnUnitSelected, OnUnitSelected);
            HFEventManager.SubscribeTo<EntityBehavior, int>(HFEventID.OnUnitSpecialized, OnUnitSpecialized);
            HFEventManager.SubscribeTo<EntityBehavior>(HFEventID.OnUnitFight, OnUnitFight);

            ButtonCallNextWave.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            HFEventManager.UnsubscribeFrom(HFEventID.OnWaveBeginned, OnNewWaveBegin);
            HFEventManager.UnsubscribeFrom(HFEventID.OnWaveCleared, OnWaveCleared);
            HFEventManager.UnsubscribeFrom<EntityBehavior, int>(HFEventID.OnUnitSelected, OnUnitSelected);
            HFEventManager.UnsubscribeFrom<EntityBehavior, int>(HFEventID.OnUnitSpecialized, OnUnitSpecialized);
            HFEventManager.UnsubscribeFrom<EntityBehavior>(HFEventID.OnUnitFight, OnUnitFight);

            Popup.gameObject.SetActive(false);
        }

        #region Events

        //--------------------------------------------------------
        // Event trigerred or listened from wave controller
        //--------------------------------------------------------

        private void OnNewWaveBegin()
        {
            ButtonCallNextWave.gameObject.SetActive(false);
        }

        private void OnWaveCleared()
        {
            ButtonCallNextWave.gameObject.SetActive(true);
        }

        private void OnUnitSelected(EntityBehavior entity, int team)
        {

            if (entity == null)
            {
                UnitSelectedContainer.SetActive(false);
                return;
            }

            if (entity.gameObject.layer == GameController.Instance.m_playerLayer)
            {
                //if (entity.IsBusy)
                //{
                //    UnitSelectedContainer.SetActive(false);
                //    return;
                //}

                SetUpSpecializationButton(entity);
               
                UnitSelectedContainer.transform.localScale = Vector3.zero;
                UnitSelectedContainer.transform.DOScale(1, 0.2f);
            }
        }

        private void OnUnitSpecialized(EntityBehavior entity, int team)
        {
            OnUnitSelected(entity, 0);
        }

        private void OnUnitFight(EntityBehavior entity)
        {
            if (entity == InputReaderManager.Instance.CurrentEntity)
            {
                UnitSelectedContainer.SetActive(false);
            }
        }

        #endregion

        public void ReturnToLevelSelection()
        {
            OnUnitSelected(null as EntityBehavior, 0);
            HFGameManager.Instance.ChangeGMState(GameStates.Pause);
            HFUIManager.Instance.ShowAndClearHistory(HFUIWindowID.WAR_ROOM);
            HFScenesManager.Instance.LoadSceneFromIndex(1);
        }

        public void WinLevel()
        {
            HFScenesManager.Instance.EndCurrentLevel(true);
            HFUIManager.Instance.ShowAndClearHistory(HFUIWindowID.WAR_ROOM);
            HFScenesManager.Instance.LoadSceneFromIndex(1);
        }

        public void SetUpSpecializationButton(EntityBehavior entity)
        {
            for (int i = 0; i < SpecializationButtons.Length; i++)
            {
                SpecializationButtons[i].RemoveAllListeners();
                SpecializationButtons[i].gameObject.SetActive(false);
            }
            SpawnTroopButton.ButtonComponent.onClick.RemoveAllListeners();

            CastleCommandContainer.gameObject.SetActive(false);
            UpgradeButton.gameObject.SetActive(false);
            SpecializationBar.gameObject.SetActive(false);
            UnitSelectedContainer.gameObject.SetActive(false);

            if (entity is Troop)
            {
                UnitSelectedContainer.gameObject.SetActive(true);

                // get typed entity
                var typedEntity = entity as Troop;

                // Set the icon on hud info
                EntitySelectedIcon.sprite = typedEntity.m_troopStats.Icon;

                // Set description, if the string is null then show a default one.
                TextDescription.text = typedEntity.GetStats().OutputStringDescription;

                if (typedEntity.m_troopStats.UnitType == Types.UnitType.STANDARD_ALLY)
                {
                    for (int i = 0; i < SpecializationButtons.Length; i++)
                    {
                        SpecializationButtons[i].Icon.sprite = TroopIconsSpecializations[i];
                        SpecializationButtons[i].gameObject.SetActive(true);
                    }
                    SpecializationBar.gameObject.SetActive(true);
                    SpecializationButtons[0].ButtonComponent.onClick.AddListener(() => entity.Specialization(Types.UnitType.DEFENDER_LVL1));
                    SpecializationButtons[1].ButtonComponent.onClick.AddListener(() => entity.Specialization(Types.UnitType.LIFTER_LVL1));
                    SpecializationButtons[2].ButtonComponent.onClick.AddListener(() => entity.Specialization(Types.UnitType.RUNNER_LVL1));

                    SpecializationButtons[0].Cost.text = GameController.Instance.Collection.UnitsDictionary[Types.UnitType.DEFENDER_LVL1].OriginalUnitStats.Cost.ToString();
                    SpecializationButtons[1].Cost.text = GameController.Instance.Collection.UnitsDictionary[Types.UnitType.LIFTER_LVL1].OriginalUnitStats.Cost.ToString();
                    SpecializationButtons[2].Cost.text = GameController.Instance.Collection.UnitsDictionary[Types.UnitType.RUNNER_LVL1].OriginalUnitStats.Cost.ToString();
                }
                else
                {
                    UpgradeButton.RemoveAllListeners();
                    if (typedEntity.m_troopStats.UnitType == Types.UnitType.DEFENDER_LVL3 ||
                        typedEntity.m_troopStats.UnitType == Types.UnitType.LIFTER_LVL3 ||
                        typedEntity.m_troopStats.UnitType == Types.UnitType.RUNNER_LVL3)
                    {
                        UpgradeButton.Icon.sprite = null;
                    }
                    else
                    {
                        UpgradeButton.gameObject.SetActive(true);
                        UpgradeButton.Icon.sprite = GameController.Instance.GetIcon(typedEntity.m_troopStats.UnitType + 1);
                        UpgradeButton.ButtonComponent.onClick.AddListener(() => typedEntity.Specialization(typedEntity.m_troopStats.UnitType + 1));

                        UpgradeButton.Cost.text = GameController.Instance.Collection.UnitsDictionary[typedEntity.m_troopStats.UnitType + 1].UnitStatsCopy.Cost.ToString();
                    }
                }
            }
            else if (entity is BuildingBehaviour)
            {
                var typedEntity = entity as BuildingBehaviour;
                EntitySelectedIcon.sprite = typedEntity.m_buildingStats.Icon;

                // Set description, if the string is null then show a default one.
                TextDescription.text = typedEntity.GetStats().OutputStringDescription;

                if (typedEntity.m_buildingStats.BuildingType == Types.BuildingType.CASTLE)
                {
                    CastleCommandContainer.gameObject.SetActive(true);
                    SpawnTroopButton.Icon.sprite = GameController.Instance.Collection.UnitsDictionary[Types.UnitType.STANDARD_ALLY].OriginalUnitStats.Icon;
                    SpawnTroopButton.Cost.text = GameController.Instance.Collection.UnitsDictionary[Types.UnitType.STANDARD_ALLY].OriginalUnitStats.Cost.ToString();

                    CastleStarter castle = typedEntity.GetComponent<CastleStarter>();
                    SpawnTroopButton.ButtonComponent.onClick.AddListener(() => castle.SpawnTroop());
                    //SpecializationBar.gameObject.SetActive(true);
                    return;
                }

                UnitSelectedContainer.gameObject.SetActive(true);
                if (typedEntity.m_buildingStats.BuildingType == Types.BuildingType.TOWER)
                {

                    for (int i = 0; i < SpecializationButtons.Length; i++)
                    {
                        SpecializationButtons[i].Icon.sprite = BuildingIconsSpecializations[i];
                        SpecializationButtons[i].gameObject.SetActive(true);
                    }
                    SpecializationBar.gameObject.SetActive(true);
                    SpecializationButtons[0].ButtonComponent.onClick.AddListener(() => entity.Specialization(Types.BuildingType.BALLISTA_LVL1));
                    SpecializationButtons[1].ButtonComponent.onClick.AddListener(() => entity.Specialization(Types.BuildingType.CRYSTAL_LVL1));
                    SpecializationButtons[2].ButtonComponent.onClick.AddListener(() => entity.Specialization(Types.BuildingType.MORTAR_LVL1));

                    SpecializationButtons[0].Cost.text = GameController.Instance.Collection.BuildingsDictionary[Types.BuildingType.BALLISTA_LVL1].OriginalBuildingStats.Cost.ToString();
                    SpecializationButtons[1].Cost.text = GameController.Instance.Collection.BuildingsDictionary[Types.BuildingType.CRYSTAL_LVL1].OriginalBuildingStats.Cost.ToString();
                    SpecializationButtons[2].Cost.text = GameController.Instance.Collection.BuildingsDictionary[Types.BuildingType.MORTAR_LVL1].OriginalBuildingStats.Cost.ToString();
                }
                else
                {
                    UpgradeButton.RemoveAllListeners();
                    if (typedEntity.m_buildingStats.BuildingType == Types.BuildingType.CRYSTAL_LVL3 ||
                        typedEntity.m_buildingStats.BuildingType == Types.BuildingType.MORTAR_LVL3 ||
                        typedEntity.m_buildingStats.BuildingType == Types.BuildingType.BALLISTA_LVL3)
                    {
                        UpgradeButton.Icon.sprite = null;
                    }
                    else
                    {
                        UpgradeButton.gameObject.SetActive(true);
                        UpgradeButton.Icon.sprite = GameController.Instance.GetIcon(typedEntity.m_buildingStats.BuildingType + 1);
                        UpgradeButton.ButtonComponent.onClick.AddListener(() => typedEntity.Specialization(typedEntity.m_buildingStats.BuildingType + 1));

                        UpgradeButton.Cost.text = GameController.Instance.Collection.BuildingsDictionary[typedEntity.m_buildingStats.BuildingType + 1].OriginalBuildingStats.Cost.ToString();
                    }
                }
            }
        }
    }
}
