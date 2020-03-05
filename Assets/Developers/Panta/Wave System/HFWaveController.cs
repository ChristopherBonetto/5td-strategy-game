using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HF.WaveSystem
{
    [RequireComponent(typeof(HFWaveView))]
    public class HFWaveController : MonoBehaviour
    {
        #region Serializefield
        [SerializeField]
        private List<Transform> m_SpawnPoints;

        public HFController Controller;
        #endregion

        /// <summary>
        /// Level's spawn points ordered from inspector.
        /// The ordered declare the spawn point ID.
        /// </summary>
        public List<Transform> SpawnPoints => m_SpawnPoints;


        /// <summary>
        /// Curernt state of wave controller flow.
        /// </summary>
        public HFWaveControllerState CurrentState { get; set; }


        /// <summary>
        /// is it waiting for player input?
        /// </summary>
        public bool WaitForInput { get; private set; }

        #region Wave's management variables

        /// <summary>
        /// Collection of all level's waves.
        /// It's only to read purpose.
        /// </summary>
        public HFWavesCollector WaveCollector { get; set; }


        private int m_WaveIndex = 0;
        /// <summary>
        /// Current index of the wave.
        /// </summary>
        public int WaveIndex
        {
            get { return m_WaveIndex; }
            set { m_WaveIndex = value; }
        }


        private int m_MinorWaveIndex;
        /// <summary>
        /// Current index of the minor wave.
        /// </summary>
        public int MinorWaveIndex
        {
            get { return m_MinorWaveIndex; }
            set { m_MinorWaveIndex = value; }
        }


        /// <summary>
        /// Get number of waves
        /// </summary>
        public List<HFWave> GetWaves => WaveCollector.WavesCollection;


        /// <summary>
        /// Get current wave.
        /// </summary>
        public HFWave GetCurrentWave => GetWaves[Mathf.Clamp(m_WaveIndex, 0, GetWaves.Count - 1)];


        /// <summary>
        /// Get number of minor waves of the current wave
        /// </summary>
        public List<HFWave.MinorWave> GetMinorWaves => GetWaves[Mathf.Clamp(m_WaveIndex, 0, GetWaves.Count - 1)].MinorWavesCollection;


        /// <summary>
        /// Get Current minor wave.
        /// </summary>
        public HFWave.MinorWave GetCurrentMinorWave => GetCurrentWave.MinorWavesCollection[MinorWaveIndex];


        /// <summary>
        /// Get number of all enemies in the current wave.
        /// </summary>
        public int GetTotalEnemiesOfTheWave => HFWaveReader.GetNumberOfEnemiesInTheWave(GetCurrentWave);    // Do that every wave refresh.


        private int m_CountOfEnemiesKilled;
        /// <summary>
        /// Count of enemies killed.
        /// </summary>
        public int CountOfEnemyKilled
        {
            get { return m_CountOfEnemiesKilled; }
            set { m_CountOfEnemiesKilled = value; }
        }

        #endregion

        private HFWaveView m_waveView;

        #region Monobehaviour cycle

        private void Awake()
        {
            m_waveView = GetComponent<HFWaveView>();
            if (m_waveView == null) gameObject.AddComponent<HFWaveView>();
        }

        private void OnEnable()
        {
            HFEventManager.SubscribeTo<HFUnit>(HFEventID.OnGenericUnitDeath, OnEnemyKilled);
            HFEventManager.SubscribeTo(HFEventID.OnCallNextWave, OnCallNextWave);
        }

        private void OnDisable()
        {
            HFEventManager.UnsubscribeFrom<HFUnit>(HFEventID.OnGenericUnitDeath, OnEnemyKilled);
            HFEventManager.UnsubscribeFrom(HFEventID.OnCallNextWave, OnCallNextWave);
        }

        private void Start()
        {
            Init();

            // Update view.
            m_waveView.UpdateWaveInfo(1, GetWaves.Count);
            m_waveView.EnableButtonToCallnextWave(true);
        }

        private void Update()
        {
            if (!WaitForInput)
                if (MinorWaveIndex < GetMinorWaves.Count)
                    CurrentState.Update(this);
        }

        #endregion

        private void Init()
        {
            WaitForInput = true;

            CurrentState = HFWaveControllerState.CheckingTimeElapsed;
            WaveIndex = 0;
            MinorWaveIndex = 0;

            // Try to get wave collector.
            HFScenesManager sm = HFScenesManager.Instance;
            if (sm.CurrentLevelSelected != null &&
                sm.CurrentLevelSelected.LevelWavesInfo != null)
                WaveCollector = sm.CurrentLevelSelected.LevelWavesInfo;
            // If there isn't, get a ddefault one.
            else
            {
                Debug.LogWarning($"There isn't a WaveCollection assets to level {sm.CurrentLevelSelected}" +
                    $"I give a default one");
                WaveCollector = sm.LevelContainer.Levels[0].LevelWavesInfo;
            }
        }

        /// <summary>
        /// Check if the level is cleared.
        /// This check is evaluated every time the wave is cleared.
        /// </summary>
        /// <returns></returns>
        public bool LevelCleared()
        {
            return WaveIndex > GetWaves.Count - 1;
        }

        /// <summary>
        /// check if the wave is cleared.
        /// This check is evaluated every kill confirmed.
        /// </summary>
        /// <returns></returns>
        public bool WaveCleared()
        {
            return CountOfEnemyKilled >= GetTotalEnemiesOfTheWave;
        }

        /// <summary>
        /// Invoke from event when a enemy troop is killed.
        /// </summary>
        public void OnEnemyKilled(HFUnit unit)
        {
            // Is it an enemy?
            if (unit.Team == Controller.Team)
            {
                CountOfEnemyKilled++;
                // Show in the UI this infos:
                // Wave index,
                // Enemies Killed / Total enemies in the wave.

                if (WaveCleared())
                {
                    WaitForInput = true;

                    // Reset all count and index.
                    CountOfEnemyKilled++;
                    MinorWaveIndex++;
                    // Increment wave index.
                    WaveIndex++;

                    if (LevelCleared())
                    {
                        //Set the game in win condition.
                        HFScenesManager.Instance.EndCurrentLevel(true);
                    }
                    else
                    {
                        m_waveView.EnableButtonToCallnextWave(true);
                    }
                }
            }
        }

        /// <summary>
        /// Called when OnCallNextWave event is triggered.
        /// </summary>
        private void OnCallNextWave()
        {
            if (WaitForInput)
            {
                WaitForInput = false;
                m_waveView.EnableButtonToCallnextWave(false);
                m_waveView.UpdateWaveInfo(Mathf.Min(WaveIndex + 1, GetWaves.Count), GetWaves.Count);
                m_waveView.SetEnemiesInfo(GetCurrentWave);
            }
        }
    }
}