using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using BehaviorDesigner.Runtime;

public class CastleStarter : MonoBehaviour
{
    private void OnEnable()
    {
        HFEventManager.SubscribeTo<GameStates>(HFEventID.OnGameStateChanged, CreateCastleWhenStartLevel);
    }
    private void OnDisable()
    {
        HFEventManager.SubscribeTo<GameStates>(HFEventID.OnGameStateChanged, CreateCastleWhenStartLevel);
    }


    public void CreateCastleWhenStartLevel(GameStates inState)
    {
        if(inState == GameStates.InitializeLevel)
        {
            GameController.Instance.CreateNewBuilding(BuildingType.CASTLE, PlayerType.Player, transform.position);
        }
    }
}
