using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTBUTTONS : MonoBehaviour
{
    public GameStates StateToCallMe;

    private void OnEnable()
    {
        HFEventManager.SubscribeTo<GameStates>(HFEventID.OnGameStateChanged, CheckCurrentState); 
    }
    //private void OnDisable()
    //{
    //    HFEventManager.UnsubscribeFrom<GameStates>(HFEventID.OnGameStateChanged, CheckCurrentState);
    //}

    

    public void CheckCurrentState(GameStates inState)
    {
        if(inState == StateToCallMe)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

}
