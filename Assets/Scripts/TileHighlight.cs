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

            UpdateTiles();
  
    }
    private void OnDestroy()
    {
        HFEventManager.UnsubscribeFrom<EntityBehavior, int>(HFEventID.OnUnitSelected, EnableTiles);
    }

   
    public void OnMouseOver()
    {
    
        if(isActive==true)
        {
            transform.DOScale(1.25f, 0.3f);
            SpRender.DOFade(0.6f, 0.3f);
           

            if (Input.GetMouseButtonDown(1))
            {
                transform.DOScale(1.5f, 0.3f);
                SpRender.DOColor(selectedColor, 0.2f);
            }

        }
 

}
    public void OnMouseExit()
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
        if (isActive)
        {

            TileActivationRadius = HFGameManager.Instance.TileActivationRadius;
            RelativePos = Cam.WorldToScreenPoint(transform.position);
            Vector2 FlatRelativePos = new Vector2(RelativePos.x, RelativePos.y);
            MousePos = Input.mousePosition;
            Vector2 FlatMousePos = new Vector2(MousePos.x, MousePos.y);
            Vector2 Vectordist = FlatRelativePos - FlatMousePos;
            float Rotationfactor = 1 / Mathf.Sin(Mathf.Deg2Rad * Cam.transform.eulerAngles.x);
            Vectordist.y /= Screen.width / 1000f;
            Vectordist.x /= Screen.width / 1000f;
            Vectordist.y = Vectordist.y * Rotationfactor ;

            float dist = Vectordist.magnitude;
            float DistanceFactor = Mathf.Round(dist / (HFGameManager.Instance.TileDistanceFactor*(TileActivationRadius / (RelativePos.z)))*HFGameManager.Instance.TileQuantizationFactor) / HFGameManager.Instance.TileQuantizationFactor;
            if (enabledebug)
            {

                Debug.Log("Cam Angle: " + Cam.transform.eulerAngles.x + " RotFac: " + Rotationfactor);
            }

            if (dist > TileActivationRadius / RelativePos.z && SpRender.color.a != 0f)
            {



                SpRender.DOFade(0f, 0.2f)
                    .SetEase(Ease.OutSine);


            }
            else if (dist <= TileActivationRadius / RelativePos.z && SpRender.color.a != BaseAlpha / 255f * (1 - DistanceFactor))
            {

                float tmp = BaseAlpha / 255f * (1 - DistanceFactor);
                SpRender.DOFade(tmp, 0.2f)
                    .SetEase(Ease.OutSine);
                transform.DOScale(1.2f * (1 - DistanceFactor), 0.3f);
                    //.SetEase(Ease.OutElastic);
            }

        }
        else if(!isActive && SpRender.color.a != 0f)
        {

            SpRender.DOFade(0f, 0.2f)
                .SetEase(Ease.OutSine);
        }
    }
}




