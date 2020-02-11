using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.WaveSystem
{
    public class HFWaveLevel : MonoBehaviour
    {
        public HFWave[] Waves;

        public Dictionary<int, Queue<HFWave.Behaviour>> BehavioursQueue;


        private void Start()
        {
            BehavioursQueue = new Dictionary<int, Queue<HFWave.Behaviour>>();

            for (int i = 0; i < Waves.Length; i++)
            {
                BehavioursQueue.Add(i, new Queue<HFWave.Behaviour>());

                for (int j = 0; j < Waves[i].BehavioursCollection.Count; j++)
                {
                    BehavioursQueue[i].Enqueue(Waves[i].BehavioursCollection[j]);
                    Debug.Log("Add : " + Waves[i].BehavioursCollection[j].Type + " | in key : " + i);
                }
            }
        }
    }
}