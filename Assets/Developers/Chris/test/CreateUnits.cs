using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using UnityEngine.AI;

public class CreateUnits : MonoBehaviour
{
    public BehaviorTree player;
    public BehaviorTree enemy;

    public NavMeshAgent playerAgent;
    public Collider playerCollider;

    public GameObject Tower;
    public GameObject Castle;
    public GameObject EnemyTarget;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            player.EnableBehavior();
            playerAgent.SetDestination(new Vector3(10, 0, 10));
            playerCollider.enabled = false;
            enemy.EnableBehavior();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            var myIntVariable2 = (SharedGameObject)player.GetVariable("EnemyTarget");
            myIntVariable2.Value = null;
            var myIntVariable = (SharedGameObject)player.GetVariable("BuildingTarget");
            myIntVariable.Value = Tower;
            Debug.Log(myIntVariable);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            player.DisableBehavior();
            playerAgent.ResetPath();
            playerAgent.isStopped = false;
            playerAgent.destination = new Vector3(10, 0, 10);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            var myIntVariable = (SharedGameObject)player.GetVariable("BuildingTarget");
            myIntVariable.Value = null;

            var myIntVariable2 = (SharedGameObject)player.GetVariable("EnemyTarget");
            myIntVariable2.Value = null;

            playerAgent.ResetPath();
            player.EnableBehavior();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            var myIntVariable2 = (SharedGameObject)player.GetVariable("BuildingTarget");
            myIntVariable2.Value = null;
            var myIntVariable = (SharedGameObject)player.GetVariable("EnemyTarget");
            myIntVariable.Value = EnemyTarget;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Click();
        }
    }
    public void Fight()
    {
        player.DisableBehavior();
        enemy.DisableBehavior();
    }

    public void Click()
    {
        RaycastHit HitInfo;
        Ray Ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(Ray, out HitInfo, Mathf.Infinity))
        {
            Debug.Log(HitInfo.transform.name);
        }
    }
}
