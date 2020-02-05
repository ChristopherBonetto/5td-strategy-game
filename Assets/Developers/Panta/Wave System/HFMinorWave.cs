using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represent the sub-wave of each wave.
/// Link the troop to spawn point.
/// </summary>
[System.Serializable]
public class HFMinorWave
{
	public SpawnType SpawnType;

	[Tooltip("Total time to call this minor wave")]
	public float CallTime;

	[Tooltip("Assigne prefab of the troop")]
	public GameObject PrefabToSpawn;

	[Tooltip("Drop here the spawn point of the troop to spawn")]
	public Transform SpawnPoint;

	private bool m_IsCleared;
	public bool IsCleared
	{
		get { return m_IsCleared; }
		set
		{
			m_IsCleared = value;
			
			if (value == true)
			{
				HFWaveControl control = HFWaveManager.Instance.GetCurrentWaveControl();
				control.TotalMinorWaveCleared++;
			}
		}
	}

	public void MarkAsCleared()
	{
		IsCleared = true;
	}
}
