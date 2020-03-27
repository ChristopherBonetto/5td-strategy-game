using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceChanger : MonoBehaviour
{
    [SerializeField] private Ambience m_enterAmbience;
    [SerializeField] private Ambience m_exitAmbience;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Character tempValue = collision.GetComponent<Character>();

        if (tempValue != null)
            if(m_enterAmbience == Ambience.Underwater)
            {
                tempValue.SplashSound();
            }
            AmbienceSwitcher.Instance.ChangeAmbience(m_enterAmbience);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Character tempValue = collision.GetComponent<Character>();

        if (tempValue != null)
            AmbienceSwitcher.Instance.ChangeAmbience(m_exitAmbience);
    }
}
