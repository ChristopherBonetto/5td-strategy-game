using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.WaveSystem
{
    public class HFWaveLevel : MonoBehaviour
    {
        [SerializeField] 
        private List<HFWave> m_WaveCollection;
        /// <summary>
        /// This list it's only to read purpose.
        /// Contains all waves assets of the level.
        /// </summary>
        public List<HFWave> WavesCollection => m_WaveCollection;

        private Dictionary<int, Queue<HFWave.Behaviour>> m_BehaviourQueue;
        /// <summary>
        /// Queue of all behaviour of all waves.
        /// key : represents the number of the wave.
        /// value : a queue of the key-wave actions.
        /// </summary>
        public Dictionary<int, Queue<HFWave.Behaviour>> BehavioursQueue
        {
            get
            {
                if (m_BehaviourQueue == null)
                    m_BehaviourQueue = new Dictionary<int, Queue<HFWave.Behaviour>>();
                return m_BehaviourQueue;
            }
        }

        private int m_TotalWavesCount;    // Update the UI throw event.
        private int m_CurrentWaveCount;   // Update UI throw event.


        private void Start()
        {
            InitDictionary();

            ConvertListToQueue();
        }

        // Initialization
        private void InitDictionary()
        {
            for (int i = 0; i < WavesCollection.Count; i++)
            {
                BehavioursQueue[i] = new Queue<HFWave.Behaviour>();
            }
        }

        // Read the list and convert it to a queue.
        private void ConvertListToQueue()
        {
            for (int i = 0; i < WavesCollection.Count; i++)
            {
                for (int j = 0; j < WavesCollection[i].BehavioursCollection.Count; j++)
                {
                    HFWave.Behaviour behaviour = WavesCollection[i].BehavioursCollection[j];

                    BehavioursQueue[i].Enqueue(behaviour);
                }
            }
        }

        private void ReadNextBehaviour() { }    // It will be called throw event.
    }
}