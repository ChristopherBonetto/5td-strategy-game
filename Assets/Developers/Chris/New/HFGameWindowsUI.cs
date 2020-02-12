using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFGameWindowsUI : MonoBehaviour
{
    private void OnEnable()
    {
        HFEventManager.SubscribeTo<GameStates>(HFEventID.OnGameStateChanged, UnderstandBehaviorFromGameState);
    }

    public void UnderstandBehaviorFromGameState(GameStates inState)
    {
        if (inState == GameStates.InitializeLevel || inState == GameStates.PlayingLevel)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
