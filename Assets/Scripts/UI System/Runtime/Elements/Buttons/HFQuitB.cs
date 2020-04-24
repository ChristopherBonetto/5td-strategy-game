using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HF.Refactoring;

public class HFQuitB : HFButton
{
    public void QuitApplication()
    {
        if (m_isListeningInput)
        {
            Application.Quit();
        }
    }
}
