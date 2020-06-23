using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class HFScenesManager : Singleton<HFScenesManager>
{
    new public static HFScenesManager Instance
    {
        get
        {
            if (applicationIsQuitting)
                return null;

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (HFScenesManager)FindObjectOfType(typeof(HFScenesManager));


                    if (_instance == null)
                    {
                        GameObject outGO = Instantiate(Resources.Load<GameObject>("Managers/ScenesManager"));
                        _instance = outGO.GetComponent<HFScenesManager>();

                        DontDestroyOnLoad(_instance);
                    }
                    else
                        DontDestroyOnLoad(_instance);
                }
                
                return _instance;
            }
        }
    }

    #region Scene variables

    public int IndexCurrentScene = 0;

    private int m_sceneCount;
    public int SceneCount { get => m_sceneCount; }

    //MUST TO REVIEW THIS VARIABLE. if setted as property the scene manager in build don't load right the scenes.
    [HideInInspector]
    public List<string> AllScenes;

    #endregion

    #region Levels variables

    [SerializeField] private HFLevelContainerSO m_levelContainer;
    public HFLevelContainerSO LevelContainer { get => m_levelContainer; }

    private HFLevelInfoSO m_currentLevelSelected = null;
    public HFLevelInfoSO CurrentLevelSelected
    {
        get
        {
            return m_currentLevelSelected;
        }
        set
        {
            m_currentLevelSelected = value;

            if(m_currentLevelSelected != null)
            GameController.Instance.SetResources(m_currentLevelSelected.ResoucesQuantity);
        }
    }

    public int LevelCompletedCount { get => CountHowMuchLevelAreCompleted(); }
    

    #endregion

    #region MONOBEHAVIOUR CYCLE

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

    }
    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        IndexCurrentScene = currentScene.buildIndex;

        CheckCurrentScene(IndexCurrentScene);
    }


    #endregion

    #region EVENTS

    private void OnEnable()
    {
        HFEventManager.SubscribeTo<GameStates>(HFEventID.OnGameStateChanged, GiveAllReferenceToTheSelectedLevel);

        HFEventManager.SubscribeTo<EntityBehavior>(HFEventID.OnEntityDeath, CheckWhichUnitDeath);
    }
    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom<GameStates>(HFEventID.OnGameStateChanged, GiveAllReferenceToTheSelectedLevel);

        HFEventManager.UnsubscribeFrom<EntityBehavior>(HFEventID.OnEntityDeath, CheckWhichUnitDeath);
    }

    #endregion

    #region LEVEL METHODS

    //Call to all the subscribers the info's level.
    public void GiveAllReferenceToTheSelectedLevel(GameStates inState)
    {
        if (inState == GameStates.InitializeLevel)
        {
            Debug.Log("PASSATE LE INFO DEL LIVELLO CORRENTE");

            if(CurrentLevelSelected != null)
            {
                HFEventManager.TriggerEvent<HFLevelInfoSO>(HFEventID.OnInitializeLevel, CurrentLevelSelected);
            }
            else
            {
                CurrentLevelSelected = LevelContainer.Levels[0];
                HFEventManager.TriggerEvent<HFLevelInfoSO>(HFEventID.OnInitializeLevel, CurrentLevelSelected);
            }
        }
        
    }

    // #TEMP Move this to missions/win condition check (remember to subscribe event) or change entirely
    private void CheckWhichUnitDeath(EntityBehavior inKilledEntity)
    {
        if((inKilledEntity.EntityPlayerType == Types.PlayerType.Player))
        {
            if(inKilledEntity is BuildingBehaviour)
            {
                BuildingBehaviour building = inKilledEntity as BuildingBehaviour;

                if(building.GetStats().BuildingType == Types.BuildingType.CASTLE)
                {
                    EndCurrentLevel(false);
                }
            }
        }
    }

    public void EndCurrentLevel(bool winCondition)
    {
        HFGameManager.Instance.ChangeGMState(GameStates.EndLevel);

        Debug.Log(CurrentLevelSelected.LevelName + " winned : " + winCondition);

        if (winCondition)
        {
            CurrentLevelSelected.CompleteLevel();
        }

        HFEventManager.TriggerEvent<HFLevelInfoSO, bool>(HFEventID.OnEndLevel, CurrentLevelSelected, winCondition);

        HFGameManager.Instance.PlayerData.RefreshPlayerData();

        CurrentLevelSelected = null;
    }

    public void ActivateDeactivateLevels(int inValue)
    {
        for (int i = 0; i <= LevelContainer.Levels.Count - 1; i++)
        {
            if(i < inValue)
            {
                LevelContainer.Levels[i].CompleteLevel();
            }
            else
            {
                LevelContainer.Levels[i].ResetLevel();
            }
        }
    }

    public int CountHowMuchLevelAreCompleted()
    {
        int counter = 0;

        for(counter = 0; counter <= LevelContainer.Levels.Count -1;)
        {
            if (LevelContainer.Levels[counter].m_levelCompleted)
            {
                counter++;
            }
            else
            {
                break;
            }
        }
        return counter;
    }

    #endregion

    #region SCENE METHODS

    public void CheckCurrentScene(int inValue)
    {
        (int, int) levelInterval = TakeLevelInterval();

        if(inValue == 0)
        {
            HFGameManager.Instance.CurrentGameState = GameStates.LoadStartingInfo;
            Debug.Log("Sei partito nella scene load");
            return;
        }
        else if(inValue == 1)
        {
            HFGameManager.Instance.CurrentGameState = GameStates.WarRoom;
            Debug.Log("Sei partito nella level selection");
            return;
        }
        else if(inValue >= levelInterval.Item1 && inValue <= levelInterval.Item2)
        {
            CurrentLevelSelected = LevelContainer.Levels[inValue];
            HFGameManager.Instance.CurrentGameState = GameStates.InitializeLevel;
            Debug.Log("Stai giocando un livello");
            return;
        }
        else
        {
            Debug.Log("SCENA DI TEST");
        }
        
    }

    

    public (int, int) TakeLevelInterval()
    {
        List<HFLevelInfoSO> tempList = LevelContainer.Levels;

        int LevelsCount = tempList.Count;

        int firstLevel = tempList[0].LevelSceneIndex;
        int lastLevel = tempList[LevelsCount -1].LevelSceneIndex;

        return (firstLevel, lastLevel);
    }



    #region Get Scenes Reference

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
        LevelContainer.SorLevelByIndex();
    }

    public void GiveIndexToAllLevels()
    {
        for (int i = 0; i < LevelContainer.Levels.Count; i++)
        {
            if (GiveIndexToALevel(LevelContainer.Levels[i]))
            {
                Debug.Log("REFRESH : " + LevelContainer.Levels[i].LevelName + " REFRESHED INDEX/ADDED TO SCENES LIST");
            }
            else
            {
                //not refreshed because not finded;
            }
        }
        
    }

    public bool GiveIndexToALevel(HFLevelInfoSO inLevel)
    {
        if (AllScenes.Contains(inLevel.LevelScene.name))
        {
            int indexToAssign = AllScenes.FindIndex(x => x.Equals(inLevel.LevelScene.name));
            inLevel.LevelSceneIndex = indexToAssign;
            return true;
        }
        return false;
    }

    #endregion

    #region LOAD SCENE

    //Generic Load
    public void LoadSceneFromIndex(int inSceneIndex)
    {
        SceneManager.LoadSceneAsync(inSceneIndex);
    }

    //Load next scene
    public void LoadNextScene()
    {
        // Check if current scene index is smaller than the max scene count minus one
        if (IndexCurrentScene < SceneManager.sceneCountInBuildSettings - 1)
        {
            // Load next scene
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            // Load war room
            SceneManager.LoadSceneAsync(1);
        }
    }

    //Load scene from name
    public void LoadFromName(string inName)
    {
        if (AllScenes.Contains(inName))
        {
            SceneManager.LoadScene(inName);
        }
    }

    #endregion

    #region LOAD LEVEL SCENE

    //Giving a level info in input the corresponsive scene will be loaded
    public void LoadLevelFromLevelInfo(HFLevelInfoSO inLevel)
    {
        CurrentLevelSelected = inLevel;
        SceneManager.LoadScene(inLevel.LevelSceneIndex);

    }


    public void LoadLevelWithIndex(int inIndex)
    {
        CurrentLevelSelected = LevelContainer.Levels[inIndex];
        SceneManager.LoadScene(LevelContainer.Levels[inIndex].LevelSceneIndex);
    }
    #endregion

    #region WHEN A SCENE IS LOADED

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
        GameController.Instance.ClearDictionary();
    }

    #endregion

    #endregion
}
