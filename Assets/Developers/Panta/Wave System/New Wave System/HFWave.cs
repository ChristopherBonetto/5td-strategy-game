using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.WaveSystem
{
    public enum BehaviourType
    {
        /// <summary>
        /// Represent the single troop to spawn.
        /// </summary>
        Single = 1 << 0,
        /// <summary>
        /// Represent the time to wait
        /// </summary>
        Wait = 1 << 1,
    }

    [CreateAssetMenu(fileName = "L_00_Wave_00", menuName = "Human Factor/New Wave")]
    public class HFWave : ScriptableObject
    {
        public List<Behaviour> BehavioursCollection;

        [System.Serializable]
        public class Behaviour
        {
            public BehaviourType Type = BehaviourType.Single;

            // 0) Single
            public bool RandomEnemy;
            public GameObject EnemyPrefab;
            public int SpawnPoint;

            // 1) wait
            public float TimeToWait;
        }
    }
}
