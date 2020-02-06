using System.Collections.Generic;
using UnityEngine;

public enum InputType
{
	None = 0,
	Player = 1,
	AI = 2
}

public class HFUnit : MonoBehaviour
{
	#region Variables

	/*** Input */

	private HFController m_controller;

	public InputType ControllerType { get; protected set; }

	/*** Team */

	public int Team { get; protected set; }

	/*** Statistics */

	[SerializeField]
	private HFBaseStats m_baseStats = null;
	private Dictionary<HFStatistics, float> m_stats;

	[SerializeField]
	private HFStatUpgrade[] m_upgrades = new HFStatUpgrade[0];
	private IHFStatModifier[] m_mods;

	#endregion

	#region Core Loop

	void Awake()
	{
		HFHelpers.NullCheck(gameObject, m_baseStats, "base stats");

		UpdateModifiers();

		m_stats = new Dictionary<HFStatistics, float>();
		UpdateStats();
	}

	#endregion

	#region Statistics

	private void UpdateModifiers()
	{
		m_mods = new IHFStatModifier[m_upgrades.Length];
		for (int i = 0; i < m_mods.Length; i++)
		{
			m_mods[i] = m_upgrades[i] as IHFStatModifier;
		}
	}

	private void UpdateStats()
	{
		m_stats.Clear();
		HFStatistics[] allStats = HFHelpers.EnumToArray<HFStatistics>();
		foreach (HFStatistics stat in allStats)
		{
			m_stats.Add(stat, CalculateStat(stat));
		}
	}

	private float CalculateStat(HFStatistics stat)
	{
		return (m_baseStats.GetFloat(stat) + GetAddModifiers(stat)) * (1f + GetPctModifiers(stat));
	}

	private float GetAddModifiers(HFStatistics stat)
	{
		float total = 0f;
		foreach (IHFStatModifier mod in m_mods)
		{
			foreach (float add in mod.GetFloatAddModifiers(stat))
			{
				total += add;
			} 
		}
		return total;
	}

	private float GetPctModifiers(HFStatistics stat)
	{
		float totalPct = 0f;
		foreach (IHFStatModifier mod in m_mods)
		{
			foreach (float add in mod.GetPctModifiers(stat))
			{
				totalPct += add;
			}
		}
		return totalPct / 100f;
	}

	#endregion

	#region Input

	public void Possess(HFController controller)
	{
		if (!controller)
		{
			ControllerType = InputType.None;
		}
		else if (controller is HFAIController)
		{
			ControllerType = InputType.AI;
		}
		else
		{
			ControllerType = InputType.Player;
		}

		m_controller = controller;
	}

	public void UnPossess()
	{
		ControllerType = InputType.None;
		m_controller = null;
	}

	#endregion
}
