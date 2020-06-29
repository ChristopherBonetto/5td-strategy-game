using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace HF.Refactoring
{
    public class HFUIHUD : HFUIWindow, IHFTutorial
    {
        public override HFUIWindowID ID => HFUIWindowID.HUD;

        [Header("Tutorial Field")]
        public HFTutorialPopUp Popup;

        public GameEventData Initialization;
        // Tutorial variables
        public bool m_isTutorial { get; private set; } = false;

        public TutorialID TutorialID { get; set; } = TutorialID.Upgrade_Unit;

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

        [Header("Error Messafe")]
        public HFUIMessage Message;

        [Header("Carry capacity")]
        public Slider carryCapacitySlider;

        [Header("Marker")]
        public HFUIEnemySpawnMarker Marker;

        private void OnEnable()
        {
            HFEventManager.SubscribeTo(HFEventID.OnWaveBeginned, OnNewWaveBegin);
            HFEventManager.SubscribeTo(HFEventID.OnWaveCleared, OnWaveCleared);
            HFEventManager.SubscribeTo<EntityBehavior, int>(HFEventID.OnEntitySelected, OnUnitSelected);
            HFEventManager.SubscribeTo<EntityBehavior, int>(HFEventID.OnUnitSpecialized, OnUnitSpecialized);
            HFEventManager.SubscribeTo<EntityBehavior>(HFEventID.OnUnitFight, OnUnitFight);
            HFEventManager.SubscribeTo<string>(HFEventID.OnError, SetMessage);

            ButtonCallNextWave.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            HFEventManager.UnsubscribeFrom(HFEventID.OnWaveBeginned, OnNewWaveBegin);
            HFEventManager.UnsubscribeFrom(HFEventID.OnWaveCleared, OnWaveCleared);
            HFEventManager.UnsubscribeFrom<EntityBehavior, int>(HFEventID.OnEntitySelected, OnUnitSelected);
            HFEventManager.UnsubscribeFrom<EntityBehavior, int>(HFEventID.OnUnitSpecialized, OnUnitSpecialized);
            HFEventManager.UnsubscribeFrom<EntityBehavior>(HFEventID.OnUnitFight, OnUnitFight);
            HFEventManager.UnsubscribeFrom<string>(HFEventID.OnError, SetMessage);

            Popup.gameObject.SetActive(false);
            Marker.gameObject.SetActive(false);
            Reset();
        }

        private void Awake()
        {
            Initialization.AddListener(this);
        }

        private void OnDestroy()
        {
            Initialization.RemoveListener(this);
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
                CastleCommandContainer.SetActive(false);
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

        private void SetMessage(string message)
        {
            Message.SetMessage(message);
        }

        #endregion

        public void SetCarryCapacity(Vector3 worldPosition, float carryCapacityRequired) 
        {
            carryCapacitySlider.value = 0;
            carryCapacitySlider.transform.localScale = Vector3.one;

            // If there isn't an entity selecte or the current entity is not a troop,
            // then return.
            if (InputReaderManager.Instance.CurrentEntity == null || 
                !(InputReaderManager.Instance.CurrentEntity is Troop)) 
                return;

            Vector2 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, worldPosition);
            float actualFillAmount = (InputReaderManager.Instance.CurrentEntity as Troop).CurrentCarryCapacity / carryCapacityRequired;
            actualFillAmount = Mathf.Clamp(actualFillAmount, 0, 1);

            carryCapacitySlider.transform.position = screenPosition;
            carryCapacitySlider.gameObject.SetActive(true);
            carryCapacitySlider.transform.DOPunchScale(Vector3.one, .2f).
                OnComplete(() => carryCapacitySlider.DOValue(actualFillAmount, .7f, false));
        }

        public void ReturnToLevelSelection()
        {
            OnUnitSelected(null as EntityBehavior, 0);
            HFGameManager.Instance.ChangeGMState(GameStates.WarRoom);
            HFUIManager.Instance.ShowAndClearHistory(HFUIWindowID.WAR_ROOM);
            HFScenesManager.Instance.LoadSceneFromIndex(1);
        }

        public void WinLevel()
        {
            HFGameManager.Instance.ChangeGMState(GameStates.None);
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

                        if (m_isTutorial && i == 1 || !m_isTutorial)
                            SpecializationButtons[i].gameObject.SetActive(true);
                    }
                    SpecializationBar.gameObject.SetActive(true);
                    SpecializationButtons[0].AddListener(() => entity.Specialization(Types.UnitType.DEFENDER_LVL1));
                    SpecializationButtons[1].AddListener(() => entity.Specialization(Types.UnitType.LIFTER_LVL1));
                    SpecializationButtons[1].AddListener(() => Initialization.RaiseEvent(EventRaisedType.OnStepCompleted));
                    SpecializationButtons[2].AddListener(() => entity.Specialization(Types.UnitType.RUNNER_LVL1));

                    SpecializationButtons[0].Cost.text = GameController.Instance.Collection.UnitsDictionary[Types.UnitType.DEFENDER_LVL1].OriginalUnitStats.Cost.ToString();
                    SpecializationButtons[1].Cost.text = GameController.Instance.Collection.UnitsDictionary[Types.UnitType.LIFTER_LVL1].OriginalUnitStats.Cost.ToString();
                    SpecializationButtons[2].Cost.text = GameController.Instance.Collection.UnitsDictionary[Types.UnitType.RUNNER_LVL1].OriginalUnitStats.Cost.ToString();

                    SpecializationButtons[0].SetToolTipMessage(GameController.Instance.Collection.UnitsDictionary[Types.UnitType.DEFENDER_LVL1].OriginalUnitStats.Name);
                    SpecializationButtons[1].SetToolTipMessage(GameController.Instance.Collection.UnitsDictionary[Types.UnitType.LIFTER_LVL1].OriginalUnitStats.Name);
                    SpecializationButtons[2].SetToolTipMessage(GameController.Instance.Collection.UnitsDictionary[Types.UnitType.RUNNER_LVL1].OriginalUnitStats.Name);
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
                    SpawnTroopButton.AddListener(() => castle.SpawnTroop());
                    SpawnTroopButton.AddListener(() => HFEventManager.TriggerEvent(HFEventID.OnTutorialQuestCompleted, TutorialID.Create_Ally));
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

                    SpecializationButtons[0].SetToolTipMessage(GameController.Instance.Collection.BuildingsDictionary[Types.BuildingType.BALLISTA_LVL1].OriginalBuildingStats.Name);
                    SpecializationButtons[1].SetToolTipMessage(GameController.Instance.Collection.BuildingsDictionary[Types.BuildingType.CRYSTAL_LVL1].OriginalBuildingStats.Name);
                    SpecializationButtons[2].SetToolTipMessage(GameController.Instance.Collection.BuildingsDictionary[Types.BuildingType.MORTAR_LVL1].OriginalBuildingStats.Name);
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

        public void OnGlobalInitialization()
        {
            m_isTutorial = true;
            
        }

        public void OnStepInitialization()
        {

        }

        public void OnStepCompleted()
        {
            m_isTutorial = false;
        }

        public void Reset()
        {
            m_isTutorial = false;
        }

        public void SetEnemySpawnMarker(Transform transform)
        {
            Marker.SetDestinationMarker(transform);
            Marker.gameObject.SetActive(true);
        }
    }
}
