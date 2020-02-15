using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HF.WaveSystem
{
    [CreateAssetMenu(fileName = "WaveCollect_L_00", menuName = "Human Factor/Wave/Collector")]
    public class HFWavesCollector : ScriptableObject
    {
		[SerializeField]
		private List<HFWave> m_WavesCollection;
		/// <summary>
		/// List of the waves of the level.
		/// </summary>
		public List<HFWave> WavesCollection => m_WavesCollection;
	}
}
