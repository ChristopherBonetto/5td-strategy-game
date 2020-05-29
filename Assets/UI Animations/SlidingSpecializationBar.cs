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





    void Start()
    {
        //if (upgradeButton.enabled == false)
        //{
        //    slidingBarImage.enabled = true;
        //}
        //else slidingBarImage.enabled = false;


    }

    private void OnEnable()
    {
        slidingBar.DOScaleX(1f, .5f);
        slidingBar.DOShakeScale(.2f, new Vector3(.1f, 0, 0), 10, 50, true);
        slidingBar.DOAnchorPosX(50, 0.5f);
 
    }
    private void OnDisable()
    {
        slidingBar.DOScaleX(0.3f, .5f);
        slidingBar.DOAnchorPosX(-120, 0.5f);
    }

    









}
