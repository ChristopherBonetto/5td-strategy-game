using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.WaveSystem
{
    public static class HFWaveReader
    {
        /// <summary>
        /// Get numbers of "minor" wave that spawn enemies.
        /// </summary>
        /// <param name="wave"></param>
        /// <returns></returns>
        public static int GetNumberOfWaves(HFWave wave)
        {
            int n = 0;

            foreach (var w in wave.BehavioursCollection)
            {
                if (w.BehaviourType == BehaviourType.Single || w.BehaviourType == BehaviourType.Bulk)
                    n++;
            }

            return n;
        }

        /// <summary>
        /// Get numbers of all enemies in the wave, (even if they are death).
        /// </summary>
        /// <param name="wave"></param>
        /// <returns></returns>
        public static int GetNumberOfEnemiesInTheWave(HFWave wave)
        {
            int n = 0;

            foreach (var w in wave.BehavioursCollection)
            {
                if (w.BehaviourType == BehaviourType.Single)
                    n += 1;
            }
            return n;
        }
    }
}
