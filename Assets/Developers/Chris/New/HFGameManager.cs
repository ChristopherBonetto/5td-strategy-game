using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameStates
{
    LoadStartingInfo,
    StartGame,
    WarRoom,
    InitializeLevel,
    PlayingLevel,
    EndLevel,
    Pause
}

public class HFGameManager : Singleton<HFGameManager>
{
    private GameStates m_currentGameState = GameStates.LoadStartingInfo;
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
                //Used to reset somethings before change state
                //ActionBeforeChangeGMState(m_currentGameState, value);

                m_currentGameState = value;
                ActionAfterChangeGMState(value);

                Debug.Log(m_currentGameState);
            }
        }
    }

    public List<HFGMStateSO> ListOfStates = new List<HFGMStateSO>();

    

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        CurrentGameState = GameStates.LoadStartingInfo;
    }



    public void ActionBeforeChangeGMState(GameStates preState, GameStates postState)
    {
        switch (preState)
        {
            case GameStates.LoadStartingInfo:
                LoadPlayerProfile();
                break;

            case GameStates.StartGame:
                
                StartGameScene();
                break;

            case GameStates.WarRoom:
                break;

            case GameStates.InitializeLevel:
                break;

            case GameStates.PlayingLevel:
                break;

            case GameStates.EndLevel:
                break;

            case GameStates.Pause:
                break;

            default:
                break;
        }
        Debug.Log("Do something before change " + preState + " in " + postState);
    }

    public void ActionAfterChangeGMState(GameStates inState)
    {
        switch (inState)
        {
            case GameStates.LoadStartingInfo:
                break;

            case GameStates.StartGame:
                break;

            case GameStates.WarRoom:
                break;

            case GameStates.InitializeLevel:
                break;

            case GameStates.PlayingLevel:
                break;

            case GameStates.EndLevel:
                break;

            case GameStates.Pause:
                break;

            default:
                break;
        }
        HFEventManager.TriggerEvent<GameStates>(HFEventID.OnGameStateChanged, inState);
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