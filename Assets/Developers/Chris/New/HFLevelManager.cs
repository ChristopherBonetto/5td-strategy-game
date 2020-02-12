using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFLevelManager : Singleton<HFLevelManager>
{

    [SerializeField] private HFLevelContainerSO m_levelContainer;
    public HFLevelContainerSO LevelContainer { get => m_levelContainer; }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    public void StartLevelFromIndex(int inIndex)
    {
        HFSceneManager.Instance.LoadFromObjectScene(LevelContainer.Levels[inIndex].LevelScene);
    }
}
