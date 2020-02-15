using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFLoadWindowsUI : MonoBehaviour
{
    private void OnEnable()
    {
        HFEventManager.SubscribeTo<GameStates>(HFEventID.OnGameStateChanged, UnderstandBehaviorFromGameState);
    }

    public void UnderstandBehaviorFromGameState(GameStates inState)
    {
        if (inState == GameStates.LoadStartingInfo)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
