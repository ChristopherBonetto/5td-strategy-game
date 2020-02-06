using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Upgrades_", menuName = "GoodNorth/Upgrades")]
public class HFStatUpgrade : ScriptableObject, IHFStatModifier
{
	[SerializeField]
	HFFloatKV[] m_floatAddModifiers = new HFFloatKV[0];

	[SerializeField]
	HFFloatKV[] m_pctModifiers = new HFFloatKV[0];

	[SerializeField]
	HFIntKV[] m_intAddModifiers = new HFIntKV[0];

	public IEnumerable<float> GetFloatAddModifiers(HFStatistics stat)
	{
		for (int i = 0; i < m_floatAddModifiers.Length; i++)
		{
			if (m_floatAddModifiers[i].Key == stat)
			{
				yield return m_floatAddModifiers[i].Value;
			}
		}
	}

	public IEnumerable<float> GetPctModifiers(HFStatistics stat)
	{
		for (int i = 0; i < m_pctModifiers.Length; i++)
		{
			if (m_pctModifiers[i].Key == stat)
			{
				yield return m_pctModifiers[i].Value;
			}
		}
	}

	public IEnumerable<int> GetIntAddModifiers(HFStatistics stat)
	{
		for (int i = 0; i < m_intAddModifiers.Length; i++)
		{
			if (m_intAddModifiers[i].Key == stat)
			{
				yield return m_intAddModifiers[i].Value;
			}
		}
	}
}
