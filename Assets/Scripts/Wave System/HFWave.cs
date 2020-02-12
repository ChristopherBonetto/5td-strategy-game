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
        /// <summary>
        /// Setting to spawn multiple instance of the same prefab
        /// </summary>
        Bulk = 1 << 2,
    }

    [CreateAssetMenu(fileName = "L_00_Wave_00", menuName = "Human Factor/New Wave")]
    public class HFWave : ScriptableObject
    {
        public List<Behaviour> BehavioursCollection;

        [System.Serializable]
        public class Behaviour
        {
#if UNITY_EDITOR
            public bool Foldout;
#endif

            public BehaviourType Type = BehaviourType.Single;

            // 0) Single
            public bool RandomEnemy;
            public GameObject EnemyPrefab;
            public int SpawnPoint;

            // 1) Wait
            public float TimeToWait;

            // 2) Bulk
            public int AmountToSpawn;
        }
    }
}
