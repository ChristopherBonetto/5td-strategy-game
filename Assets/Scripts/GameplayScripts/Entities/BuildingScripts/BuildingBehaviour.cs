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

    public BuildingsStatsSO m_buildingStats;
    public override EntityStatsSO EntityStats
    {
        get { return m_buildingStats; }
        set { m_buildingStats = (BuildingsStatsSO)value; }
    }
    private float m_attackDelayElapsed = 0;


    #region Stats

    public override void AssignStats(EntityStatsSO inStats)
    {
        EntityStats = inStats;

        var buildingRef = (SharedBuilding)m_behaviorTree.GetVariable("BuildingRef");
        buildingRef.Value = this;
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
        return m_buildingStats;
    }

    #endregion

    public bool Carry(Vector3 carryPosition)
    {
        IsBusy = true;
        m_view.CarryBuilding();
        transform.DOJump(carryPosition, 5, 1, 0.1f);
        transform.up = Vector3.up;
        StopTree(true);
        return true;
    }

    public bool Drop(Vector3 dropPosition)
    {
        Vector3 fixedUpDirection = Vector3.up;
        RaycastHit hit;

        if (Physics.Raycast(dropPosition + Vector3.up * 1.5f, Vector3.down, out hit, 5f))
        {
            fixedUpDirection = hit.normal;
        }

        m_view.DropBuilding();
        transform.DOJump(dropPosition, 5, 1, 0.25f);
        transform.up = fixedUpDirection;
        IsBusy = false;
        StopTree(false);
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
