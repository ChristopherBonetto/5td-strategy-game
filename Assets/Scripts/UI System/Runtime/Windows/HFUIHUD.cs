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
        public HFSpecializationB[] SpecializationButtons;
        public HFSpecializationB UpgradeButton;

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
                if (entity.IsBusy)
                {
                    UnitSelectedContainer.SetActive(false);
                    return;
                }

                SetUpSpecializationButton(entity);
                UnitSelectedContainer.SetActive(true);

                UnitSelectedContainer.transform.localScale = Vector3.zero;
                UnitSelectedContainer.transform.DOScale(1, 0.2f);
            }
        }

        private void OnUnitSpecialized(EntityBehavior entity, int team)
        {
            UnitSelectedContainer.SetActive(false);
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
            if (entity.EntityStats is UnitsStatsSO)
            {
                UnitsStatsSO stats = entity.EntityStats as UnitsStatsSO;

                if (stats.UnitType == Types.UnitType.STANDARD_ALLY)
                {
                    UpgradeButton.EnableButton(false);

                    for (int i = 0; i < SpecializationButtons.Length; i++)
                    {
                        SpecializationButtons[i].EnableButton(true);
                    }

                    SpecializationButtons[0].AddListener(() => entity.Specialization(Types.UnitType.DEFENDER_LVL1));
                    SpecializationButtons[0].SetUpButton(GameController.Instance.Collection.UnitsDictionary[Types.UnitType.DEFENDER_LVL1].UnitStatsCopy.Icon,
                                                        GameController.Instance.Collection.UnitsDictionary[Types.UnitType.DEFENDER_LVL1].UnitStatsCopy.Cost);

                    SpecializationButtons[1].AddListener(() => entity.Specialization(Types.UnitType.LIFTER_LVL1));
                    SpecializationButtons[1].SetUpButton(GameController.Instance.Collection.UnitsDictionary[Types.UnitType.LIFTER_LVL1].UnitStatsCopy.Icon,
                                                        GameController.Instance.Collection.UnitsDictionary[Types.UnitType.LIFTER_LVL1].UnitStatsCopy.Cost);

                    SpecializationButtons[2].AddListener(() => entity.Specialization(Types.UnitType.RUNNER_LVL1));
                    SpecializationButtons[2].SetUpButton(GameController.Instance.Collection.UnitsDictionary[Types.UnitType.RUNNER_LVL1].UnitStatsCopy.Icon,
                                                        GameController.Instance.Collection.UnitsDictionary[Types.UnitType.RUNNER_LVL1].UnitStatsCopy.Cost);
                }
                else
                {
                    UpgradeButton.EnableButton(true);

                    for (int i = 0; i < SpecializationButtons.Length; i++)
                    {
                        SpecializationButtons[i].EnableButton(false);
                    }

                    // max level
                    if (stats.UnitType == Types.UnitType.DEFENDER_LVL3 || stats.UnitType == Types.UnitType.RUNNER_LVL3 || stats.UnitType == Types.UnitType.LIFTER_LVL3)
                    {
                        UpgradeButton.EnableButton(false);
                        return;
                    }

                    // Defender
                    else if ((int)stats.UnitType >= (int)Types.UnitType.DEFENDER_LVL1 && (int)stats.UnitType < (int)Types.UnitType.DEFENDER_LVL3)
                    {
                        UpgradeButton.AddListener(() => entity.Specialization(stats.UnitType + 1));
                        UpgradeButton.SetUpButton(GameController.Instance.Collection.UnitsDictionary[stats.UnitType + 1].UnitStatsCopy.Icon,
                                    GameController.Instance.Collection.UnitsDictionary[stats.UnitType + 1].UnitStatsCopy.Cost);
                    }

                    // Lifter
                    else if ((int)stats.UnitType >= (int)Types.UnitType.LIFTER_LVL1 && (int)stats.UnitType < (int)Types.UnitType.LIFTER_LVL3)
                    {
                        UpgradeButton.AddListener(() => entity.Specialization(stats.UnitType + 1));
                        UpgradeButton.SetUpButton(GameController.Instance.Collection.UnitsDictionary[stats.UnitType + 1].UnitStatsCopy.Icon,
                                    GameController.Instance.Collection.UnitsDictionary[stats.UnitType + 1].UnitStatsCopy.Cost);
                    }

                    // Runner
                    else if ((int)stats.UnitType >= (int)Types.UnitType.RUNNER_LVL1 && (int)stats.UnitType < (int)Types.UnitType.RUNNER_LVL3)
                    {
                        UpgradeButton.AddListener(() => entity.Specialization(stats.UnitType + 1));
                        UpgradeButton.SetUpButton(GameController.Instance.Collection.UnitsDictionary[stats.UnitType + 1].UnitStatsCopy.Icon,
                                    GameController.Instance.Collection.UnitsDictionary[stats.UnitType + 1].UnitStatsCopy.Cost);
                    }
                }
            }
            else if (entity.EntityStats is BuildingsStatsSO)
            {
                BuildingsStatsSO stats = entity.EntityStats as BuildingsStatsSO;

                if (stats.BuildingType == Types.BuildingType.TOWER)
                {
                    UpgradeButton.EnableButton(false);

                    for (int i = 0; i < SpecializationButtons.Length; i++)
                    {
                        SpecializationButtons[i].EnableButton(true);
                    }

                    SpecializationButtons[0].AddListener(() => entity.Specialization(Types.BuildingType.BALLISTA_LVL1));
                    SpecializationButtons[0].SetUpButton(GameController.Instance.Collection.BuildingsDictionary[Types.BuildingType.BALLISTA_LVL1].BuildingStatsCopy.Icon,
                                                        GameController.Instance.Collection.BuildingsDictionary[Types.BuildingType.BALLISTA_LVL1].BuildingStatsCopy.Cost);

                    SpecializationButtons[1].AddListener(() => entity.Specialization(Types.BuildingType.CRYSTAL_LVL1));
                    SpecializationButtons[1].SetUpButton(GameController.Instance.Collection.BuildingsDictionary[Types.BuildingType.CRYSTAL_LVL1].BuildingStatsCopy.Icon,
                                                        GameController.Instance.Collection.BuildingsDictionary[Types.BuildingType.CRYSTAL_LVL1].BuildingStatsCopy.Cost);

                    SpecializationButtons[2].AddListener(() => entity.Specialization(Types.BuildingType.MORTAR_LVL1));
                    SpecializationButtons[2].SetUpButton(GameController.Instance.Collection.BuildingsDictionary[Types.BuildingType.MORTAR_LVL1].BuildingStatsCopy.Icon,
                                                        GameController.Instance.Collection.BuildingsDictionary[Types.BuildingType.MORTAR_LVL1].BuildingStatsCopy.Cost);
                }
                else
                {
                    UpgradeButton.EnableButton(true);

                    for (int i = 0; i < SpecializationButtons.Length; i++)
                    {
                        SpecializationButtons[i].EnableButton(false);
                    }

                    // max level
                    if (stats.BuildingType == Types.BuildingType.BALLISTA_LVL3 || stats.BuildingType == Types.BuildingType.CRYSTAL_LVL3 || stats.BuildingType == Types.BuildingType.MORTAR_LVL3)
                    {
                        UpgradeButton.EnableButton(false);
                        return;
                    }

                    // Ballista
                    else if ((int)stats.BuildingType >= (int)Types.BuildingType.BALLISTA_LVL1 && (int)stats.BuildingType < (int)Types.BuildingType.BALLISTA_LVL3)
                    {
                        UpgradeButton.AddListener(() => entity.Specialization(stats.BuildingType + 1));
                        UpgradeButton.SetUpButton(GameController.Instance.Collection.BuildingsDictionary[stats.BuildingType + 1].BuildingStatsCopy.Icon,
                                    GameController.Instance.Collection.BuildingsDictionary[stats.BuildingType + 1].BuildingStatsCopy.Cost);
                    }

                    // Crystal
                    else if ((int)stats.BuildingType >= (int)Types.BuildingType.CRYSTAL_LVL1 && (int)stats.BuildingType < (int)Types.BuildingType.CRYSTAL_LVL3)
                    {
                        UpgradeButton.AddListener(() => entity.Specialization(stats.BuildingType + 1));
                        UpgradeButton.SetUpButton(GameController.Instance.Collection.BuildingsDictionary[stats.BuildingType + 1].BuildingStatsCopy.Icon,
                                    GameController.Instance.Collection.BuildingsDictionary[stats.BuildingType + 1].BuildingStatsCopy.Cost);
                    }

                    // Mortar
                    else if ((int)stats.BuildingType >= (int)Types.BuildingType.MORTAR_LVL1 && (int)stats.BuildingType < (int)Types.BuildingType.MORTAR_LVL3)
                    {
                        UpgradeButton.AddListener(() => entity.Specialization(stats.BuildingType + 1));
                        UpgradeButton.SetUpButton(GameController.Instance.Collection.BuildingsDictionary[stats.BuildingType + 1].BuildingStatsCopy.Icon,
                                    GameController.Instance.Collection.BuildingsDictionary[stats.BuildingType + 1].BuildingStatsCopy.Cost);
                    }
                }
            }
        }
    }
}
