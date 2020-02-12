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

    [SerializeField] private HFWave m_levelWavesInfo;
    public HFWave LevelWavesInfo { get => m_levelWavesInfo; }
    

    [TextArea] public string m_levelDescription;

    [SerializeField] private Object m_levelScene;
    public Object LevelScene { get => m_levelScene;}
}
