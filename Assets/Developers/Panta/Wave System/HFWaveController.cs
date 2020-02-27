using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HF.WaveSystem
{
    public class HFWaveController : MonoBehaviour
    {
        #region Serializefield
        [SerializeField]
        private List<Transform> m_SpawnPoints;

        [SerializeField] 
        private HFWavesCollector m_WaveCollector;

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

        #region Wave variables
        /// <summary>
        /// Collection of all level's waves.
        /// It's only to read purpose.
        /// </summary>
        public HFWavesCollector WaveCollector => m_WaveCollector;

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

        private HFInGameWindow m_inGameWindow;

        private void OnEnable()
        {
            HFEventManager.SubscribeTo<HFUnit>(HFEventID.OnUnitDeath, OnEnemyKilled);
            HFEventManager.SubscribeTo(HFEventID.OnCallNextWave, OnCallNextWave);
        }

        private void OnDisable()
        {
            HFEventManager.UnsubscribeFrom<HFUnit>(HFEventID.OnUnitDeath, OnEnemyKilled);
            HFEventManager.UnsubscribeFrom(HFEventID.OnCallNextWave, OnCallNextWave);
        }

        private void Start()
        {
            // Wait for input at the beggining.
            WaitForInput = true;
			HFEventManager.TriggerEvent(HFEventID.OnLevelReady);

            // Initialization.
            CurrentState = HFWaveControllerState.CheckingTimeElapsed;
            WaveIndex = 0;
            MinorWaveIndex = 0;

            m_inGameWindow = HFUIManager.Instance.UIControls[UIControlID.InGameWindow] as HFInGameWindow;
            m_inGameWindow.WaveInfoUIElement.UpdateWaveInfoDisplayed(WaveIndex, GetWaves.Count);
            m_inGameWindow.EnemyInfoUIElement.SetEnemiesInfo(GetCurrentWave);
        }

        private void Update()
        {
            if (!WaitForInput)
                if (MinorWaveIndex < GetMinorWaves.Count)
                    CurrentState.Update(this);
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
            if (unit.Team == Controller.Team)
            {
                CountOfEnemyKilled++;
                Debug.Log(WaveIndex);   // => UI feedback
                Debug.Log($"Total enemies killed: {CountOfEnemyKilled} / {GetTotalEnemiesOfTheWave}");  // => UI feedback

                if (WaveCleared())
                {
                    WaitForInput = true;

                    // Reset all count and index
                    CountOfEnemyKilled = 0;
                    WaveIndex++;
                    MinorWaveIndex = 0;

                    if (LevelCleared())
                    {
                        //Set the game in win condition.
                        HFEventManager.TriggerEvent<bool>(HFEventID.OnEndLevel, true);

                        Debug.Log("End Level");
                    }
                    else
                    {
                        m_inGameWindow.WaveInfoUIElement.UpdateWaveInfoDisplayed(WaveIndex, GetWaves.Count);
                        m_inGameWindow.EnemyInfoUIElement.SetEnemiesInfo(GetCurrentWave);

                        // Show button in UI to start the next wave.
                        HFEventManager.TriggerEvent<bool>(HFEventID.OnWaveEnd, false);
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
			}
        }
    }
}