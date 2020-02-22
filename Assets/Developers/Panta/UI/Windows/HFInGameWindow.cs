using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HFInGameWindow : HFUIControl
{
    public override UIControlID Name => UIControlID.InGameWindow;

    /// <summary>
    /// "Call next wave" button.
    /// </summary>
    public Button ButtonCallNextWave;


    private void OnEnable()
    {
        HFEventManager.SubscribeTo<bool>(HFEventID.OnWaveEnd, OnWaveEnd);
    }

    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom<bool>(HFEventID.OnWaveEnd, OnWaveEnd);
    }


    /// <summary>
    /// Trigger by the event when a wave end.
    /// </summary>
    public void OnWaveEnd(bool isLevelCompleted)
    {
        ButtonCallNextWave.gameObject.SetActive(!isLevelCompleted);
    }

    public void OnClickCallNextWave()
    {
        ButtonCallNextWave.gameObject.SetActive(false);
        HFEventManager.TriggerEvent(HFEventID.OnCallNextWave);
    }
}
