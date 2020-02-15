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

        private void Start()
        {
            RequestNextBehaviour(RequestType.Pre);
        }

        public void RequestNextBehaviour(RequestType requestType)
        {
            m_behaviour = GetBehaviours[m_currentBehaviourIndex];
            Debug.Log("Index = " + m_currentBehaviourIndex + ", " + "Type picked: " + m_behaviour.RequestType + ", " + "Type request: " + requestType);
            
            if (requestType == m_behaviour.RequestType)
            {
                m_currentBehaviourIndex++;
                
                if (m_behaviour.WaitForInput)
                {
                    // Show UI
                    Debug.Log("Show UI");
                    return;
                }

                if (m_currentBehaviourIndex > GetBehaviours.Count - 1)
                {
                    m_currentBehaviourIndex = 0;
                    m_currentWaveIndex++;

                    if (m_currentWaveIndex > GetWaves.Count - 1)
                    {
                        Debug.Log("End waves");
                        return;
                    }
                }

                if (m_behaviour.BehaviourType == BehaviourType.Single)
                {
                    ExecuteSingleBehaviour(m_behaviour);
                }
                else if (m_behaviour.BehaviourType == BehaviourType.Wait)
                {
                    ExecuteWaitBehaviour(m_behaviour);
                }
            }

        }

        private void ExecuteSingleBehaviour(HFWave.Behaviour behaviour)
        {
            //GameObject go = new GameObject("Enemy", typeof(HFUnit));    // Set stats
            //go.transform.position = SpawnPoints[behaviour.SpawnPoint].position;
            //go.transform.rotation = Quaternion.LookRotation(Vector3.zero - go.transform.position, Vector3.up);
            Debug.Log("Spawn Enemy");
            //RequestNextBehaviour(m_behaviour.RequestType); // simulate the eevnt trigger.
        }

        private void ExecuteWaitBehaviour(HFWave.Behaviour behaviour)
        {
            HFTimer timer = new HFTimer(behaviour.TimeToWait);
            StartCoroutine(timer.DecreaseTime(RequestNextBehaviour, m_behaviour.RequestType));
            Debug.Log("Time elapsed...");
        }
    }
}