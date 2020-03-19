using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugLevelInBuild : MonoBehaviour
{
    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    private void Update()
    {
        text.text = $"{HFScenesManager.Instance.LevelContainer.Levels.Count} | {HFScenesManager.Instance.IndexCurrentScene} | " +
            $"{HFScenesManager.Instance.CurrentLevelSelected}";
    }
}
