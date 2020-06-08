using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Fight
{
    public Troop Attacker;
    public EntityBehavior Defender;

    public Fight(Troop inPlayer, EntityBehavior inEnemy)
    {
        Attacker = inPlayer;
        Defender = inEnemy;

        //Prende l'oggetto dalla pool e li assegna le due unita
        TakeBattleHandler();
    }

    private void TakeBattleHandler()
    {
        GameObject battleHandler = ObjectPooler.Instance.GetPooledObject("BattleHandler");

        BattleHandler battleScript = battleHandler.GetComponent<BattleHandler>();

        if (battleScript != null)
        {
            battleScript.StartFight(Attacker, Defender);
        }

        Vector3 objectLine = (Defender.transform.position - Attacker.transform.position); // This operation doesn't give you the midlle point between attacker and defender. 
        // If you want to position the Battle handler in the midlle of the fight just do the follow operations:
        // (V1 + V2) / 2.   (or in this case --> (attacker.position + defender.positon) / 2)
        // This change also the Y axis, give some offset if you wish.

        float distance = objectLine.magnitude;
        objectLine = objectLine.normalized; // Why normalize? Do you want to spawn it in 0,0,0 + some distance?
        battleHandler.gameObject.SetActive(true);
        battleHandler.transform.position = objectLine;
    }
}


public class BattleHandler : MonoBehaviour
{
    private Troop attacker;
    private EntityBehavior defender;

    public void StartFight(Troop inAttacker, EntityBehavior inDefender)
    {
        attacker = inAttacker;
        defender = inDefender;

        attacker.FocusEntity = defender;
        defender.FocusEntity = attacker;

        attacker.StopTree(true);
        defender.StopTree(true);

        if(!defender is BuildingBehaviour)
        {
            attacker.IsBusy = true;
        }
        
        defender.IsBusy = true;

        inAttacker.m_currentBattle = this;

        // Case the defender is a troop.
        if (inDefender is Troop)
        {
            Troop defenderT = inDefender as Troop;
            defenderT.m_currentBattle = this;

            int enemyIndex = 0;
            int playerIndex = 0;

            for (int i = 0; i < attacker.UnitList.Count; i++)
            {
                if (defenderT.UnitList.Count == 0) {
                    FinishFight();
                    goto end;   // make sure to exit from the loop
                }

                enemyIndex = Mathf.Max(0, enemyIndex);
                attacker.UnitList[i].AssignFocusUnit(defenderT.UnitList[enemyIndex]);

                attacker.UnitList[i].StopTree(false);
                enemyIndex = (enemyIndex + 1) % defenderT.UnitList.Count;
            }
            for (int i = 0; i < defenderT.UnitList.Count; i++)
            {
                if (attacker.UnitList.Count == 0) {
                    FinishFight();
                    goto end;   // make sure to exit from the loop
                }

                playerIndex = Mathf.Max(0, playerIndex);
                defenderT.UnitList[i].AssignFocusUnit(attacker.UnitList[playerIndex]);

                defenderT.UnitList[i].StopTree(false);
                playerIndex = (playerIndex + 1) % attacker.UnitList.Count;
            }
        }
        // Case the defender is a building
        else if (inDefender is BuildingBehaviour)
        {
            BuildingBehaviour defenderB = inDefender as BuildingBehaviour;

            for (int i = 0; i < attacker.UnitList.Count; i++)
            {
                attacker.UnitList[i].AssignFocusBuilding(defenderB);

                attacker.UnitList[i].StopTree(false);
            }
        }
    end:; // The goal of the "goto" keyword.
    }

    public void TakeOtherTarget(Unit inUnit)
    {
        if (!(defender is Troop)) return;

        Troop defenderT = defender as Troop;

        if (attacker.UnitList.Contains(inUnit))
        {
            inUnit.AssignFocusUnit(TakeAnotherTargerFromTroop(inUnit, defenderT));
        }
        else if (defenderT.UnitList.Contains(inUnit))
        {
            inUnit.AssignFocusUnit(TakeAnotherTargerFromTroop(inUnit, attacker));
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
            if (inTroop.UnitList[i].gameObject.activeSelf)
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
        attacker.SetIdleState();

        if (defender is Troop)
        {
            Troop defenderT = defender as Troop;
            defenderT.SetIdleState();
        }

        gameObject.SetActive(false);
    }
}