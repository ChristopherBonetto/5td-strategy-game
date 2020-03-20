using System;
using System.Collections.Generic;
using UnityEngine;

public enum HFUnitType
{
	PlaceHolder = 0,
	Unit = 1,
	Turret = 2,
	Castle = 3
}

public enum HFStatistics
{
	Dummy = 0,
	MaxHealth = 1,
	Speed = 2,
	AttackRange = 3,
	AttackRate = 4,
	UnitDamage = 5,
	BuildingDamage = 6,
	CarryCapacity = 7,
	Weight = 8,
	RewardValue = 9,
	ShootAngle = 10,
	BulletSpeed = 11,
	UnitRespawnDelay = 12,
	SoldierRespawnDelay = 13,
	SoldiersPerUnit = 14,


	Name = 20,
	Description = 21
}

public enum HFRewardCondition
{
	NoReward = 0,
	Kill = 1,
	Survive = 2
}

[Serializable]
public abstract class HFKeyVal<T>
{
	public HFStatistics Key;
	public T Value;
}

[Serializable]
public class HFFloatKV : HFKeyVal<float>
{}

[Serializable]
public class HFIntKV : HFKeyVal<int>
{}

[Serializable]
public class HFStringKV : HFKeyVal<string>
{ }

[Serializable]
public class HFBoolKV : HFKeyVal<bool>
{ }

[CreateAssetMenu(fileName = "Stats_", menuName = "GoodNorth/Statistics")]
public class HFBaseStats : ScriptableObject
{
	[Serializable]
	public struct Upgrades
	{
		public List<HFStatUpgrade> List;
	}

	[SerializeField]
	public Sprite Icon = null;

	[SerializeField]
	public HFUnitType UnitType = HFUnitType.PlaceHolder;

	[SerializeField]
	public HF.HFUnitVisuals Visuals = null;

	[SerializeField]
	private Upgrades[] m_levels = new Upgrades[3];
	public Upgrades[] Levels => m_levels;

	[SerializeField]
	public HFRewardCondition RewardCondition = HFRewardCondition.NoReward;

	void OnValidate()
	{
		UpdateAll();
	}

	#region Floats

	[SerializeField]
	protected HFFloatKV[] m_floatStats = new HFFloatKV[0];

	private Dictionary<HFStatistics, float> FloatDict = new Dictionary<HFStatistics, float>();

	public float GetFloat(HFStatistics stat)
	{
		if (FloatDict.Count == 0)
		{
			UpdateDict(FloatDict, m_floatStats);
		}
		return GetVal(FloatDict, stat);
	}

	#endregion

	#region Ints

	[SerializeField]
	protected HFIntKV[] m_intStats = new HFIntKV[0];

	private Dictionary<HFStatistics, int> IntDict = new Dictionary<HFStatistics, int>();

	public int GetInt(HFStatistics stat)
	{
		if (IntDict.Count == 0)
		{
			UpdateDict(IntDict, m_intStats);
		}
		return GetVal(IntDict, stat);
	}

	#endregion

	#region Strings

	[SerializeField]
	protected HFStringKV[] m_stringStats = new HFStringKV[0];

	private Dictionary<HFStatistics, string> StringDict = new Dictionary<HFStatistics, string>();

	public string GetString(HFStatistics stat)
	{
		if (StringDict.Count == 0)
		{
			UpdateDict(StringDict, m_stringStats);
		}
		return GetVal(StringDict, stat);
	}

	#endregion

	#region Bools

	[SerializeField]
	protected HFBoolKV[] m_boolStats = new HFBoolKV[0];

	private Dictionary<HFStatistics, bool> BoolDict = new Dictionary<HFStatistics, bool>();

	public bool GetBool(HFStatistics stat)
	{
		if (BoolDict.Count == 0)
		{
			UpdateDict(BoolDict, m_boolStats);
		}
		return GetVal(BoolDict, stat);
	}

	#endregion

	#region Helpers

	public T GetVal<T>(Dictionary<HFStatistics, T> dict, HFStatistics stat)
	{
		if (!dict.TryGetValue(stat, out T retValue))
		{
			//LogMissing(retValue.GetType().Name, stat.ToString());
		}
		return retValue;
	}

	public void UpdateDict<T>(Dictionary<HFStatistics, T> dict, HFKeyVal<T>[] values)
	{
		dict.Clear();

		foreach (HFKeyVal<T> pair in values)
		{
			if (!dict.ContainsKey(pair.Key))
			{
				dict.Add(pair.Key, pair.Value);
			}
			else
			{
				//LogDuplicate(pair.Key.ToString(), dict.ToString());
			}
		}
	}

	[ContextMenu("Update all")]
	public void UpdateAll()
	{
		UpdateDict(FloatDict, m_floatStats);
		UpdateDict(IntDict, m_intStats);
		UpdateDict(StringDict, m_stringStats);
		UpdateDict(BoolDict, m_boolStats);
	}

	private void LogMissing(string type, string stat)
	{
		Debug.Log("Missing " + type + " data " + stat);
	}

	private void LogDuplicate(string key, string dict)
	{
		Debug.Log("Duplicate entry " + key + " in filling " + dict);
	}

	#endregion
}
