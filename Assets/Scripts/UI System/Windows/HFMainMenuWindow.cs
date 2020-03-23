using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HFMainMenuWindow : HFUIControl
{
    public override UIControlID Name => UIControlID.MainMenu;


    protected override void Start()
    {
        HFUIManager.Instance.AddControl(this);
    }

    public void OnClickStart()
    {
        HFUIManager.Instance.ShowAndHide(UIControlID.LevelSelection, this);

        HFScenesManager.Instance.LoadSceneFromIndex(1);

        // Notify the game manager that the game is started.
        //HFGameManager.Instance.CurrentGameState = GameStates.StartGame;
    }

    public void OnCLickCredits()
    {
        // Show credits window.
        // Turn off this window.
        HFUIManager.Instance.ShowAndHide(UIControlID.Credits, this);
    }
}
