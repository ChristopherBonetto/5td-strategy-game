using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HFInGameWindow : HFUIControl
{
    public override UIControlID Name => UIControlID.InGameWindow;

    [Header("Generic buttons")]

    /// <summary>
    /// "Call next wave" button.
    /// </summary>
    public Button ButtonCallNextWave;

    private void OnEnable()
    {
        HFEventManager.SubscribeTo(HFEventID.OnNewWaveBegin, OnNewWaveBegin);
        HFEventManager.SubscribeTo(HFEventID.OnWaveEnd, OnWaveEnd);

        ButtonCallNextWave.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom(HFEventID.OnNewWaveBegin, OnNewWaveBegin);
        HFEventManager.UnsubscribeFrom(HFEventID.OnWaveEnd, OnWaveEnd);
        
    }

    protected override void Start()
    {
        base.Start();
    }

    #region Events

    //--------------------------------------------------------
    // Event trigerred or listened from wave controller
    //--------------------------------------------------------

    private void OnNewWaveBegin()
    {
        ButtonCallNextWave.gameObject.SetActive(false);
    }

    private void OnWaveEnd()
    {
        ButtonCallNextWave.gameObject.SetActive(true);
    }

    #endregion

    public void PressNewWave()
    {
        HFEventManager.TriggerEvent(HFEventID.OnNewWaveBegin);
    }

    public void ReturnToLevelSelection()
    {
        HFGameManager.Instance.ChangeGMState(GameStates.Pause);
        HFUIManager.Instance.ShowAndHide(UIControlID.LevelSelection, this);
        HFScenesManager.Instance.LoadSceneFromIndex(1);
    }

    public void WinLevel()
    {
        HFScenesManager.Instance.EndCurrentLevel(true);
        HFUIManager.Instance.ShowAndHide(UIControlID.LevelSelection, this);
        HFScenesManager.Instance.LoadSceneFromIndex(1);
    }
}
