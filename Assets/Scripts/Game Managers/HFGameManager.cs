using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameStates
{
    None,
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

    new public static HFGameManager Instance
    {
        get
        {
            if (applicationIsQuitting)
                return null;

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (HFGameManager)FindObjectOfType(typeof(HFGameManager));

                    if (_instance == null)
                    {
                        GameObject outGO = Instantiate(Resources.Load<GameObject>("Managers/GameManager"));
                        _instance = outGO.GetComponent<HFGameManager>();

                        DontDestroyOnLoad(_instance);
                    }
                    else
                        DontDestroyOnLoad(_instance);
                }
                return _instance;
            }
        }
    }

    public GameStates m_currentGameState = GameStates.None;
    public float TileActivationRadius;
    public float TileQuantizationFactor;
    public float TileDistanceFactor;

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

    public HFPlayerData m_playerData = null;
    public HFPlayerData PlayerData
    {
        get
        {
            return m_playerData;
        }
        set
        {
            m_playerData = value;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
    }


    #region GAME STATE SYSTEM, and trigger event.

    //Deprecated
 //   public void ActionBeforeChangeGMState(GameStates preState, GameStates postState)
 //   {
 //       switch (preState)
 //       {
 //           case GameStates.LoadStartingInfo:
 //               break;

 //           case GameStates.StartGame:
 //               //used to take all the player info and if the player want load another file info
 //               break;

 //           case GameStates.WarRoom:
 //               //level selection
 //               break;

 //           case GameStates.InitializeLevel:
 //               //give info to all
 //               break;

 //           case GameStates.PlayingLevel:
 //               //level in play
 //               break;

 //           case GameStates.EndLevel:
 //               break;

 //           case GameStates.Pause:
 //               break;

 //           default:
 //               break;
 //       }
 //       //Debug.Log("Do something before change " + preState + " in " + postState);
	//	HFEventManager.TriggerEvent<GameStates, GameStates>(HFEventID.OnBeforeChangeState, preState, postState);
	//}

	public void ActionAfterChangeGMState(GameStates inState)
    {
        switch (inState)
        {
            case GameStates.LoadStartingInfo:
                StartCoroutine(WaitAllLoadCompleted());
                break;

            case GameStates.StartGame:
                break;

            case GameStates.WarRoom:
                break;

            case GameStates.InitializeLevel:
                break;

            case GameStates.PlayingLevel:
                HFEventManager.TriggerEvent(HFEventID.OnLevelReady);
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

    #endregion


    private IEnumerator WaitAllLoadCompleted()
    {
        Debug.Log("WAIT TO LOAD ALL GAME INFO");
        yield return new WaitUntil(() => HFFmodDatabase.Instance.EventDatabaseCompleted == true);

        while(PlayerData == null)
        {
            PlayerData = HFSavingManager.LoadGame("NoName");
            yield return new WaitForEndOfFrame();
        }

        CurrentGameState = GameStates.StartGame;
    }

}