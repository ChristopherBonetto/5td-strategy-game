using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Pulsating_Image_Tween : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    private Button my_button;


    public float highlightButtonSize = 1.2f;
    public float standardButtonSize = 1f;


    private void Start()
    {
        my_button = GetComponent<Button>();

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        my_button.transform.DOScale(highlightButtonSize, 0.3f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        my_button.transform.DOScale(standardButtonSize, 0.3f);
    }




}
