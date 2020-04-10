using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.Refactoring
{
    [CreateAssetMenu(fileName = "SO_wave_collection_lvl_00", menuName = "Refactoring/Good North/Wave/New Wave Collection")]
    public class HFWaveCollection : ScriptableObject
    {
        [SerializeField]
        List<HFWaveData> m_waves = new List<HFWaveData>();


        /// <summary>
        /// Get all wave assets in this collection.
        /// </summary>
        /// <returns></returns>
        public List<HFWaveData> GetWaves()
        {
            return m_waves;
        }
    }
}
