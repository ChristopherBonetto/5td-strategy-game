using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.Refactoring
{
    public class HFLoadSceneB : HFButton
    {
        public int Sceneindex = 0;

        public void LoadScene()
        {
            if (m_isListeningInput)
            {
                HFScenesManager.Instance.LoadSceneFromIndex(Sceneindex);
            }
        }

        public void LoadSceneWithLoading()
        {
            if (m_isListeningInput)
            {
                HFGameManager.Instance.ChangeGMState(GameStates.Pause); // Change the state when the level end.
                HFUIManager.Instance.ClearHistory();
                HFUIManager.Instance.Getwindow<HFUILoadingScreen>(HFUIWindowID.LOADING_SCREEN).LoadLevel(Sceneindex, false);
            }
        }
    }
}