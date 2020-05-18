using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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


        [Header("Pop-ups")]
        public EntityUpgradeButton EntityUpgradeButton;
        public EntitySpecializeButton EntitySpecializeButton;


        private void OnEnable()
        {
            HFEventManager.SubscribeTo(HFEventID.OnWaveBeginned, OnNewWaveBegin);
            HFEventManager.SubscribeTo(HFEventID.OnWaveCleared, OnWaveCleared);
            HFEventManager.SubscribeTo<EntityBehavior, int>(HFEventID.OnUnitSelected, OnunitSelected);
            HFEventManager.SubscribeTo(HFEventID.OnUnitLift, OnUnitLift);

            ButtonCallNextWave.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            HFEventManager.UnsubscribeFrom(HFEventID.OnWaveBeginned, OnNewWaveBegin);
            HFEventManager.UnsubscribeFrom(HFEventID.OnWaveCleared, OnWaveCleared);
            HFEventManager.UnsubscribeFrom<EntityBehavior, int>(HFEventID.OnUnitSelected, OnunitSelected);
            HFEventManager.UnsubscribeFrom(HFEventID.OnUnitLift, OnUnitLift);
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

        public void OnUnitLift()
        {
            EntitySpecializeButton.gameObject.SetActive(false);
            EntityUpgradeButton.gameObject.SetActive(false);
        }

        public void OnunitSelected(EntityBehavior entity, int team)
        {
            if (entity != null)
            {
                EntitySpecializeButton.gameObject.SetActive(false);
                EntityUpgradeButton.gameObject.SetActive(false);


                // The case it's a unit
                if (entity is Troop)
                {
                    if (entity.IsBusy)
                    {
                        // Trigger an event that display "Unit can't be upgraded"
                        return;
                    }
                    if ((entity as Troop).BuildingHandled != null)
                    {
                        // Trigger an event that display "Unit can't be upgraded"
                        return;
                    }

                    UnitsStatsSO stats = entity.GetComponent<Troop>().GetStats();

                    if (stats.UnitType != Types.UnitType.PEASANT)
                        EntityUpgradeButton.SetUpgradeButton(entity);
                    else if (stats.UnitType == Types.UnitType.PEASANT)
                        EntitySpecializeButton.SetSpecializeButton(entity);
                }
                // the case it's a building
                else if (entity is BuildingBehaviour)
                {
                    BuildingsStatsSO stats = entity.GetComponent<BuildingBehaviour>().GetStats();
                    EntityUpgradeButton.SetUpgradeButton(entity);
                }
            }
            else
            {
                EntitySpecializeButton.gameObject.SetActive(false);
                EntityUpgradeButton.gameObject.SetActive(false);
            }
        }
    }
}
