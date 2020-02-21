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


    /// <summary>
    /// Trigger by the event when a wave end.
    /// </summary>
    public void OnWaveEnd()
    {
        ButtonCallNextWave.gameObject.SetActive(true);
    }

    public void OnClickCallNextWave()
    {
        HFEventManager.TriggerEvent(HFEventID.OnCallNextWave);
    }
}
