using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HFLevelSelctecionWindow : HFUIControl
{
    public override UIControlID Name => UIControlID.LevelSelection;

    //public string SceneName;


    public void OnClickSelectLevel(int inSceneIndex)
    {
        // Invoke some event instead of running this method.
        HFLevelContainerManager.Instance.StartLevelFromIndex(inSceneIndex);
    }

    public void OnClickBackToMainMenu()
    {
        // Turn off this window,
        // Turn on main menu window.
        HFUIManager.Instance.ShowAndHide(UIControlID.MainMenu, this);
    }
}
