using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateSetter : MonoBehaviour
{
    public GameStates State;

    public void SetState()
    {
        HFGameManager.Instance.ChangeGMState(State);
    }
}
