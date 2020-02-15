using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFLevelContainerManager : Singleton<HFLevelContainerManager>
{

    [SerializeField] private HFLevelContainerSO m_levelContainer;
    public HFLevelContainerSO LevelContainer { get => m_levelContainer; }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    public void StartLevelFromIndex(int inIndex)
    {
        HFSceneManager.Instance.LoadFromName(LevelContainer.Levels[inIndex].LevelSceneName);
    }
}
