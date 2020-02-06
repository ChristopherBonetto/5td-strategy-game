using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HFLevelSelctecionWindow : HFUIControl
{
    public override UIControlID Name => UIControlID.LevelSelection;

    public string SceneName;


    public void OnClickSelectLevel()
    {
        // Invoke some event instead of running this method.
        HFGameManager.Instance.CurrentGameState = GameStates.InitializeLevel;
        SceneManager.LoadScene(SceneName);
    }

    public void OnClickBackToMainMenu()
    {
        // Turn off this window,
        // Turn on main menu window.
        HFGameManager.Instance.CurrentGameState = GameStates.StartGame;
        HFUIManager.Instance.ShowAndHide(UIControlID.MainMenu, this);
    }
}
