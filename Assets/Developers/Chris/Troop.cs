using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;

public class Troop : Entity
{
    public UnitsStatsSO m_troopStats;
    public override EntityStatsSO EntityStats
    {
        get { return m_troopStats; }
        set { m_troopStats = (UnitsStatsSO)value; }
    }

    public override void AssignStats(EntityStatsSO inStats)
    {
        base.AssignStats(inStats);
        CreateUnits(m_troopStats.UnitType, m_troopStats.UnitQuantity);
    }

    public void CreateUnits(UnitType inType, int inValue)
    {
        for (int i = 0; i < inValue; i++)
        {
            GameObject tempUnit = ObjectPooler.Instance.GetUnityObject(inType);
            tempUnit.transform.parent = this.transform;
            tempUnit.transform.localPosition = Vector3.zero;
            tempUnit.SetActive(true);
        }
    }
}
