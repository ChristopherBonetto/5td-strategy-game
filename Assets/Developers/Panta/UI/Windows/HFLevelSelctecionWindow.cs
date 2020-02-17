using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HFLevelSelctecionWindow : HFUIControl
{
    public override UIControlID Name => UIControlID.LevelSelection;

    //public string SceneName;


    public void OnClickSelectLevel(HFLevelInfoSO inLevel)
    {
        // Invoke some event instead of running this method.
        HFScenesManager.Instance.LoadLevelFromLevelInfo(inLevel);
    }

    public void OnClickBackToMainMenu()
    {
        // Turn off this window,
        // Turn on main menu window.
        HFUIManager.Instance.ShowAndHide(UIControlID.MainMenu, this);
    }
}
