using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.Refactoring
{
    public class HFWaveController : MonoBehaviour
    {
        [SerializeField]
        HFSpawnPoint[] m_spawnPoints;
        public HFSpawnPoint[] SpawnPoints => m_spawnPoints;

        HFWaveCollection m_waveCollection = null;

        //--------------------------------
        // Wave
        //--------------------------------

        List<HFWaveData> TotalWaves => m_waveCollection.GetWaves();
        HFWaveData m_currentWave = null;
        public int WaveIndex { get; private set; } = 0;

        //--------------------------------
        // Behaviour
        //--------------------------------

        List<HFWaveData.HFWaveBehaviourData> TotalBehaviours => m_currentWave.GetBehaviours();
        Queue<HFWaveBehaviour> m_behavioursToPerform = new Queue<HFWaveBehaviour>();
        HFWaveBehaviour m_currentBehaviour = null;
        private bool Pausing = false;

        //--------------------------------
        // Enemies
        //--------------------------------

        int TotalEnemiesInTheWave => m_currentWave.GetCountOfEnemies();
        int m_currentEnemiesCount = 0;

        TileHighlight lastLocator;
        public LayerMask LocatorLayer;




        #region Helpers
        private const string m_debugColor = "#FFFF00";
        #endregion


        #region MonoBehvaiour Cycle

        private void OnEnable()
        {
            HFEventManager.SubscribeTo(HFEventID.OnWaveBeginned, OnWaveBeggined);
            HFEventManager.SubscribeTo(HFEventID.OnWaveCleared, OnWaveCleared);
            HFEventManager.SubscribeTo<EntityBehavior>(HFEventID.OnEntityDeath, OnEntityDead);
            HFEventManager.SubscribeTo<bool>(HFEventID.OnPauseMode, OnPauseMode);
        }

        private void OnDisable()
        {
            HFEventManager.UnsubscribeFrom(HFEventID.OnWaveBeginned, OnWaveBeggined);
            HFEventManager.UnsubscribeFrom(HFEventID.OnWaveCleared, OnWaveCleared);
            HFEventManager.UnsubscribeFrom<EntityBehavior>(HFEventID.OnEntityDeath, OnEntityDead);
            HFEventManager.UnsubscribeFrom<bool>(HFEventID.OnPauseMode, OnPauseMode);
        }

        private void Start()
        {
            Initialization();
            LocatorLayer = LayerMask.GetMask("Terrain");
        }

        private void Update()
        {
            if (m_currentBehaviour != null && !Pausing)
            {
                m_currentBehaviour.Execute(this);
                m_currentBehaviour.Exit(this);
            }
            
            //LocatorRay();

            //if (lastLocator != null)
            //{
            //    if (Input.GetMouseButtonDown(1))
            //    {
            //        lastLocator.OnClick();


            //    }
            //}


        }

        #endregion


        private void Initialization()
        {
            // Get the collection
            m_waveCollection = HFScenesManager.Instance.CurrentLevelSelected.LevelWavesInfo;

            // Get and set the wave
            WaveIndex = 0;
            m_currentWave = TotalWaves[0];

            // Get and set the behaviours
            SetBehavioursToPerform();

            // Set the enemies count
            m_currentEnemiesCount = 0;

            HFGameManager.Instance.ChangeGMState(GameStates.PlayingLevel);
        }

        private void SetBehavioursToPerform()
        {
            HFWaveData.HFWaveBehaviourData behaviour = null;

            for (int i = 0; i < TotalBehaviours.Count; i++)
            {
                behaviour = TotalBehaviours[i];

                switch (behaviour.Type)
                {
                    case BehaviourType.SINGLE:
                        m_behavioursToPerform.Enqueue(new HFWaveBehaviourSingle(behaviour.SpawnPointID, behaviour.AmountToSpawn, behaviour.UnitType));
                        break;

                    case BehaviourType.WAIT:
                        m_behavioursToPerform.Enqueue(new HFWaveBehaviourWait(behaviour.TimeToWait));
                        break;

                    case BehaviourType.BULK:
                        m_behavioursToPerform.Enqueue(new HFWaveBehaviourBulk(behaviour.SpawnPointID, behaviour.AmountToSpawn, behaviour.TimeToWait, behaviour.UnitType));
                        break;
                }
            }
        }


        /// <summary>
        /// Set the current wave behaviour to perform.
        /// It's called in Exit condition.
        /// <see cref="HFWaveBehaviour.Exit(HFWaveController)"/>
        /// </summary>
        public void SetCurrentBehaviourToPerform()
        {
            // Force the current behaviour to reset.
            m_currentBehaviour = null;

            if (m_behavioursToPerform.Count <= 0)
            {
                // Trigger OnWaveEnded().
                Debug.Log($"<color={m_debugColor}><b>[{this.GetType().Name}]</b></color> : Wave end!");
            }
            else
            {
                m_currentBehaviour = m_behavioursToPerform.Dequeue();
                Debug.Log($"<color={m_debugColor}><b>[{this.GetType().Name}]</b></color> : Remaining behaviours = {m_behavioursToPerform.Count} / {TotalBehaviours.Count}");
            }
        }

        #region Events

        //---------------------------------------------
        // All events the this component listening
        //---------------------------------------------

        /// <summary>
        /// Triggered when the last behaviour is performed.
        /// </summary>
        private void OnWaveEnded()
        {

        }

        /// <summary>
        /// Triggered when the new wave begin.
        /// </summary>
        private void OnWaveBeggined()
        {
            m_currentWave = TotalWaves[WaveIndex];

            m_currentEnemiesCount = 0;

            // Set the current wave to perform 
            // when the wave begin
            SetCurrentBehaviourToPerform();

            // Update wave index in UI.
            HFEventManager.TriggerEvent<int, int>(HFEventID.OnWaveIndexUpdate, WaveIndex + 1, TotalWaves.Count);
        }

        /// <summary>
        /// Triggered when the last enmy is killed.
        /// </summary>
        private void OnWaveCleared()
        {
            WaveIndex++;

            // @Temp
            // if the level end, return to level selection.
            if (WaveIndex >= TotalWaves.Count)
            {
                // Win level.
                HFScenesManager.Instance.EndCurrentLevel(true);
                HFUIManager.Instance.ShowAndClearHistory(HFUIWindowID.WAR_ROOM);
                HFScenesManager.Instance.LoadSceneFromIndex(1);

                return;
            }

            m_currentWave = TotalWaves[WaveIndex];

            // After set the new wave, we store all new behaviours.
            // We do it when the wave is cleared to save performance.
            SetBehavioursToPerform();
        }

        /// <summary>
        /// Triggered when an enemy die.
        /// </summary>
        private void OnEntityDead(EntityBehavior entity)
        {
            if(entity.EntityPlayerType == Types.PlayerType.AI)
            {
                m_currentEnemiesCount++;

                Debug.Log($"<color={m_debugColor}><b>[{this.GetType().Name}]</b></color> : Enemies killed = {m_currentEnemiesCount} / {TotalEnemiesInTheWave}");

                if (m_currentEnemiesCount >= TotalEnemiesInTheWave)
                {
                    HFEventManager.TriggerEvent(HFEventID.OnWaveCleared);
                }
            }
        }

        private void OnPauseMode(bool freeze)
        {
            Pausing = freeze;
        }

        #endregion 

        private void LocatorRay()
        {
            //Deprecated
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LocatorLayer))
            {
                if (hit.collider != null)
                {
                    TileHighlight tmpLoc = hit.collider.GetComponentInChildren<TileHighlight>();

                    if (lastLocator != null && lastLocator == tmpLoc) return;

                    //lastLocator?.MouseExit();


                    lastLocator = tmpLoc;

                    //lastLocator?.MouseEnter();

                }
            }
            else
            {
                if (lastLocator != null)
                {
                    //lastLocator?.MouseExit();


                    lastLocator = null;
                }
            }
        }
    }
}
