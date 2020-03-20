using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.WaveSystem
{
    public enum MinorWaveType
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

    [CreateAssetMenu(fileName = "L_00_Wave_00", menuName = "Human Factor/Wave/New Wave")]
    public class HFWaveModel : ScriptableObject
    {
        /// <summary>
        /// Collection of minor wave.
        /// </summary>
        public List<MinorWave> MinorWavesCollection;

        [System.Serializable]
        public class MinorWave
        {
#if UNITY_EDITOR
            public bool Foldout = true;
#endif

            public MinorWaveType MinorWaveType = MinorWaveType.Single;

            public HFBaseStats UnitStatsData;
            public int SpawnPoint;

            // 1) Wait
            public float TimeToWait;

            // 2) Bulk
            public int AmountToSpawn;
        }
    }
}
