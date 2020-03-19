using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HF.WaveSystem
{
    [CreateAssetMenu(fileName = "WaveCollect_L_00", menuName = "Human Factor/Wave/Collector")]
    public class HFWavesCollection : ScriptableObject
    {
		[SerializeField]
		private List<HFWaveModel> m_WavesCollection;
		/// <summary>
		/// List of the waves of the level.
		/// </summary>
		public List<HFWaveModel> WavesCollection => m_WavesCollection;
	}
}
