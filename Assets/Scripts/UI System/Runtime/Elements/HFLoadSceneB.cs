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
                HFUIManager.Instance.GetLoadingScreen().LoadLevel(Sceneindex, false);
            }
        }
    }
}