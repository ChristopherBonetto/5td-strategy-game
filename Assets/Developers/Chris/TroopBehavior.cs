using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using Types;

public class TroopBehavior : EntityBehavior, ICanMove
{
    public TroopBehavior(UnitsStatsSO inStat)
    {
        m_unitStats = inStat;
    }

    public UnitsStatsSO m_unitStats;
    private UnitBehavior[] m_troopUnits;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            ResetStats();
        }
    }

    

    public override void AssignStats(EntityStatsSO inStats)
    {
        if(inStats is UnitsStatsSO)
        {
            base.AssignStats(inStats);
            m_unitStats = (UnitsStatsSO)inStats;
            CreateUnit(m_unitStats.UnitType, m_unitStats.TroopsQuantity);
        }
        else
        {
            Debug.LogWarning("This unit can take stats from: " + inStats.Name);
        }
    }


    public void CreateUnit(UnitType inType, int inValue)
    {
        m_troopUnits = new UnitBehavior[inValue];

        for (int i = 0; i < m_troopUnits.Length; i++)
        {
            GameObject tempUnit = ObjectPooler.SharedInstance.GetUnityObject(inType);

            UnitBehavior tempRef = tempUnit.GetComponent<UnitBehavior>();

            if(tempRef == null)
            {
                Debug.Log(inType + "didn't have UnitBehavior script, pls add next time");
                return;
            }
            m_troopUnits[i] = AssignUnit(tempRef);
        }
    }

    public void ResetStats()
    {
        foreach(UnitBehavior unit in m_troopUnits)
        {
            DeassignUnit(unit);
        }
        m_troopUnits = null;

        m_unitStats = null;

        gameObject.SetActive(false);
        //Return to the pool
    }

    public UnitBehavior AssignUnit(UnitBehavior inUnit)
    {
        inUnit.gameObject.SetActive(true);
        inUnit.AssignTroop(this);
        inUnit.gameObject.transform.parent = this.transform;
        inUnit.gameObject.transform.position = gameObject.transform.position;
        inUnit.gameObject.layer = gameObject.layer;
        return inUnit;
    }

    public void DeassignUnit(UnitBehavior inUnit)
    {
        inUnit.transform.parent = null;
        inUnit.gameObject.SetActive(false);
    }


    public void MoveFromTo(Vector3 endPosition)
    {
        foreach (UnitBehavior unit in m_troopUnits)
        {
            unit.UnitAgent.destination = endPosition;
        }
    }

    public override void Clicked()
    {
        base.Clicked();
    }
}
