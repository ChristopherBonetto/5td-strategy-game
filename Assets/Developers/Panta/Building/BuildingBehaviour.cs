using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BehaviorDesigner.Runtime;
using Types;

public class BuildingBehaviour : EntityBehavior
{
    private BuildingsStatsSO m_buildingStats;

    #region Stats

    public override void AssignStats(EntityStatsSO inStats)
    {
        m_buildingStats = inStats as BuildingsStatsSO;

        var attackRange = (SharedFloat)m_behaviorTree.GetVariable("AttackRange");
        attackRange.Value = m_buildingStats.AttackRange;

        TakeVisualPrefab(m_buildingStats.BuildingType);

        CurrentHp = m_buildingStats.MaxHp;

        m_behaviorTree.enabled = true;
    }

    public void TakeVisualPrefab(BuildingType inType)
    {
        GameObject building = ObjectPooler.Instance.GetBuildingObject(inType);
        building.gameObject.transform.parent = this.transform;
        building.gameObject.layer = gameObject.layer;
        building.transform.localPosition = Vector3.zero;
        building.gameObject.SetActive(true);
    }

    public BuildingsStatsSO GetStats()
    {
        return m_buildingStats as BuildingsStatsSO;
    }

    #endregion
}
