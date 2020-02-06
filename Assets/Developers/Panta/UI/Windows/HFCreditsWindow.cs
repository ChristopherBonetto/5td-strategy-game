using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFCreditsWindow : HFUIControl
{
    public override UIControlID Name => UIControlID.Credits;

    public void OnClickBackToMainMenu()
    {
        // Turn on main menu window,
        // turn off this window.
        HFUIManager.Instance.ShowAndHide(UIControlID.MainMenu, this);
    }
}
