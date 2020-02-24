using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFLevelManager : Singleton<HFLevelManager>
{
    /// <summary>
    /// References about all levels of the game.
    /// </summary>
    public HFLevelContainerSO LevelContainer;   // better load from resources?

    /// <summary>
    /// Tell us what level the player plays last.
    /// </summary>
    public HFLevelInfoSO LastLevelPlayed { get; set; }


    // The levels progression is linear? 
    // if it is store the last level completed.
    [HideInInspector] public int LastLevelCompleted;
    // if not store an array of bools.
    [HideInInspector] public bool[] LevelsCompleted;
    // We need this to save datas.


    public HFLevelInfoSO CurrentLevel = null;

    private void OnEnable()
    {
        HFEventManager.SubscribeTo<HFLevelInfoSO>(HFEventID.OnInitializeLevel, TakeCurrentLevel);
    }
    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom<HFLevelInfoSO>(HFEventID.OnInitializeLevel, TakeCurrentLevel);
    }

    private void Start()
    {
        LevelsCompleted = new bool[LevelContainer.Levels.Count];
    }


    public void TakeCurrentLevel(HFLevelInfoSO inLevel)
    {
        CurrentLevel = inLevel;
    }
}
