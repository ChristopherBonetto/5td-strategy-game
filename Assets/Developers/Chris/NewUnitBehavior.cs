using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Types;

public class NewUnitBehavior : MonoBehaviour, ITakeDamage, IAttack
{

    private NewTroopBehavior m_troopRef;
    public NewTroopBehavior TroopRef { get => m_troopRef; }

    public int CurrentHp;

    private UnitsStatsSO m_unitStats = null;
    private IAttackTypes m_unitAttackType { get => TroopRef.AttackType; }

    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JoinTroop(NewTroopBehavior inTroop)
    {
        gameObject.SetActive(true);
        gameObject.transform.parent = inTroop.transform;
        gameObject.layer = inTroop.gameObject.layer;

        m_troopRef = inTroop;

        m_unitStats = TroopRef.m_troopStats;

        //UnitAgent.speed = m_unitStats.UnitSpeed;
        RefreshHp();
    }

    private void RefreshHp()
    {
        CurrentHp = m_troopRef.m_troopStats.MaxHp;
    }

    public void LeaveTroop()
    {
        TroopRef.m_units.Remove(this);

        transform.parent = null;

        m_troopRef = null;

        gameObject.SetActive(false);
    }



    //TO DO: NEW COMMAND
    public void Attack()
    {
        if (m_unitStats.CanAttack)
        {
            if (m_unitAttackType.CanAttack(m_unitStats.AttackSpeed))
            {
                if (m_unitStats.AttackType == AttackType.MELEE)
                {
                    //if (m_unitAttackType.SingleAttack(FocusEntity, m_unitStats.Damage))
                    //{
                    //    //TO DO: Chiedi alla truppa un altro bersaglio del suo oggetto focus.
                    //    FocusEntity = null;
                    //}
                }
            }
        }
    }

    #region Take Damage

    public bool TakeDamage(int Damage)
    {
        if (m_unitStats.CanTakeDamage)
        {
            Damage = Mathf.Clamp(Damage, 1, m_unitStats.MaxHp + m_unitStats.Armor);

            if (CurrentHp <= Damage)
            {
                Debug.Log("Death");
                Death();
                return true;
            }
            {
                CurrentHp -= Damage;
                Debug.Log(CurrentHp);
                return false;
            }
        }
        return false;
    }

    public void Death()
    {
        TroopRef.TroopTakeDamage(this);
    }

    #endregion
}
