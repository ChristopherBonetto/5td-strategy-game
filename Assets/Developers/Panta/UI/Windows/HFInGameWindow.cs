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


    protected override void Start()
    {
        HFUIManager.Instance.AddControl(this);
    }

    /// <summary>
    /// Trigger by the event when a wave end.
    /// </summary>
    public void OnWaveEnd()
    {
        ButtonCallNextWave.gameObject.SetActive(true);
    }
}
