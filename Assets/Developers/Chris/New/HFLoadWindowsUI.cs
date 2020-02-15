using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFLoadWindowsUI : MonoBehaviour
{
    public void LoadedPlayer()
    {
        HFGameManager.Instance.CurrentGameState = GameStates.StartGame;
    }
}
