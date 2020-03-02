using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HFInGameWindow : HFUIControl
{
    public override UIControlID Name => UIControlID.InGameWindow;

    /// <summary>
    /// All info displayed about wave.
    /// </summary>
    public HFWaveInfoUIElement WaveInfoUIElement;

    /// <summary>
    /// All info displayed about enemies.
    /// </summary>
    public HFEnemyInfoUIElement EnemyInfoUIElement;

    /// <summary>
    /// Used when level end.
    /// </summary>
    public Button ButtonReturnToLevelSelection;

    /// <summary>
    /// "Call next wave" button.
    /// </summary>
    public Button ButtonCallNextWave;


    private void OnEnable()
    {
        HFEventManager.SubscribeTo<HFLevelInfoSO, bool>(HFEventID.OnEndLevel, OnEndLevel);
    }

    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom<HFLevelInfoSO, bool>(HFEventID.OnEndLevel, OnEndLevel);
    }

    /// <summary>
    /// Trigger by the event when a wave end.
    /// </summary>
    public void ActiveNextWaveButton()
    {
        ButtonCallNextWave.gameObject.SetActive(true);
    }

    /// <summary>
    /// Invoked through event
    /// </summary>
    /// <param name="isWin"></param>
    public void OnEndLevel(HFLevelInfoSO level, bool isWin)
    {
        ButtonReturnToLevelSelection.gameObject.SetActive(true);
    }

    public void OnClickCallNextWave()
    {
        ButtonCallNextWave.gameObject.SetActive(false);
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
