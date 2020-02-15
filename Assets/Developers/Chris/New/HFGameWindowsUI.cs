using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFGameWindowsUI : HFUIControl
{
    public override UIControlID Name => UIControlID.InGameWindow;

    public void OnClickStartGame()
    {
        HFGameManager.Instance.CurrentGameState = GameStates.PlayingLevel;
    }

    public void EndLevel()
    {
        HFGameManager.Instance.CurrentGameState = GameStates.EndLevel;
        HFSceneManager.Instance.LoadSceneFromIndex(1);
    }
}
