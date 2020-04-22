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

        private void Start()
        {
            HFScenesManager sceneM = HFScenesManager.Instance;
            HFLevelContainerSO levelContainer = sceneM.LevelContainer;

            for (int i = 0; i < levelContainer.Levels.Count; i++)
            {
                m_loadLevelButtons[i].ButtonText.text = PrefixButtonText + " " + (i + 1).ToString();
                m_loadLevelButtons[i].Level = levelContainer.Levels[i];
            }
        }
    }
}
