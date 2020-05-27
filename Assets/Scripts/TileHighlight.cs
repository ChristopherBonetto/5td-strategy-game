using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using DG.Tweening;


public class TileHighlight : MonoBehaviour
{


    public bool enabledebug = false;

    [Range(0, 255)]
    public float BaseAlpha = 30;
    public float TileActivationRadius = 15;
    public SpriteRenderer SpRender;
    public Color selectedColor;
    public Color startColor;
    public Sprite[] RandomSpriteList;



    private BoxCollider coll;
    private bool isOnPlayableArea;
    public bool isActive;
    private Camera Cam;
    private Vector3 RelativePos;
    private Vector3 MousePos;

    bool mouseOver = false;




    private void Awake()
    {
        isOnPlayableArea = false;
        coll = GetComponentInParent<BoxCollider>();
        SpRender = GetComponent<SpriteRenderer>();
        Cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        //spriteColor.sprite = RandomSpriteList[Random.Range(0, RandomSpriteList.Length-1)];
        //startColor = spriteColor.color;

        if (coll.gameObject.layer != LayerMask.NameToLayer("Terrain"))
        {
            gameObject.SetActive(false);
        }
    }
    public void Start()
    {
        isActive = false;
        HFEventManager.SubscribeTo<EntityBehavior, int>(HFEventID.OnUnitSelected, EnableTiles);
        Color tmp = SpRender.color;
        tmp.a = 0f;
        SpRender.color = tmp;

    }
    private void Update()
    {
        if (isActive)
        {

            UpdateTiles();
        }
    }
    private void OnDestroy()
    {
        HFEventManager.UnsubscribeFrom<EntityBehavior, int>(HFEventID.OnUnitSelected, EnableTiles);
    }
    public void MouseEnter()
    {
        transform.DOScale(1.2f, 0.3f);
        transform.DOShakePosition(0.3f, new Vector3(0.3f, 0, 0.3f), 10, 90, false, false).SetLoops(2, LoopType.Yoyo);



    }

    public void OnClick()
    {
        transform.DOScale(1.5f, 0.3f)/*.SetLoops(2, LoopType.Yoyo)*/;
        SpRender.DOColor(selectedColor, 0.2f)/*.SetLoops(2, LoopType.Yoyo)*/;
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
        SpRender.DOColor(startColor, 0.2f);

    }
    void EnableTiles(EntityBehavior entity, int team)
    {
        if (entity != null && entity.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            isActive = true;

        }
        else 
        { 

            isActive = false;
            Color tmp = SpRender.color;
            tmp.a = 0;
            SpRender.color = tmp;

        }

    }
    public void UpdateTiles()
    {

        TileActivationRadius = HFGameManager.Instance.TileActivationRadius;
        RelativePos = Cam.WorldToScreenPoint(transform.position);
        Vector2 FlatRelativePos = new Vector2(RelativePos.x, RelativePos.y);
        MousePos = Input.mousePosition;
        Vector2 FlatMousePos = new Vector2(MousePos.x, MousePos.y);
        Vector2 Vectordist = FlatRelativePos - FlatMousePos;
        float Rotationfactor = 1/Mathf.Sin(Mathf.Deg2Rad *Cam.transform.eulerAngles.x);
        Vectordist.y = Vectordist.y* Rotationfactor;
        float dist = Vectordist.magnitude;
        if (enabledebug)
        {

            Debug.Log("Cam Angle: "+Cam.transform.eulerAngles.x+" RotFac: "+ Rotationfactor);
        }
 



        Color tmp = SpRender.color;
        if (dist > TileActivationRadius/ RelativePos.z && SpRender.color.a !=0f)
        {
            if (enabledebug)
            {
                Debug.Log("Tile is hidden");
            }
            
            tmp.a = 0f;
            SpRender.color = tmp;
        }
        else if(dist <= TileActivationRadius/RelativePos.z && SpRender.color.a != BaseAlpha / 255f * (1 - dist / (TileActivationRadius / (RelativePos.z))))
        {
            if (enabledebug)
            {
                Debug.Log("Tile is showing");
            }
            tmp.a = BaseAlpha/255f *(1- dist/(TileActivationRadius / (RelativePos.z)));
            SpRender.color = tmp;
        }


    }
}




