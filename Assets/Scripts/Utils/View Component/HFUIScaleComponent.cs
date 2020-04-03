using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HFUIScaleComponent : MonoBehaviour
{
    enum UIScaleMode
    {
        In,
        Out,
    }

    [SerializeField]
    bool m_animateOnEnable;
    [SerializeField]
    bool m_startFromScaleZero;
    [SerializeField]
    float m_duration;
    [SerializeField]
    Vector3 m_targetScale;
    [SerializeField]
    Transform m_imageComponent;

    Vector3 m_initialScale;
    float m_timeElapsed;

    UIScaleMode m_mode = UIScaleMode.In;

    private void OnEnable()
    {
        if (m_animateOnEnable)
        {
            m_timeElapsed = 0;
        }
        else
        {
            m_timeElapsed = m_duration;
        }

        m_imageComponent.transform.localScale = GetInitialScale();
    }

    private Vector3 GetInitialScale()
    {
        if (m_initialScale == null)
        {
            m_initialScale = m_startFromScaleZero ? Vector3.zero : m_imageComponent.transform.localScale;
        }
        return m_initialScale;
    }

    private void Update()
    {
        if (m_timeElapsed < m_duration)
        {
            if (m_mode == UIScaleMode.In)
            {
                ScaleIn();
            }
            else if (m_mode == UIScaleMode.Out)
            {
                ScaleOut();
            }
        }
    }

    public void OnPointerEnter()
    {
        m_timeElapsed = 0;
        m_mode = UIScaleMode.In;
    }

    public void OnPoiterExit()
    {
        m_timeElapsed = 0;
        m_mode = UIScaleMode.Out;
    }

    public void ScaleIn()
    {
        m_timeElapsed += Time.deltaTime;
        m_imageComponent.transform.DOScale(m_targetScale, m_duration);
    }

    public void ScaleOut()
    {
        m_timeElapsed += Time.deltaTime;
        m_imageComponent.transform.DOScale(GetInitialScale(), m_duration);
    }
}
