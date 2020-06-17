using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;


public class SlidingSpecializationBar : MonoBehaviour
{

    public RectTransform slidingBar;
    public Image slidingBarImage;
    public Button upgradeButton;

    public float onEnablePos = 0;
    public float onDisablePos = -80f;


    private void OnEnable()
    {
        if (!upgradeButton.gameObject.activeSelf)
        {
            slidingBar.DOScaleX(.75f, .5f);
            slidingBar.DOShakeScale(.2f, new Vector3(.1f, 0, 0), 10, 50, true);
            slidingBar.DOAnchorPosX(onEnablePos, 0.5f);
        }
 
    }
    private void OnDisable()
    {
        slidingBar.DOScaleX(0.3f, .5f);
        slidingBar.DOAnchorPosX(onDisablePos, 0.5f);
    }
}
