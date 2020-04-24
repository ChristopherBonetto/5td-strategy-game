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

    public override int CurrentHp
    {
        get
        {
            return TakeTroopHealth();
        }
    }

    public List<UnitBehavior> m_units = new List<UnitBehavior>();

    private Vector3[] m_formationPosition = new Vector3[4];

    private int Xsize;
    private int Zsize;


    [SerializeField] private Transform m_destinationPoint;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            m_units[0].TakeDamage(0);
        }
    }

    #region Troop management

    public override void AssignStats(EntityStatsSO inStats)
    {
        base.AssignStats(inStats);
        CreateUnits(m_troopStats.UnitType, m_troopStats.TroopsQuantity);
    }

    public void CreateUnits(UnitType inType, int inValue)
    {
        m_units = new List<UnitBehavior>(inValue);

        for (int i = 0; i < inValue; i++)
        {
            GameObject tempUnit = ObjectPooler.SharedInstance.GetUnityObject(inType);

            UnitBehavior tempRef = tempUnit.GetComponent<UnitBehavior>();

            if(tempRef == null)
            {
                Debug.Log(inType + "didn't have UnitBehavior script, pls add next time");
                return;
            }
            m_units.Add(AssignUnit(tempRef));
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
        inUnit.JoinTroop(this);
        return inUnit;
    }

    public void DeassignUnit(UnitBehavior inUnit)
    {
        if (!m_units.Contains(inUnit))
        {
            return;
        }
        inUnit.LeaveTroop();
    }

    #endregion

    #region Troop Formation

    public void CreateSquareFormation(float inOffset = 1)
    {
        if(m_troopStats == null || m_units.Count == 0)
        {
            return;
        }

        Xsize = Mathf.RoundToInt(m_units[0].transform.localScale.x);
        Zsize = Mathf.RoundToInt(m_units[0].transform.localScale.z);
        m_formationPosition = new Vector3[4];

        switch (m_units.Count)
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
        for(int i = 0; i < m_units.Count; i++)
        {
            m_units[i].transform.localPosition = inPos[i];
        }
    }

    #endregion

    #region Move inteface

    //Muovi la truppa e le unita.
    public void MoveFromTo(Vector3 endPosition)
    {
        m_destinationPoint.position = endPosition;

        for(int i = 0; i < m_units.Count; i++)
        {
            Vector3 destination = m_destinationPoint.position + m_formationPosition[i];
            m_units[i].MoveFromTo(m_destinationPoint.position + m_formationPosition[i]);
        }
    }

    #endregion

    #region Click interface

    public override void Select()
    {
        base.Select();
    }

    //Come la truppa interagisce con le altre entity.
    public override void Interact(EntityBehavior inEntity)
    {
        base.Interact(inEntity);

        if(inEntity.EntityPlayerType != this.EntityPlayerType)
        {
            if(inEntity is TroopBehavior)
            {
                TroopBehavior tempTroop = (TroopBehavior)inEntity;

                if (m_troopStats.CanAttack && tempTroop.m_troopStats.CanTakeDamage)
                {
                    for (int i = 0; i < m_units.Count; i++)
                    {
                        m_units[i].FocusEntity = tempTroop.m_units[i];
                        m_units[i].UnitAgent.SetDestination(m_units[i].FocusEntity.transform.position);
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

    #region Troop health

    //Prende la vita totale
    public int TakeTroopHealth()
    {
        int health = 0;

        if(m_units.Count == 0)
        {
            Debug.Log("This troop don't have units");
            return health;
        }

        for(int i = 0; i < m_units.Count; i++)
        {
            health += m_units[i].CurrentHp;
        }
        
        return health;
    }

    //Non usato
    public override bool TakeDamage(int Damage = 0)
    {
        return true;
    }

    //Come prende danno la truppa
    public void TroopTakeDamage(UnitBehavior inUnit)
    {
        inUnit.LeaveTroop();

        if(CurrentHp == 0)
        {
            if(EntityPlayerType == PlayerType.AI)
            {
                gameObject.SetActive(false);
                return;
            }
            else
            {
                StartCoroutine("Respawn");
            }
        }
    }

    public override void Death()
    {
        base.Death();
    }

    //Respawna le unita dopo un timer
    IEnumerator Respawn()
    {
        transform.position = new Vector3(0, 0.5f, 0);
        yield return new WaitForSeconds(m_troopStats.RespawnTime);
        CreateUnits(m_troopStats.UnitType, m_troopStats.TroopsQuantity);
    }

    #endregion
}
