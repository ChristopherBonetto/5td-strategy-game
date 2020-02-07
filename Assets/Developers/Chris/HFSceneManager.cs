using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HFSceneManager : Singleton<HFSceneManager>
{

    public int IndexCurrentScene = 0;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        IndexCurrentScene = currentScene.buildIndex;
    }

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

    public void OnLevelWasLoaded(int level)
    {
        IndexCurrentScene = level;

        if(IndexCurrentScene == 0)
        {
            HFGameManager.Instance.CurrentGameState = GameStates.LoadStartingInfo;
        }
        else if(IndexCurrentScene == 1)
        {
            HFGameManager.Instance.CurrentGameState = GameStates.StartGame;
        }
        else
        {
            HFGameManager.Instance.CurrentGameState = GameStates.InitializeLevel;
        }

    }
}