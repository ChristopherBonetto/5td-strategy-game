using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.WaveSystem
{
    public class HFWaveLevel : MonoBehaviour
    {
        #region Serializefield (single variables)
        // e.g. 
        // public var;
        // [Serializefield] private var;
        [SerializeField]
        private List<Transform> m_SpawnPoints;

        [SerializeField] 
        private HFWavesCollector m_WaveCollector;

        public HFController Controller;

        public bool wait;
        #endregion

        #region Private (single variables)
        // e.g. 
        // private var 
        private int m_WaveIndex;
        private int m_MinorWaveIndex;
        private int m_CountOfEnemiesKilled;
        private float m_currentTime;
        #endregion

        #region Property
        /// <summary>
        /// Level's spawn points ordered from inspector.
        /// The ordered declare the spawn point ID.
        /// </summary>
        public List<Transform> SpawnPoints => m_SpawnPoints;

        /// <summary>
        /// Collection of all level's waves.
        /// It's only to read purpose.
        /// </summary>
        public HFWavesCollector WaveCollector => m_WaveCollector;

        /// <summary>
        /// Current index of the wave.
        /// </summary>
        public int WaveIndex
        {
            get { return m_WaveIndex; }
            set { m_WaveIndex = value; }
        }

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
        /// Get number of minor waves of the current wave
        /// </summary>
        public List<HFWave.Behaviour> GetMinorWaves => GetWaves[WaveIndex].BehavioursCollection;

        /// <summary>
        /// Get current wave.
        /// </summary>
        public HFWave GetCurrentWave => GetWaves[WaveIndex];

        /// <summary>
        /// Get Current minor wave.
        /// </summary>
        public HFWave.Behaviour GetCurrentMinorWave => GetCurrentWave.BehavioursCollection[MinorWaveIndex];

        /// <summary>
        /// Get number of all enemies in the current wave.
        /// </summary>
        public int GetTotalEnemiesOfTheWave => HFWaveReader.GetNumberOfEnemiesInTheWave(GetCurrentWave);

        /// <summary>
        /// Count of enemies killed.
        /// </summary>
        public int CountOfEnemyKilled
        {
            get { return m_CountOfEnemiesKilled; }
            set { m_CountOfEnemiesKilled = value; }
        }

        #endregion

        private void Update()
        {
            CallNextMinorWave();
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
        public void OnEnemyKilled()
        {
            // only if the unit is marked as enemy do this function.

            CountOfEnemyKilled++;

            if (WaveCleared())
            {
                // Show button in UI to start the next wave.
                // Reset the count of enemies killed.
                CountOfEnemyKilled = 0;
                // increase the WaveIndex.
                WaveIndex++;
                // Reset the MinorWaveIndex.
                MinorWaveIndex = 0;

                if (LevelCleared())
                {
                    // Show end level results.
                }
            }
        }

        public void CallNextMinorWave()
        {
            if (MinorWaveIndex <= GetMinorWaves.Count - 1)
            {
                if (m_currentTime <= 0)
                {
                    HFWave.Behaviour bh = GetCurrentMinorWave;
                    Debug.Log($"Wave {WaveIndex}, Minor wave {MinorWaveIndex}");

                    if (bh.BehaviourType == BehaviourType.Single)
                    {
                        // Spawn it
                        Vector3 position = SpawnPoints[GetCurrentMinorWave.SpawnPoint].position;
                        Controller.SpawnUnit(GetCurrentMinorWave.EnemyPrefab, position);

                        // Count ++ of the wave spawned.
                    }
                    else if (bh.BehaviourType == BehaviourType.Wait)
                    {
                        m_currentTime = bh.TimeToWait;
                    }

                    MinorWaveIndex++;
                }
                else
                    m_currentTime -= Time.deltaTime;
            }
        }
    }
}