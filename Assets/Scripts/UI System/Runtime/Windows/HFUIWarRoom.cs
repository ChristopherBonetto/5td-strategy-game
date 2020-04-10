using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HF.Refactoring
{
    public class HFUIWarRoom : HFUIWindow
    {
        public override HFUIWindowID ID => HFUIWindowID.WAR_ROOM;


        /// <summary>
        /// it'sused as parent of buttons.
        /// </summary>
        public VerticalLayoutGroup ButtonsGrid;

        [Header("Buttons Field")]

        /// <summary>
        /// Selection level button prefab.
        /// </summary>
        public HFPoolID LoadLevelButtonID;

        /// <summary>
        /// Constant word present in every button. 
        /// It will be followed by the index of the level.
        /// </summary>
        public string PrefixButtonText;


        private void Awake()
        {
            SpawnLevelButtons();
        }

        private void SpawnLevelButtons()
        {
            List<HFLevelInfoSO> levels = HFScenesManager.Instance.LevelContainer.Levels;

            for (int i = 0; i < levels.Count; i++)
            {
                Debug.Log(HFPoolManager.Instance);
                HFLoadLevelB button = HFPoolManager.Instance.GetPooledObject(LoadLevelButtonID.ID).GetComponent<HFLoadLevelB>();
                button.Level = levels[i];
                button.ButtonText.text = $"{PrefixButtonText}: {i + 1}";
                button.gameObject.SetActive(true);
                button.transform.SetParent(ButtonsGrid.transform);
            }
        }
    }
}
