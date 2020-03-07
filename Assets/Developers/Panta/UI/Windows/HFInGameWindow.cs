using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HFInGameWindow : HFUIControl
{
    public override UIControlID Name => UIControlID.InGameWindow;

    [Header("Wave Info Field")]

    /// <summary>
    /// Number of wave compleated.
    /// </summary>
    public Text waveProgressionInfo;

    /// <summary>
    /// real time passed.
    /// </summary>
    public Text TimeElapsed;

    [Header("Enemies info")]

    // This variables will be inside a class
    // to allow us to manage the ally infos too.
    // when created this class it will be filered in a dictionary.

    public Image EnemyIconPrefab;
    public HorizontalLayoutGroup EnemyIconsGridParent;
    public List<HFBaseStats> EnemiesTroopsStats;
    public List<Image> EnemiesTroopIcons;

    [Header("Generic buttons")]

    /// <summary>
    /// Used when level end.
    /// </summary>
    public Button ButtonReturnToLevelSelection;

    /// <summary>
    /// "Call next wave" button.
    /// </summary>
    public Button ButtonCallNextWave;

    protected override void Start()
    {
        base.Start();
        EnemiesTroopIcons = new List<Image>();
        EnemiesTroopsStats = new List<HFBaseStats>();
    }

    public void OnClickCallNextWave()
    {
        HFEventManager.TriggerEvent(HFEventID.OnCallNextWave);
    }

    public void ReturnToLevelSelection()
    {
        HFUIManager.Instance.ShowAndHide(UIControlID.LevelSelection, this);
        HFScenesManager.Instance.LoadSceneFromIndex(1);
    }

    public void StartTemporaryGame()
    {
        HFGameManager.Instance.ChangeGMState(GameStates.PlayingLevel);
    }
}
