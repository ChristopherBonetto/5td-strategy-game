using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Pulsating_Button_Tween : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    private Button my_button;
    public Text my_text;

    public float highlightButtonSize = 1.2f;
    public float highlightTextSize = 1.5f;
    public float standardButtonSize = 1f;
    public float standardTextSize = 1f;

    private void Start()
    {
        my_button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        my_button.transform.DOScale(highlightButtonSize, 0.3f);
        my_text.transform.DOScale(highlightTextSize, 0.6f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        my_text.transform.DOScale(standardTextSize, 0.6f);
        my_button.transform.DOScale(standardButtonSize, 0.3f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        my_text.transform.DOScale(standardTextSize, 0.6f);
        my_button.transform.DOScale(standardButtonSize, 0.3f);
    }
}
