using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HF.WaveSystem
{
    public class HFWaveController : MonoBehaviour
    {
        //----------------------------------------------
        // The wave collection has stored the waves.
        // Waves have a sequence  of behaviour that the
        // state machine perform.
        //----------------------------------------------

        private HFWavesCollection m_WaveCollection;
        public HFWavesCollection WaveCollection { get { return m_WaveCollection; } }

        public HFController Controller;


        //----------------------------------------------
        // Wave controller in order to run, it needs the 
        // reference to spawn points (of the enemies unity).
        // A state machine manage the behaviour of the wave
        // that can be: Signle, wait, bulk. 
        /// <see cref="HFWaveControllerState"/>.
        //----------------------------------------------

        [SerializeField]
        private List<HFSpawnPoint> m_SpawnPoints;
        public List<HFSpawnPoint> SpawnPoints => m_SpawnPoints;

        private HFSingleState m_SingleState;
        public HFSingleState SingleState => m_SingleState;

        private HFBulkState m_BulkState;
        public HFBulkState BulkState => m_BulkState;

        private HFWaitState m_WaitState;
        public HFWaitState WaitState => m_WaitState;

        private HFWaveControllerState m_CurrentState;
        public HFWaveControllerState CurrentState
        {
            get { return m_CurrentState; }
            set { m_CurrentState = value; }
        }

        private bool m_WaitForInput;
        public bool WaitForInput 
        { 
            get { return m_WaitForInput; }
            set { m_WaitForInput = value; }
        }


        //----------------------------------------------
        // Indexes take care about the position of the 
        // wave behaviour sequence.
        // Also the wave controller has a count of enemies
        // killed.
        //----------------------------------------------

        #region Count and indexes

        private int m_WaveIndex = 0;
        public int WaveIndex
        {
            get { return m_WaveIndex; }
            set 
            {
                m_WaveIndex = value;

                // Update the view.
                HFEventManager.TriggerEvent<int, int>(HFEventID.OnWaveIndexUpdate, Mathf.Min(WaveIndex + 1, GetWaves.Count), GetWaves.Count);
            }
        }

        private int m_MinorWaveIndex;
        public int MinorWaveIndex
        {
            get { return m_MinorWaveIndex; }
            set 
            { 
                m_MinorWaveIndex = value; 
                // Update the view
            }
        }

        private int m_CountOfEnemyKilled;
        public int CountOfEnemyKilled
        {
            get { return m_CountOfEnemyKilled; }
            set { m_CountOfEnemyKilled = value; }
        }

        #endregion

        //----------------------------------------------
        // Some usefull properties.
        //----------------------------------------------

        #region Utils

        public List<HFWaveModel> GetWaves => WaveCollection.WavesCollection;
        public HFWaveModel GetCurrentWave => GetWaves[Mathf.Min(WaveIndex, GetWaves.Count - 1)];

        public List<HFWaveModel.MinorWave> GetMinorWaves => GetWaves[Mathf.Min(WaveIndex, GetWaves.Count - 1)].MinorWavesCollection;
        public HFWaveModel.MinorWave GetCurrentMinorWave => GetCurrentWave.MinorWavesCollection[Mathf.Min(MinorWaveIndex, GetMinorWaves.Count - 1)];

        public int GetTotalEnemiesOfTheWave => HFWaveReader.GetNumberOfEnemiesInTheWave(GetCurrentWave);

        #endregion

        #region Monobehaviour cycle

        private void OnEnable()
        {
            HFEventManager.SubscribeTo<HFUnit>(HFEventID.OnUnitDeath, OnUnitDeath);
            HFEventManager.SubscribeTo(HFEventID.OnNewWaveBegin, OnNewWaveBegin);
            HFEventManager.SubscribeTo(HFEventID.OnWaveEnd, OnWaveEnd);
        }

        private void OnDisable()
        {
            HFEventManager.UnsubscribeFrom<HFUnit>(HFEventID.OnUnitDeath, OnUnitDeath);
            HFEventManager.UnsubscribeFrom(HFEventID.OnNewWaveBegin, OnNewWaveBegin);
            HFEventManager.UnsubscribeFrom(HFEventID.OnWaveEnd, OnWaveEnd);
        }

        private void Start()
        {
#if UNITY_EDITOR
            if (HFUIManager.Instance != null) Debug.Log("");
#endif

            Initialize();
            ResetAllCounts();

            HFGameManager.Instance.ChangeGMState(GameStates.PlayingLevel);

            // State Initialization

            m_SingleState = new HFSingleState(this);
            m_BulkState = new HFBulkState(this);
            m_WaitState = new HFWaitState(this);
            ChangeState();
        }

        private void Update()
        {
            if (!WaitForInput)
            {
                if (MinorWaveIndex < GetMinorWaves.Count)
                {
                    CurrentState.Update(this);
                    CurrentState.HandleExitCondition(this);
                }
            }
        }

        #endregion

        private void Initialize()
        {
            // Don't trigger the event.
            m_WaitForInput = true;

            HFScenesManager sm = HFScenesManager.Instance;
            if (sm.CurrentLevelSelected != null && sm.CurrentLevelSelected.LevelWavesInfo != null)
                m_WaveCollection = sm.CurrentLevelSelected.LevelWavesInfo;
            // If there isn't, get a ddefault one.
            else
            {
                Debug.LogWarning($"There isn't a WaveCollection assets to level {sm.CurrentLevelSelected}" +
                    $"I give a default one");
                m_WaveCollection = sm.LevelContainer.Levels[0].LevelWavesInfo;
            }
        }

        /// <summary>
        /// It will be called to initialize the state in the start,
        /// also when the current state go in exit condition.
        /// <see cref="HFWaveControllerState"/>
        /// </summary>
        public void ChangeState()
        {
            switch (GetCurrentMinorWave.MinorWaveType)
            {
                case MinorWaveType.Bulk:
                    CurrentState = BulkState;
                    break;
                case MinorWaveType.Single:
                    CurrentState = SingleState;
                    break;
                case MinorWaveType.Wait:
                    CurrentState = WaitState;
                    break;
            }

            CurrentState.HandleEnterPhase(this);
        }


        public bool LevelCleared()
        {
            return WaveIndex > GetWaves.Count - 1;
        }

        public bool WaveCleared()
        {
            return CountOfEnemyKilled >= GetTotalEnemiesOfTheWave;
        }


        public void OnUnitDeath(HFUnit unit)
        {
            // Is it an enemy?
            if (unit.Team != HFGameParameters.PlayerTeam)
            {
                CountOfEnemyKilled++;

                if (WaveCleared())
                {
                    HFEventManager.TriggerEvent(HFEventID.OnWaveEnd);
                }
            }
        }

        /// <summary>
        /// Reset waveIndex,
        /// Minor wave index.
        /// Count of enemy killed.
        /// </summary>
        public void ResetAllCounts()
        {
            WaveIndex = 0;
            MinorWaveIndex = 0;
            CountOfEnemyKilled = 0;
        }

        #region Events

        //-------------------------------------------
        // This event will be triggered by UI button.
        //-------------------------------------------

        private void OnNewWaveBegin()
        {
            CountOfEnemyKilled = 0;
            MinorWaveIndex = 0;

            WaitForInput = false;
        }

        //-------------------------------------------
        // This event will be triggered when the wave 
        // is cleared.
        //-------------------------------------------

        private void OnWaveEnd()
        {
            WaveIndex++;

            // Check if the level is cleared.
            // if it is, trigger the end level.

            if (LevelCleared())
            {
                WaitForInput = true;
            }
        }

        #endregion
    }
}