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

    private void OnEnable()
    {
        HFEventManager.SubscribeTo<bool>(HFEventID.OnEndLevel, EndLevelCondition);
    }
    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom<bool>(HFEventID.OnEndLevel, EndLevelCondition);
    }

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
                //try to load player file
                //if doesn't file any file it will create a new one
                LoadPlayerProfile();
                break;

            case GameStates.StartGame:
                //used to take all the player info and if the player want load another file info
                break;

            case GameStates.WarRoom:
                //level selection
                break;

            case GameStates.InitializeLevel:
                //give info to all
                break;

            case GameStates.PlayingLevel:
                //level in play
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

    
    public void EndLevelCondition(bool inBool)
    {
        ChangeGMState(GameStates.EndLevel);
    }

    public void StartGame()
    {
        //will be replace when the player has been positioned all his units in game.
        CurrentGameState = GameStates.PlayingLevel;
    }
}