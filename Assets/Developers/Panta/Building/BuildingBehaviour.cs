using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BehaviorDesigner.Runtime;
using Types;
using HF.Unit;
using DG.Tweening;

public class BuildingBehaviour : EntityBehavior
{
    private BuildingView m_view;

    private BuildingsStatsSO m_buildingStats;
    private float m_attackDelayElapsed = 0;


    #region Stats

    public override void AssignStats(EntityStatsSO inStats)
    {
        m_buildingStats = inStats as BuildingsStatsSO;

        var attackRange = (SharedFloat)m_behaviorTree.GetVariable("AttackRange");
        attackRange.Value = m_buildingStats.AttackRange;

        Debug.Log(gameObject.name, gameObject);
        TakeVisualPrefab(m_buildingStats.BuildingType);

        CurrentHp = m_buildingStats.MaxHp;

        m_behaviorTree.enabled = true;
    }

    public override bool TakeDamage(int Damage)
    {
        return base.TakeDamage(Damage);
    }

    public void TakeVisualPrefab(BuildingType inType)
    {
        if (inType == BuildingType.CASTLE)
        {
            GameObject go = ObjectPooler.Instance.GetBuildingObject(inType);
            go.gameObject.transform.parent = this.transform;
            go.gameObject.layer = gameObject.layer;
            go.transform.localPosition = Vector3.zero;
            go.gameObject.SetActive(true);
        }
        else
        {
            m_view = ObjectPooler.Instance.GetBuildingObject(inType).GetComponent<BuildingView>();
            m_view.gameObject.transform.parent = this.transform;
            m_view.gameObject.layer = gameObject.layer;
            m_view.transform.localPosition = Vector3.zero;
            m_view.gameObject.SetActive(true);
        }
    }

    new public BuildingsStatsSO GetStats()
    {
        return m_buildingStats as BuildingsStatsSO;
    }

    #endregion

    public bool Carry(Vector3 carryPosition)
    {
        m_view.CarryBuilding();
        transform.DOJump(carryPosition, 5, 1, 0.5f);
        return true;
    }

    public bool Drop(Vector3 dropPosition)
    {
        if (Physics.CheckSphere(dropPosition + Vector3.up, 1))
            return false;

        m_view.DropBuilding();
        transform.DOJump(dropPosition, 5, 1, 0.5f);
        return true;
    }

    public override void Attack()
    {
        if (m_focusEntity != null)
        {
            Debug.Log($"i'm attacking {m_focusEntity.name}");
            if (m_attackDelayElapsed > m_buildingStats.AttackSpeed)
            {
                //m_view.SpawnBullet();
                StartCoroutine(DealDamge(m_focusEntity));
                m_attackDelayElapsed = 0;
            }
            else
            {
                m_attackDelayElapsed += Time.deltaTime;
            }
        }
    }

    IEnumerator DealDamge(EntityBehavior entity)
    {
        Troop target = entity.GetComponent<Troop>();
        float distance = Vector3.Distance(transform.position, m_focusEntity.transform.position);
        float time = distance / m_view.BulletSpeed;

        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }


        int count = target.UnitList.Count;

        for (int i = 0; i < count; i++)
        {
            if (target.UnitList[i].TakeDamage(m_buildingStats.Damage)) FocusEntity = null;
            break;
        }
    }
}
