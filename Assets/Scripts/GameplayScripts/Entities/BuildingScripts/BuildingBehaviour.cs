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
    private GameObject m__viewObj;
    private BuildingView m_view;

    public BuildingsStatsSO m_buildingStats;
    public override EntityStatsSO EntityStats
    {
        get { return m_buildingStats; }
        set { m_buildingStats = (BuildingsStatsSO)value; }
    }
    private float m_attackDelayElapsed = 0;
	public GameObject RangeFeedback;

    private bool m_isFreezeMode = false;


    protected override void OnEnable()
    {
        base.Start();

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 2, Vector3.down, out hit, 20, LayerMask.GetMask("Terrain")))
        {
            transform.position = hit.point.SnapLocation();
        }

        HFEventManager.SubscribeTo<bool>(HFEventID.OnPauseMode, OnPause);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Deselected();
        
        HFEventManager.UnsubscribeFrom<bool>(HFEventID.OnPauseMode, OnPause);
    }

    #region Stats

    public override void AssignStats(EntityStatsSO inStats)
    {
        EntityStats = inStats;

        var buildingRef = (SharedBuilding)m_behaviorTree.GetVariable("BuildingRef");
        buildingRef.Value = this;
        var attackRange = (SharedFloat)m_behaviorTree.GetVariable("AttackRange");
        attackRange.Value = m_buildingStats.AttackRange;

        TakeVisualPrefab(m_buildingStats.BuildingType);

        CurrentHp = m_buildingStats.MaxHp;

        m_behaviorTree.enabled = true;
    }

    public override bool TakeDamage(int Damage)
    {
        m_currentHp -= Damage;

        if (m_currentHp <= 0) {
            Death();
            return true;
        }
        return false;
    }

    public void TakeVisualPrefab(BuildingType inType)
    {
        m__viewObj = ObjectPooler.Instance.GetBuildingObject(inType);
        m__viewObj.gameObject.transform.parent = this.transform;
        m__viewObj.gameObject.layer = gameObject.layer;
        m__viewObj.transform.localPosition = Vector3.zero;

        if (inType != BuildingType.CASTLE)
        {
            m_view = m__viewObj.GetComponent<BuildingView>();
        }
        m__viewObj.gameObject.SetActive(true);
    }

    new public BuildingsStatsSO GetStats()
    {
        return m_buildingStats;
    }

    #endregion

    private void OnPause(bool freeze)
    {
        m_isFreezeMode = freeze;
    }

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
        if (m_isFreezeMode) return;

        if (m_focusEntity != null)
        {
            Troop troop = m_focusEntity as Troop;
            Unit unit = troop.AliveUnit;

            if (unit != null)
            {
                Vector3 destinationPrediction = unit.transform.position + (unit.transform.forward * 1.5f);
                m_view.TrackOstile(destinationPrediction);
            }


            if (m_attackDelayElapsed > m_buildingStats.AttackSpeed)
            {
                m_view.SpawnBullet();
                StartCoroutine(DealDamge(m_focusEntity));
                m_attackDelayElapsed = 0;
            }
            else
            {
                m_attackDelayElapsed += Time.deltaTime;
            }
        }
    }

    public void TakeFocusTargetFromTree()
    {
        var troop = (SharedEntity)m_behaviorTree.GetVariable("FocusEntity");
        FocusEntity = troop.Value;
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

        if (target == null || entity == null || target.UnitList == null) yield return null;

        int count = target.UnitList.Count;

        if ((bool)m_behaviorTree.GetVariable("DamageMultipleTargets").GetValue())
        {
            for (int i = 0; i < count; i++)
            {
                if (target.UnitList != null && target.UnitList[i].TakeDamage(m_buildingStats.Damage))
                {
                    m_behaviorTree.SetVariableValue("FocusEntity", null);
                }
            }
        }
        else
        {
            Unit unit = null;

            for (int i = 0; i < count; i++)
            {
                if (target.UnitList != null && target.UnitList[i].UnitHp > 0)
                {
                    unit = target.UnitList[i];
                    break;
                }
            }

            if (unit == null) yield return null;

            if (unit.TakeDamage(m_buildingStats.Damage))
            {
                m_behaviorTree.SetVariableValue("FocusEntity", null);
            }
        }
    }

    protected override void DisableEntity()
    {
        StopTree(true);
        StopAllCoroutines();
        m_buildingStats = null;

        if (m__viewObj)
        {
            m__viewObj.gameObject.SetActive(false);
            m__viewObj.gameObject.transform.parent = null;
            m__viewObj = null;
        }
        this.gameObject.SetActive(false);
    }

    public override void Specialization(BuildingType type)
    {
        if (!GameController.Instance.CheckResourcesAvailability(GameController.Instance.Collection.BuildingsDictionary[type].OriginalBuildingStats.Cost))
        {
            HFEventManager.TriggerEvent(HFEventID.OnError, "You don't have enough resources");
            return;
        }

        Deselected();
        m_view.transform.SetParent(null);
        m_view.gameObject.SetActive(false);
        gameObject.SetActive(false);

        if (GameController.Instance.CreateNewBuilding(type, this.transform.position) != null)
        {
            base.Specialization(type);
            m_view?.UpdateTowerVisualState(true);
        }
    }

    public override void Death()
    {
        StopAllCoroutines();
        DisableEntity();
    }

    public override void Click()
    {
        base.Click();
 
            m_view?.UpdateTowerVisualState(true);
        
    }
    public override void Deselected()
    {
        base.Deselected();

        m_view?.UpdateTowerVisualState(false);
        
    }
}
