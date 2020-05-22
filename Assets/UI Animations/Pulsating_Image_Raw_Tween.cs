using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Pulsating_Image_Raw_Tween : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private Image my_button;


    public float highlightImageSize = 1.2f;
    public float standardImageSize = 1f;


    private void Start()
    {
        my_button = GetComponent<Image>();

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        my_button.transform.DOScale(highlightImageSize, 0.3f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        my_button.transform.DOScale(standardImageSize, 0.3f);
    }




}
