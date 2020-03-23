using HF.WaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(menuName = "Human Factor/New Level Info", fileName = "L_00_Info")]
public class HFLevelInfoSO : ScriptableObject
{
    [SerializeField] private string m_levelName;
    public string LevelName { get => m_levelName;}

    [SerializeField] private HFWavesCollection m_levelWavesInfo;
    public HFWavesCollection LevelWavesInfo { get => m_levelWavesInfo; }
    

    [TextArea] public string m_levelDescription;

    [SerializeField] private Object m_levelScene;
    public Object LevelScene { get => m_levelScene; }

    //MUST TO REVIEW THIS VARIABLE. if setted as property the scene manager in build don't load right the scenes.
    [HideInInspector]
    public int LevelSceneIndex;

    public bool m_levelCompleted = false;

    public void CompleteLevel()
    {
        m_levelCompleted = true;
    }

    public void ResetLevel()
    {
        m_levelCompleted = false;
    }
}
