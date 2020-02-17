using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HFScenesManager : Singleton<HFScenesManager>
{
    [SerializeField] private HFLevelContainerSO m_levelContainer;
    public HFLevelContainerSO LevelContainer { get => m_levelContainer; }

    public int IndexCurrentScene = 0;

    private int m_sceneCount;
    public int SceneCount { get => m_sceneCount; }

    public List<string> AllScenes;

    public Text ciao;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        IndexCurrentScene = currentScene.buildIndex;

        GiveIndexToAllLevels();
    }
    
   

    #region Get Scenes

    public void TakeAllSceneInBuild()
    {
        AllScenes.Clear();

        m_sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;

        for (int i = 0; i < m_sceneCount; i++)
        {
            string tempSceneName = System.IO.Path.GetFileNameWithoutExtension(UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i));
            if (!AllScenes.Contains(tempSceneName))
            {
                AllScenes.Add(tempSceneName);
            }
        }
    }

    public void GiveIndexToAllLevels()
    {
        for (int i = 0; i < LevelContainer.Levels.Count; i++)
        {
            GiveIndexToALevel(LevelContainer.Levels[i]);
        }
    }

    public void GiveIndexToALevel(HFLevelInfoSO inLevel)
    {
        if (AllScenes.Contains(inLevel.LevelScene.name))
        {
            int indexToAssign = AllScenes.FindIndex(x => x.Equals(inLevel.LevelScene.name));
            inLevel.LevelSceneIndex = indexToAssign;
        }
    }

    #endregion

    #region LOAD SCENE

    public void LoadSceneFromIndex(int inSceneIndex)
    {
        SceneManager.LoadScene(inSceneIndex, LoadSceneMode.Single);
    }

    public void LoadNextScene()
    {
        // Check if current scene index is smaller than the max scene count minus one
        if (IndexCurrentScene < SceneManager.sceneCountInBuildSettings - 1)
        {
            // Load next scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            // Load war room
            SceneManager.LoadScene(1);
        }
    }

    public void LoadFromName(string inName)
    {
        if (AllScenes.Contains(inName))
        {
            SceneManager.LoadScene(inName);
        }
    }

    public void LoadLevelFromLevelInfo(HFLevelInfoSO inLevel)
    {
        SceneManager.LoadScene(inLevel.LevelSceneIndex);
    }

    public void LoadLevelWithIndex(int inIndex)
    {
        Debug.Log("To implement");
        //SceneManager.LoadScene();
    }

    #endregion

    public void OnLevelWasLoaded(int level)
    {
        IndexCurrentScene = level;

        if (IndexCurrentScene == 0)
        {
            HFGameManager.Instance.CurrentGameState = GameStates.LoadStartingInfo;
        }
        else if (IndexCurrentScene == 1)
        {
            HFGameManager.Instance.CurrentGameState = GameStates.WarRoom;
        }
        else
        {
            HFGameManager.Instance.CurrentGameState = GameStates.InitializeLevel;
        }

    }
}
