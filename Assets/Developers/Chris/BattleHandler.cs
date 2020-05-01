using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Fight
{
    public TroopBehavior PlayerTroop;
    public TroopBehavior EnemyTroop;

    public Fight(TroopBehavior inPlayer, TroopBehavior inEnemy)
    {
        PlayerTroop = inPlayer;
        EnemyTroop = inEnemy;

        //Prende l'oggetto dalla pool e li assegna le due unita
        TakeBattleHandler();
    }

    private void TakeBattleHandler()
    {
        GameObject battleHandler = ObjectPooler.Instance.GetPooledObject("BattleHandler");

        BattleHandler battleScript = battleHandler.GetComponent<BattleHandler>();

        if(battleScript != null)
        {
            battleScript.StartFight(PlayerTroop, EnemyTroop);
        }
    }
}


public class BattleHandler : MonoBehaviour
{
    private TroopBehavior playerTroop;
    private TroopBehavior enemyTroop;

    public void StartFight(TroopBehavior inPlayer, TroopBehavior inEnemy)
    {
        playerTroop = inPlayer;
        enemyTroop = inEnemy;

        playerTroop.FocusEntity = enemyTroop;
        enemyTroop.FocusEntity = playerTroop;

        playerTroop.Stop(true);
        enemyTroop.Stop(true);

        playerTroop.ChangeInCombat(true);
        enemyTroop.ChangeInCombat(true);

        inPlayer.currentBattle = this;
        inEnemy.currentBattle = this;

        int enemyIndex = 0;
        int playerIndex = 0;

        for (int i = 0; i < playerTroop.m_units.Count; i++)
        {
            playerTroop.m_units[i].FocusEntity = enemyTroop.m_units[enemyIndex];

            enemyIndex++;
            enemyIndex = (int)Mathf.Repeat(0f, (float)enemyTroop.m_units.Count);
        }
        for (int i = 0; i < enemyTroop.m_units.Count; i++)
        {
            enemyTroop.m_units[i].FocusEntity = playerTroop.m_units[playerIndex];

            playerIndex++;
            playerIndex = (int)Mathf.Repeat(0f, (float)playerTroop.m_units.Count);
        }
    }

    public void TakeOtherTarget(UnitBehavior inUnit)
    {
        if (playerTroop.m_units.Contains(inUnit))
        {
            inUnit.FocusEntity = TakeAnotherTargerFromTroop(inUnit, enemyTroop);
        }
        else if (enemyTroop.m_units.Contains(inUnit))
        {
            inUnit.FocusEntity = TakeAnotherTargerFromTroop(inUnit, playerTroop);
        }
        else
        {
            Debug.Log("No one troop contain " + inUnit.name);
        }
        
    }

    private UnitBehavior TakeAnotherTargerFromTroop(UnitBehavior inUnit, TroopBehavior inTroop)
    {
        float distance = 1000f;
        UnitBehavior tempUnit = null;

        for (int i = 0; i < inTroop.m_units.Count; i++)
        {
            if (inTroop.m_units[i].gameObject.active)
            {
                if(Vector3.Distance(inUnit.transform.position, inTroop.m_units[i].transform.position) < distance)
                {
                    tempUnit = inTroop.m_units[i];
                }
            }
        }

        if(tempUnit == null)
        {
            FinishFight();
            return null;
        }
        
        return tempUnit;
    }

    public void FinishFight()
    {
        Debug.Log("Fight completed");
        ResetFocusEntity(playerTroop);
        ResetFocusEntity(enemyTroop);

        gameObject.SetActive(false);
    }

    public void ResetFocusEntity(TroopBehavior inTroop)
    {
        foreach(UnitBehavior unit in inTroop.m_units)
        {
            unit.FocusEntity = null;
        }

        inTroop.currentBattle = null;
        inTroop.FocusEntity = null;
    }

}
