using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.Refactoring
{
    [CreateAssetMenu(fileName = "so_WaveCollection_Lvl_00", menuName = "Good North/Wave system/New Wave Collection")]
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
