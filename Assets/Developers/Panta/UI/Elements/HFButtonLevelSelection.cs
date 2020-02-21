using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HFButtonLevelSelection : MonoBehaviour
{
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
        HFUIManager.Instance.LoadingScreenWindow.OnShow(SceneManager.LoadSceneAsync(Level.LevelSceneIndex), UIControlID.LevelSelection, UIControlID.InGameWindow);
    }
}
