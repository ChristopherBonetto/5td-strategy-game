using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Fight
{
    public Troop PlayerTroop;
    public Troop EnemyTroop;

    public Fight(Troop inPlayer, Troop inEnemy)
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

        if (battleScript != null)
        {
            battleScript.StartFight(PlayerTroop, EnemyTroop);
        }
    }
}


public class BattleHandler : MonoBehaviour
{
    private Troop playerTroop;
    private Troop enemyTroop;

    public void StartFight(Troop inPlayer, Troop inEnemy)
    {
        playerTroop = inPlayer;
        enemyTroop = inEnemy;

        playerTroop.FocusEntity = enemyTroop;
        enemyTroop.FocusEntity = playerTroop;

        playerTroop.StopTree(true);
        enemyTroop.StopTree(true);

        playerTroop.IsBusy = true;
        enemyTroop.IsBusy = true;

        inPlayer.m_currentBattle = this;
        inEnemy.m_currentBattle = this;

        int enemyIndex = 0;
        int playerIndex = 0;

        for (int i = 0; i < playerTroop.UnitList.Count; i++)
        {
            playerTroop.UnitList[i].AssignFocusUnit(enemyTroop.UnitList[enemyIndex]);

            playerTroop.UnitList[i].StopTree(false);
            enemyIndex++;
            enemyIndex = (int)Mathf.Repeat(enemyIndex, (float)enemyTroop.UnitList.Count);
        }
        for (int i = 0; i < enemyTroop.UnitList.Count; i++)
        {
            enemyTroop.UnitList[i].AssignFocusUnit(playerTroop.UnitList[playerIndex]);

            enemyTroop.UnitList[i].StopTree(false);
            playerIndex++;
            playerIndex = (int)Mathf.Repeat(playerIndex, (float)playerTroop.UnitList.Count);
        }
    }

    public void TakeOtherTarget(Unit inUnit)
    {
        if (playerTroop.UnitList.Contains(inUnit))
        {
            inUnit.AssignFocusUnit(TakeAnotherTargerFromTroop(inUnit, enemyTroop));
        }
        else if (enemyTroop.UnitList.Contains(inUnit))
        {
            inUnit.AssignFocusUnit(TakeAnotherTargerFromTroop(inUnit, playerTroop));
        }
        else
        {
            Debug.Log("No one troop contain " + inUnit.name);
        }

    }

    private Unit TakeAnotherTargerFromTroop(Unit inUnit, Troop inTroop)
    {
        float distance = 1000f;
        Unit tempUnit = null;

        for (int i = 0; i < inTroop.UnitList.Count; i++)
        {
            if (inTroop.UnitList[i].gameObject.active)
            {
                if (Vector3.Distance(inUnit.transform.position, inTroop.UnitList[i].transform.position) < distance)
                {
                    tempUnit = inTroop.UnitList[i];
                }
            }
        }

        if (tempUnit == null)
        {
            FinishFight();
            return null;
        }

        return tempUnit;
    }

    public void FinishFight()
    {
        Debug.Log("Fight completed");
        playerTroop.SetIdleState();
        enemyTroop.SetIdleState();

        gameObject.SetActive(false);
    }
}

