using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HF.Refactoring
{
    public class HFUIWarRoom : HFUIWindow
    {
        public override HFUIWindowID ID => HFUIWindowID.WAR_ROOM;

        [SerializeField]
        private HFLoadLevelB[] m_loadLevelButtons;

        /// <summary>
        /// Constant word present in every button. 
        /// It will be followed by the index of the level.
        /// </summary>
        public string PrefixButtonText;

        public bool AllLevelsUnlocked = true;

        private void OnEnable()
        {
            HFScenesManager sceneM = HFScenesManager.Instance;
            HFLevelContainerSO levelContainer = sceneM.LevelContainer;

            for (int i = 0; i < m_loadLevelButtons.Length; i++)
            {
                if (i > 0)
                {
#if UNITY_EDITOR
                    if (AllLevelsUnlocked)
                    {
                        m_loadLevelButtons[i].button.enabled = true;
                        m_loadLevelButtons[i].Background.color = Color.white;
                    }
                    else
                    {
                        // enable the "i" button if the previous one is completed
                        m_loadLevelButtons[i].button.enabled = sceneM.LevelContainer.Levels[i - 1].m_levelCompleted;

                        if (!m_loadLevelButtons[i].button.enabled)
                            m_loadLevelButtons[i].Background.color = Color.grey;
                        else
                            m_loadLevelButtons[i].Background.color = Color.white;
                    }
#else

                    // enable the "i" button if the previous one is completed
                    m_loadLevelButtons[i].button.enabled = sceneM.LevelContainer.Levels[i - 1].m_levelCompleted;

                        if (!m_loadLevelButtons[i].button.enabled)
                            m_loadLevelButtons[i].Background.color = Color.grey;
                        else
                            m_loadLevelButtons[i].Background.color = Color.white;
#endif
                }

                m_loadLevelButtons[i].ButtonText.text = PrefixButtonText + " " + (i + 1).ToString();
                m_loadLevelButtons[i].Level = levelContainer.Levels[i];
            }
        }

        public void EnableAllButtons(bool enabled) 
        {
            for (int i = 0; i < m_loadLevelButtons.Length; i++) 
            {
                m_loadLevelButtons[i].button.enabled = enabled;
            }
        }
    }
}
