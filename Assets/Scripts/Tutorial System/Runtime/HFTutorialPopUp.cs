using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component to track the tutorial ID.
/// </summary>
public class HFTutorialPopUp : MonoBehaviour
{
    public Text MessageText;

    /// <summary>
    /// Set the tutorial message to show in UI.
    /// </summary>
    /// <param name="message"></param>
    public void SetMessage(string message)
    {
        MessageText.text = message;
    }
}
