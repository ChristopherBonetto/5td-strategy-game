using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField] private HFController m_Controller;
        [SerializeField] private Button m_NextWavebutton;
        #endregion

        #region Private (single variables)
        private int m_WaveIndex = 0;
        private int m_MinorWaveIndex;
        private int m_CountOfEnemiesKilled;

        private float m_currentTime;
        private bool m_waitForInput = false;

        // Bulk
        private int m_bulkSpawnedIndex;
        private float m_totalDelayBetweenBulkSpawn;
        private float m_currentDelayBetweenBulkSpawn;
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

        /// <summary>
        /// Count of enemies killed.
        /// </summary>
        public int CountOfEnemyKilled
        {
            get { return m_CountOfEnemiesKilled; }
            set { m_CountOfEnemiesKilled = value; }
        }

        #endregion

        private void OnEnable()
        {
            HFEventManager.SubscribeTo<HFUnit>(HFEventID.OnUnitDeath, OnEnemyKilled);
        }

        private void OnDisable()
        {
            
            HFEventManager.UnsubscribeFrom<HFUnit>(HFEventID.OnUnitDeath, OnEnemyKilled);
        }

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
        public void OnEnemyKilled(HFUnit unit)
        {
            // only if the unit is marked as enemy do this function.
            if (unit.Team == m_Controller.Team)
            {
                CountOfEnemyKilled++;
                Debug.Log(WaveIndex);
                Debug.Log($"Total enemies killed: {CountOfEnemyKilled} / {GetTotalEnemiesOfTheWave}");

                if (WaveCleared())
                {

                    // Reset the count of enemies killed.
                    CountOfEnemyKilled = 0;
                    // increase the WaveIndex.
                    WaveIndex++;
                    // Reset the MinorWaveIndex.
                    MinorWaveIndex = 0;


                    if (LevelCleared())
                    {
                        // Show end level results.
                        Debug.Log("End Level");
                    }
                    else
                    {
                        // Show button in UI to start the next wave.
                        m_waitForInput = true;
                        m_NextWavebutton.gameObject.SetActive(true);
                    }
                }
            }
        }

        // This will be a state machine.
        public void CallNextMinorWave() // => think about a state machine.
        {
            if (!m_waitForInput)
            {
                if (MinorWaveIndex <= GetMinorWaves.Count - 1)
                {
                    if (m_currentTime <= 0)
                    {
                        HFWave.MinorWave bh = GetCurrentMinorWave;
                        Debug.Log($"Wave {WaveIndex}, Minor wave {MinorWaveIndex}" + "\n" +
                            $"{GetCurrentWave.name}");

                        if (bh.MinorWaveType == MinorWaveType.Single)
                        {
                            // Spawn it
                            Vector3 position = SpawnPoints[GetCurrentMinorWave.SpawnPoint].position;
                            m_Controller.SpawnUnit(GetCurrentMinorWave.UnitStatsData, position);

                            MinorWaveIndex++;

                            // Count ++ of the wave spawned.
                        }
                        else if (bh.MinorWaveType == MinorWaveType.Wait)
                        {
                            m_currentTime = bh.TimeToWait;

                            MinorWaveIndex++;
                        }
                        else if (bh.MinorWaveType == MinorWaveType.Bulk)
                        {
                            if (m_currentDelayBetweenBulkSpawn <= 0)
                            {
                                if (m_bulkSpawnedIndex < bh.AmountToSpawn)
                                {
                                    Vector3 position = SpawnPoints[GetCurrentMinorWave.SpawnPoint].position;
                                    m_Controller.SpawnUnit(GetCurrentMinorWave.UnitStatsData, position);

                                    m_bulkSpawnedIndex++;
                                }
                                else
                                {
                                    m_bulkSpawnedIndex = 0;
                                    MinorWaveIndex++;
                                }
                            }
                            else
                            {
                                m_currentDelayBetweenBulkSpawn = m_totalDelayBetweenBulkSpawn;
                            }
                        }   // TEMP@
                    }
                    else
                        m_currentTime -= Time.deltaTime;
                }
            }
        }

        public void CallNextWave()
        {
            m_waitForInput = false;
            m_NextWavebutton.gameObject.SetActive(false);
        }
    }
}