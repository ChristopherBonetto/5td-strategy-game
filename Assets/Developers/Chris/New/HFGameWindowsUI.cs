using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFGameWindowsUI : MonoBehaviour
{

    public void OnClickStartGame()
    {
        HFGameManager.Instance.CurrentGameState = GameStates.PlayingLevel;
    }

    public void EndLevel()
    {
        HFGameManager.Instance.CurrentGameState = GameStates.EndLevel;
        HFScenesManager.Instance.LoadSceneFromIndex(1);
    }
}
