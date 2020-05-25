using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Fight
{
    public Troop Attacker;
    public Troop Defender;

    public Fight(Troop inPlayer, Troop inEnemy)
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

        Vector3 objectLine = (Defender.transform.position - Attacker.transform.position);
        float distance = objectLine.magnitude;
        objectLine = objectLine.normalized;
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

        attacker.IsBusy = true;
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
                attacker.UnitList[i].AssignFocusUnit(defenderT.UnitList[enemyIndex]);

                attacker.UnitList[i].StopTree(false);
                enemyIndex++;
                enemyIndex = (int)Mathf.Repeat(enemyIndex, (float)defenderT.UnitList.Count);
            }
            for (int i = 0; i < defenderT.UnitList.Count; i++)
            {
                defenderT.UnitList[i].AssignFocusUnit(attacker.UnitList[playerIndex]);

                defenderT.UnitList[i].StopTree(false);
                playerIndex++;
                playerIndex = (int)Mathf.Repeat(playerIndex, (float)attacker.UnitList.Count);
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