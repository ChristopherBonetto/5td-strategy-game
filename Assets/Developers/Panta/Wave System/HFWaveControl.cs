using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFWaveControl : MonoBehaviour
{
	[Header("Inspector Field")] [Tooltip("Represent the order of the wave")]
	public int WaveIndex;

	private bool m_IsCleared;
	public bool IsCleared
	{
		get { return m_IsCleared; }
		set 
		{ 
			m_IsCleared = value;

			if (value == true)
				HFWaveManager.Instance.TotalWavesCleared++;
		}
	}


	// Minor waves managememnt.
	public List<HFMinorWave> MinorWaves;

	private int m_TotalMinorWaveCleared;
	/// <summary>
	/// take ref of the minor waves cleared.
	/// if all minor waves are cleared mark this wave as cleared.
	/// </summary>
	public int TotalMinorWaveCleared
	{
		get { return m_TotalMinorWaveCleared; }
		set 
		{
			m_TotalMinorWaveCleared = value;

			if (value >= MinorWaves.Count)
				IsCleared = true;
		}
	}



	#region Monobehaviour Cycle
	private void Start()
	{
		HFWaveManager.Instance.AddWave(this);
	}

	private void OnDisable()
	{
		HFWaveManager.Instance.RemoveWave(this);
	}
	#endregion
}
