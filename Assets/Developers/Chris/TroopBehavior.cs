using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using Types;

public class TroopBehavior : EntityBehavior, ICanMove, ITakeUpgrade
{
    public TroopBehavior(UnitsStatsSO inStat)
    {
        m_troopStats = inStat;
    }

    public UnitsStatsSO m_troopStats;
    public override EntityStatsSO EntityStats
    {
        get
        {
            return m_troopStats;
        }
        set
        {
            m_troopStats = (UnitsStatsSO)value;
        }
    }



    public UnitBehavior[] m_units;

    private Vector3[] m_formationPosition = new Vector3[4];

    private int Xsize;
    private int Zsize;

    [SerializeField] private Transform m_destinationPoint;

    

    #region Create new troop with unit

    public override void AssignStats(EntityStatsSO inStats)
    {
        base.AssignStats(inStats);
        CreateUnit(m_troopStats.UnitType, m_troopStats.TroopsQuantity);
    }



    public void CreateUnit(UnitType inType, int inValue)
    {
        m_units = new UnitBehavior[inValue];

        for (int i = 0; i < m_units.Length; i++)
        {
            GameObject tempUnit = ObjectPooler.SharedInstance.GetUnityObject(inType);

            UnitBehavior tempRef = tempUnit.GetComponent<UnitBehavior>();

            if(tempRef == null)
            {
                Debug.Log(inType + "didn't have UnitBehavior script, pls add next time");
                return;
            }
            m_units[i] = AssignUnit(tempRef);
        }
        CreateSquareFormation(1f);
    }

    public void ResetStats()
    {
        foreach(UnitBehavior unit in m_units)
        {
            DeassignUnit(unit);
        }
        m_units = null;

        m_troopStats = null;

        gameObject.SetActive(false);
        //Return to the pool
    }

    public UnitBehavior AssignUnit(UnitBehavior inUnit)
    {
        inUnit.gameObject.SetActive(true);
        inUnit.gameObject.transform.parent = this.transform;
        inUnit.gameObject.layer = gameObject.layer;
        inUnit.JoinTroop(this);
        return inUnit;
    }

    public void DeassignUnit(UnitBehavior inUnit)
    {
        inUnit.transform.parent = null;
        inUnit.LeaveTroop();
        inUnit.gameObject.SetActive(false);
    }

    #endregion

    #region Troop Formation

    public void CreateSquareFormation(float inOffset = 1)
    {
        if(m_troopStats == null || m_units.Length == 0)
        {
            return;
        }

        Xsize = Mathf.RoundToInt(m_units[0].transform.localScale.x);
        Zsize = Mathf.RoundToInt(m_units[0].transform.localScale.z);
        m_formationPosition = new Vector3[4];

        switch (m_units.Length)
        {
            case 1:
                m_formationPosition[0] = new Vector3(transform.position.x, transform.position.y, inOffset + Zsize / 2);
                break;

            case 2:
                m_formationPosition[0] = new Vector3(-inOffset - Xsize / 2, transform.position.y, transform.position.z);
                m_formationPosition[1] = new Vector3(inOffset + Xsize / 2, transform.position.y, transform.position.z);
                break;

            case 3:
                m_formationPosition[0] = new Vector3(-inOffset - Xsize / 2, transform.position.y, inOffset + Zsize / 2);
                m_formationPosition[1] = new Vector3(inOffset + Xsize / 2, transform.position.y, inOffset + Zsize / 2);
                m_formationPosition[2] = new Vector3(transform.position.x, transform.position.y, -inOffset - Zsize / 2);
                break;

            case 4:
                m_formationPosition[0] = new Vector3(-inOffset - Xsize / 2, transform.position.y, inOffset + Zsize / 2);
                m_formationPosition[1] = new Vector3(inOffset + Xsize / 2, transform.position.y, inOffset + Zsize / 2);
                m_formationPosition[2] = new Vector3(-inOffset - Xsize / 2, transform.position.y, -inOffset - Zsize / 2);
                m_formationPosition[3] = new Vector3(inOffset + Xsize / 2, transform.position.y, -inOffset - Zsize / 2);
                break;

        }
        AssignFormation(m_formationPosition);
    }

    public void CreateTriangleFormation(float inOffSet)
    {

    }

    public void AssignFormation(Vector3[] inPos)
    {
        for(int i = 0; i < m_units.Length; i++)
        {
            m_units[i].transform.localPosition = inPos[i];
        }
    }

    #endregion

    #region Move inteface

    public void MoveFromTo(Vector3 endPosition)
    {
        m_destinationPoint.position = endPosition;

        for(int i = 0; i < m_units.Length; i++)
        {
            m_units[i].MoveFromTo(m_destinationPoint.position + m_formationPosition[i]);
        }
    }

    #endregion

    #region Click interface

    public override void Select()
    {
        base.Select();
    }

    public override void Interact(EntityBehavior inEntity)
    {
        base.Interact(inEntity);

        if(inEntity.m_entityPlayerType != this.m_entityPlayerType)
        {
            if(inEntity is TroopBehavior)
            {
                TroopBehavior tempTroop = (TroopBehavior)inEntity;

                if (m_troopStats.CanAttack && tempTroop.m_troopStats.CanTakeDamage)
                {
                    for (int i = 0; i < m_units.Length; i++)
                    {
                        m_units[i].UnitFocusObj = tempTroop.m_units[i].gameObject;
                        m_units[i].UnitAgent.SetDestination(m_units[i].UnitFocusObj.transform.position);
                        Debug.Log(m_units[i] + " go to attack " + tempTroop.m_units[i]);
                    }
                }
                else
                {
                    Debug.Log("unit can't attack or other troop can't take damage : PLS CHECK ON SCRIPTABLE");
                }
            }
        }
    }

    #endregion
}
