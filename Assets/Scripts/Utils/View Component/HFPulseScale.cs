using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class HFPulseScale : MonoBehaviour
{
    [SerializeField]
    bool m_startOnEnable;
    [SerializeField]
    Transform m_targetTransform;
    [SerializeField]
    float m_amplitude;
    [SerializeField]
    float m_duration;

    float m_timeElapsed;

    private void OnEnable()
    {
        m_timeElapsed = m_startOnEnable ? 0 : m_duration;
    }

    public IEnumerator Pulse()
    {
        m_timeElapsed = 0;

        while(m_timeElapsed < m_duration)
        {
            m_timeElapsed += Time.deltaTime;
            m_targetTransform.DOScale(1 + m_amplitude * Mathf.Sin(m_timeElapsed / m_duration * Mathf.PI), m_duration);
            yield return null;
        }
        yield return null;
    }
}
