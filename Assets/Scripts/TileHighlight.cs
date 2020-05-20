using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using DG.Tweening;

public class TileHighlight : MonoBehaviour
{

    bool mouseOver = false;
    private BoxCollider coll;
    private bool isOnPlayableArea;
    private SpriteRenderer spriteColor;
    public Color selectedColor;
    private Color startClor;

    


    private void Awake()
    {
        coll = GetComponent<BoxCollider>();
        isOnPlayableArea = false;
        spriteColor = GetComponent<SpriteRenderer>();
        startClor = spriteColor.color;

    }

    public void MouseEnter ()
    {
        mouseOver = true;
        transform.DOScale(1f, 0.3f);
        coll.size = new Vector3(6f, 6f, 0.001f);
        transform.DOShakePosition(0.3f, new Vector3(0.1f, 0, 0.1f),10,90,false,false).SetLoops(2, LoopType.Yoyo);



    }

    public void OnClick()
    {
        transform.DOScale(1.5f, 0.3f).SetLoops(2, LoopType.Yoyo);
        spriteColor.DOColor(selectedColor, 0.2f).SetLoops(2, LoopType.Yoyo);
    }
    //public void OnMouseOver()
    //{
  
    //    if (Input.GetMouseButtonDown(1))
    //    {
    //        transform.DOScale(1.5f, 0.3f);
    //        spriteColor.DOColor(selectedColor, 0.2f).SetLoops(2, LoopType.Yoyo);
    //    }
    //    else if (Input.GetMouseButtonUp(1))
    //    {
    //        transform.DOScale(1f, 0.3f);
    //    }
    //}


    public void MouseExit()
    {

        mouseOver = false;
        transform.DOScale(0.8f, 0.3f);
        coll.size = new Vector3(6f, 6f, 0.001f); 
        spriteColor.DOColor(startClor, 0.2f);

    }

 


}

