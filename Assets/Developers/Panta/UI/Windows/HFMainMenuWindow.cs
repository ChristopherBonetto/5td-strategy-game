using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFMainMenuWindow : HFUIControl
{
    public override UIControlID Name => UIControlID.MainMenu;


    protected override void Start()
    {
        HFUIManager.Instance.AddControl(this);
    }

    public void OnClickStart()
    {
        // Turn on level selection window,
        // turn off this window.
        HFScenesManager.Instance.LoadSceneFromIndex(1);

        // Notify the game manager that the game is started.
        HFGameManager.Instance.CurrentGameState = GameStates.StartGame;
    }

    public void OnCLickCredits()
    {
        // Show credits window.
        // Turn off this window.
        HFUIManager.Instance.ShowAndHide(UIControlID.Credits, this);
    }
}
