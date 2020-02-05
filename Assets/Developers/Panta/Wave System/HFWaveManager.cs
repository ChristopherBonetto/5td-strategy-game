using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SpawnType
{
	Pre,
	Post,
}


public class HFWaveManager : Singleton<HFWaveManager>
{
	private Dictionary<int, HFWaveControl> m_WaveControls;
	/// <summary>
	/// All wave controls of the current level.
	/// </summary>
	public Dictionary<int, HFWaveControl> WaveControls
	{
		get 
		{
			m_WaveControls = m_WaveControls ?? new Dictionary<int, HFWaveControl>();
			return m_WaveControls; 
		}
		set { m_WaveControls = value; }
	}

	private int m_CurrentWaveIndex;
	/// <summary>
	/// Current index of the running wave.
	/// </summary>
	public int CurrentWaveIndex
	{
		get { return m_CurrentWaveIndex; }
		set { m_CurrentWaveIndex = value; }
	}

	private int m_TotalWavesCleared;
	public int TotalWavesCleared
	{
		get { return m_TotalWavesCleared; }
		set
		{
			m_TotalWavesCleared = value;

			if (value >= WaveControls.Count)
			{
				Debug.Log("Level Complete!");
			}
		}
	}


	private void Start()
	{
		ResetValues();
		SpawnNextMinorWave();
	}

	public void SpawnNextMinorWave()
	{
		if (WaveControls[CurrentWaveIndex] != null)
		{
			HFWaveControl control = GetCurrentWaveControl();

			if (control.MinorWaves[control.TotalMinorWaveCleared] != null)
			{
				HFMinorWave minorWave = control.MinorWaves[control.TotalMinorWaveCleared];

				// Instantiate the prefab at the position associated, 
				// make the prefab facing the map. (i suppose the map is located in (0,0,0)).
				Instantiate(minorWave.PrefabToSpawn, minorWave.SpawnPoint.position, Quaternion.LookRotation(Vector3.zero - minorWave.SpawnPoint.position, Vector3.up));


				// If the spawn is set to "pre"
				// wait the call time and spawn next minor wave if exist.
				HFMinorWave nextMinorWave = control.MinorWaves[control.TotalMinorWaveCleared + 1];

				if (nextMinorWave != null && nextMinorWave.SpawnType == SpawnType.Pre)
				{
					// Start the timer.
					HFTimer timer = new HFTimer(nextMinorWave.CallTime);
					StartCoroutine(timer.DecreaseTime(SpawnNextMinorWave));
				}
			}
		}
	}

	#region Wave Management
	/// <summary>
	/// Add wave to the collection.
	/// </summary>
	/// <param name="control"></param>
	public void AddWave(HFWaveControl control)
	{
		if (!WaveControls.ContainsValue(control))
			WaveControls.Add(control.WaveIndex, control);
	}

	/// <summary>
	/// Remove the wave from the collection.
	/// </summary>
	/// <param name="control"></param>
	public void RemoveWave(HFWaveControl control)
	{
		if (WaveControls.ContainsValue(control))
			WaveControls.Remove(control.WaveIndex);
	}

	/// <summary>
	/// Clear all wave collection.
	/// </summary>
	public void CleartWaveCollection()
	{
		WaveControls.Clear();
	}
	#endregion

	#region Utils

	public HFWaveControl GetCurrentWaveControl()
	{
		return WaveControls[CurrentWaveIndex];
	}

	public void ResetValues()
	{
		TotalWavesCleared = 0;
		CurrentWaveIndex = 0;

		CleartWaveCollection();
	}

	#endregion
}
