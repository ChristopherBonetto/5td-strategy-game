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

        public HFController controller;
        #endregion

        #region Private (single variables)
        // e.g. 
        // private var 

        private int m_currentWaveIndex;
        private int m_currentBehaviourIndex;
        private HFWave.Behaviour m_behaviour;
        #endregion

        #region Property
        [SerializeField]
        private List<Transform> m_SpawnPoints;
        /// <summary>
        /// Level's spawn points ordered from inspector.
        /// The ordered declare the spawn point ID.
        /// </summary>
        public List<Transform> SpawnPoints => m_SpawnPoints;

        [SerializeField] 
        private HFWavesCollector m_WaveCollector;
        /// <summary>
        /// Collection of all level's waves.
        /// It's only to read purpose.
        /// </summary>
        public HFWavesCollector WaveCollector => m_WaveCollector;

        public List<HFWave> GetWaves => WaveCollector.WavesCollection;
        public List<HFWave.Behaviour> GetBehaviours => GetWaves[m_currentWaveIndex].BehavioursCollection;
        #endregion

        private void OnEnable()
        {
            HFEventManager.SubscribeTo<RequestType>(HFEventID.OnRequestNewBehaviour, RequestNextBehaviour);
        }

        private void OnDisable()
        {
            
            HFEventManager.UnsubscribeFrom<RequestType>(HFEventID.OnRequestNewBehaviour, RequestNextBehaviour);
        }

        public void RequestNextBehaviourFromInput()
        {
            RequestNextBehaviour(GetBehaviours[m_currentBehaviourIndex].RequestType);
        }

        public void RequestNextBehaviour(RequestType requestType)
        {
            if (m_behaviour == null)
                m_behaviour = GetBehaviours[m_currentBehaviourIndex];

            if (requestType == m_behaviour.RequestType)
            {
                // Check index bounds
                if (m_currentBehaviourIndex > GetBehaviours.Count - 1)
                {
                    m_currentWaveIndex++;
                    m_currentBehaviourIndex = 0;

                    if (m_currentWaveIndex > GetWaves.Count - 1)
                    {
                        Debug.Log("End Level");
                        return;
                    }
                }

                m_behaviour = GetBehaviours[m_currentBehaviourIndex];

                if (m_behaviour.BehaviourType == BehaviourType.Single)
                {
                    ExecuteSingleBehaviour(m_behaviour);
                }
                else if (m_behaviour.BehaviourType == BehaviourType.Wait)
                {
                    ExecuteWaitBehaviour(m_behaviour);
                }

                m_currentBehaviourIndex++;
            }
        }

        private void ExecuteSingleBehaviour(HFWave.Behaviour behaviour)
        {
            if (behaviour.EnemyPrefab == null)
            {
                Debug.LogWarning($"There are no stats assigned, Wave {m_currentWaveIndex}, Minor wave {m_currentBehaviourIndex}");
                return;
            }

            Vector3 pos = SpawnPoints[behaviour.SpawnPoint].position;

            controller.SpawnUnit(GetBehaviours[m_currentBehaviourIndex].EnemyPrefab, pos);
            Debug.Log("Spawn Enemy");
        }

        private void ExecuteWaitBehaviour(HFWave.Behaviour behaviour)
        {
            HFTimer timer = new HFTimer(behaviour.TimeToWait);
            StartCoroutine(timer.DecreaseTime(RequestNextBehaviour, m_behaviour.RequestType));
            Debug.Log("Time elapsed...");
        }
    }
}