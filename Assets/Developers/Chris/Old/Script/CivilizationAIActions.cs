using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilizationAIActions : CivilizationAction
{
    //private float m_Timer = 0;

    //public override void Awake()
    //{
    //    base.Awake();
    //}

    //public override void Start()
    //{
    //    TestStartingInstantiate();

    //    StartCoroutine("StartInput");
    //}

    //IEnumerator StartInput()
    //{
    //    yield return new WaitForSeconds(1f);
    //    ComandToAttackCastle();
    //}

    //private void ComandToAttackCastle()
    //{
    //    foreach (UnitActions unit in CivilizationUnits)
    //    {
    //        unit.FocusObject = GameManager.Instance.PlayerCastle.gameObject;
    //        unit.ChangeUnitState(Actions.Attack);
    //    }
    //}

    //public virtual bool Timer(float destinationTime)
    //{
    //    m_Timer += Time.deltaTime;

    //    if (m_Timer >= destinationTime)
    //    {
    //        m_Timer = 0f;
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}
}
