using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFNonGameWindowsUI : MonoBehaviour
{
    private void OnEnable()
    {
        HFEventManager.SubscribeTo<GameStates>(HFEventID.OnGameStateChanged, UnderstandBehaviorFromGameState);
    }

    public void UnderstandBehaviorFromGameState(GameStates inState)
    {
        if(inState == GameStates.StartGame || inState == GameStates.WarRoom)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
