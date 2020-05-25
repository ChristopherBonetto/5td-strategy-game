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
    public SpriteRenderer spriteColor;
    public Color selectedColor;
    public Color startColor;
    public Sprite[] RandomSpriteList;

    


    private void Awake()
    {
        
        coll = GetComponentInParent<BoxCollider>();
        isOnPlayableArea = false;
        spriteColor = GetComponent<SpriteRenderer>();
        //spriteColor.sprite = RandomSpriteList[Random.Range(0, RandomSpriteList.Length-1)];
        //startColor = spriteColor.color;


    }
    public void Start()
    {
        if (coll.gameObject.layer != LayerMask.NameToLayer("Terrain"))
        {
            gameObject.SetActive(false);
        }
    }

    public void MouseEnter ()
    {
        transform.DOScale(1.2f, 0.3f);
        transform.DOShakePosition(0.3f, new Vector3(0.3f, 0, 0.3f),10,90,false,false).SetLoops(2, LoopType.Yoyo);



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

        transform.DOScale(1f, 0.3f);
        spriteColor.DOColor(startColor, 0.2f);

    }

 


}

