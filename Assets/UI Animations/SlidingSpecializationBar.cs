using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;


public class SlidingSpecializationBar : MonoBehaviour
{

    public RectTransform slidingBar;



    void Start()
    {
        slidingBar = GetComponent<RectTransform>();

    }

    private void OnEnable()
    {
        slidingBar.DOAnchorPos(new Vector2(150, 0), 2f);
        slidingBar.DOShakeAnchorPos(1f, new Vector2(20, 0), 10, 90, false, true);

    }
    private void OnDisable()
    {
        slidingBar.DOAnchorPos(new Vector2(150, 0), 0.6f);
    }

    









}
