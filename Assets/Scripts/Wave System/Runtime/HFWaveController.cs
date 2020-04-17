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

        [SerializeField]
        private HFController m_Controller;
        public HFController Controller => m_Controller;

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

        //--------------------------------
        // Enemies
        //--------------------------------

        int TotalEnemiesInTheWave => m_currentWave.GetCountOfEnemies();
        int m_currentEnemiesCount = 0;


        #region Helpers
        private const string m_debugColor = "#FFFF00";
        #endregion


        #region MonoBehvaiour Cycle

        private void OnEnable()
        {
            HFEventManager.SubscribeTo(HFEventID.OnWaveBeginned, OnWaveBeggined);
            HFEventManager.SubscribeTo(HFEventID.OnWaveCleared, OnWaveCleared);
            HFEventManager.SubscribeTo<HFUnit>(HFEventID.OnUnitDeath, OnUnitDead);
        }

        private void OnDisable()
        {
            HFEventManager.UnsubscribeFrom(HFEventID.OnWaveBeginned, OnWaveBeggined);
            HFEventManager.UnsubscribeFrom(HFEventID.OnWaveCleared, OnWaveCleared);
            HFEventManager.UnsubscribeFrom<HFUnit>(HFEventID.OnUnitDeath, OnUnitDead);
        }

        private void Start()
        {
            Initialization();
        }

        private void Update()
        {
            if (m_currentBehaviour != null)
            {
                m_currentBehaviour.Execute(this);
                m_currentBehaviour.Exit(this);
            }
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
                        m_behavioursToPerform.Enqueue(new HFWaveBehaviourSingle(behaviour.SpawnPointID, behaviour.AmountToSpawn, behaviour.UnitData));
                        break;

                    case BehaviourType.WAIT:
                        m_behavioursToPerform.Enqueue(new HFWaveBehaviourWait(behaviour.TimeToWait));
                        break;

                    case BehaviourType.BULK:
                        m_behavioursToPerform.Enqueue(new HFWaveBehaviourBulk(behaviour.SpawnPointID, behaviour.AmountToSpawn, behaviour.TimeToWait, behaviour.UnitData));
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
                HFUIManager.Instance.ShowAndClearHistory(HFUIWindowID.WR_LEVEL_SELCTION);
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
        private void OnUnitDead(HFUnit unit)
        {
            // Is the enemy team?
            if (unit.Team != HFGameParameters.PlayerTeam)
            {
                m_currentEnemiesCount++;

                Debug.Log($"<color={m_debugColor}><b>[{this.GetType().Name}]</b></color> : Enemies killed = {m_currentEnemiesCount} / {TotalEnemiesInTheWave}");

                if (m_currentEnemiesCount >= TotalEnemiesInTheWave)
                {
                    HFEventManager.TriggerEvent(HFEventID.OnWaveCleared);
                }
            }
        }

        #endregion
    }
}
