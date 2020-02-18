using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HFLevelSelctecionWindow : HFUIControl
{
    public override UIControlID Name => UIControlID.LevelSelection;

    //public string SceneName;


    public void OnClickSelectLevel(int inIndex)
    {
        // Invoke some event instead of running this method.
        
        HFScenesManager.Instance.LoadLevelWithIndex(inIndex);
    }

    public void OnClickLoadLevelFromInfo(HFLevelInfoSO inLevel)
    {
        HFScenesManager.Instance.LoadLevelFromLevelInfo(inLevel);
    }

    public void OnClickBackToMainMenu()
    {
        // Turn off this window,
        // Turn on main menu window.
        HFUIManager.Instance.ShowAndHide(UIControlID.MainMenu, this);
    }
}
