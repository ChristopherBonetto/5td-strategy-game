using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class HFFadingImage : MonoBehaviour
{
    [SerializeField]
    private float m_fadeDuration = .2f;
    private Image m_imageComponent;


    private void Awake()
    {
        m_imageComponent = GetComponent<Image>();
    }

    private void OnEnable() 
    {
        m_imageComponent.DOFade(1, m_fadeDuration);
    }

    public void TurnOff()
    {
        m_imageComponent.DOFade(0, m_fadeDuration).OnComplete(() => gameObject.SetActive(false));
    }
}
