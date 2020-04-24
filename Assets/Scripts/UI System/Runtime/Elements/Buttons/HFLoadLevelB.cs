using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HF.Refactoring
{
    public class HFLoadLevelB : HFButton
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
        public void LoadLevel()
        {
            if (m_isMatchingWindowID)
            {
                HFScenesManager.Instance.CurrentLevelSelected = Level;

                // loading selected scene async.
                // Turn off the first window declared.
                // Turn on the second window declared (after loading).
                HFUIManager.Instance.Getwindow<HFUILoadingScreen>(HFUIWindowID.LOADING_SCREEN).LoadLevel(Level.LevelSceneIndex);
            }
        }
    }
}
