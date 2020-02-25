using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HFButtonLevelSelection : MonoBehaviour
{
    public bool ClearTextOnPointerExit;

    /// <summary>
    /// S.O. of the level.
    /// </summary>
    public HFLevelInfoSO Level { get; set; }

    /// <summary>
    /// Show the level associated to the button
    /// in form of string.
    /// </summary>
    public Text ButtonText;

    /// <summary>
    /// On click() event: Load the level associated.
    /// </summary>
    public void OnClickLoadLevel()
    {
        // loading selected scene async.
        // Turn off the first window declared.
        // Turn on the second window declared (after loading).
        HFUIManager.Instance.LoadingScreenWindow.OnShow(SceneManager.LoadSceneAsync(Level.LevelSceneIndex), UIControlID.LevelSelection, UIControlID.InGameWindow);
        
        // Store the last level played.
        HFLevelManager.Instance.LastLevelPlayed = Level;

        HFScenesManager.Instance.LoadLevelFromLevelInfo(Level);
    }

    public void OnPointEnter()
    {
        HFLevelSelctecionWindow window = HFUIManager.Instance.UIControls[UIControlID.LevelSelection] as HFLevelSelctecionWindow;

        window.LevelDescription.LevelName.text = Level.LevelName;
        window.LevelDescription.LevelDescrition.text = Level.m_levelDescription;
    }

    public void OnPointExit()
    {
        if (ClearTextOnPointerExit)
        {
            HFLevelSelctecionWindow window = HFUIManager.Instance.UIControls[UIControlID.LevelSelection] as HFLevelSelctecionWindow;

            window.LevelDescription.LevelName.text = "";
            window.LevelDescription.LevelDescrition.text = "";
        }
    }
}
