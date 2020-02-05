using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameStates
{
    LoadStartingInfo,
    StartGame,
    LevelSelection,
    InitializeLevel,
    PlayingLevel,
    EndLevel,
    Pause
}

public class HFGameManager : Singleton<HFGameManager>
{
    public GameStates m_currentGameState = GameStates.LoadStartingInfo;
    public GameStates CurrentGameState
    {
        get
        {
            return m_currentGameState;
        }
        set
        {
            if (CheckNextState(value))
            {
                ActionBeforeChangeGMState(value);
                m_currentGameState = value;
                ActionAfterChangeGMState(value);
            }
        }
    }

    public List<HFGMStateSO> ListOfStates = new List<HFGMStateSO>();

    public GameStates NextGameState;


    
    private void Start()
    {
        CurrentGameState = GameStates.LoadStartingInfo;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            CurrentGameState = NextGameState;
        }
    }

    public void ActionBeforeChangeGMState(GameStates newState)
    {
        switch (newState)
        {
            case GameStates.LoadStartingInfo:
                Debug.Log("Load info");
                LoadPlayerProfile();
                break;

            case GameStates.StartGame:
                Debug.Log("THE GAME STARTED");
                StartGameScene();
                break;

            case GameStates.LevelSelection:
                Debug.Log("ON LEVEL SELECTION SCENE");
                break;

            case GameStates.InitializeLevel:
                Debug.Log("INITIALIZE LEVEL WITH INFO");
                break;

            case GameStates.PlayingLevel:
                Debug.Log("LEVEL IS RUNNING");
                break;


            case GameStates.EndLevel:
                Debug.Log("LEVEL FINISHED");
                break;

            case GameStates.Pause:
                Debug.Log("PAUSE");
                break;

            default:
                break;
        }
    }

    public void ActionAfterChangeGMState(GameStates newState)
    {
        HFEventManager.TriggerEvent<GameStates>(HFEventID.OnGameStateChanged, newState);
    }


    public bool CheckNextState(GameStates newState)
    {
        for(int i = 0; i < ListOfStates.Count; i++)
        {
            if(CurrentGameState == ListOfStates[i].GameState)
            {
                if (ListOfStates[i].PossibleSwitchStates.Contains(newState))
                {
                    return true;
                }
                else
                {
                    Debug.Log(CurrentGameState + " can't go on : " + newState);
                }
            }
        }
        return false;
    }


    public void ChangeGMState(GameStates newState)
    {
        CurrentGameState = newState;
    }

    public void LoadPlayerProfile()
    {
        Debug.Log("Load player info method");
    }

    public void StartGameScene()
    {

    }
}