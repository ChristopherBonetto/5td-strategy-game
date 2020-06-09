using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeyShortcuts : MonoBehaviour
{
    public KeyCode assignedKey;

    public Button m_Button;

    void Update()
    {
        if( m_Button!=null)
        {
            if(Input.GetKeyDown(assignedKey))
            {
                m_Button.onClick.Invoke();
            }
        }
    }
}
