using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlight : MonoBehaviour
{

    bool mouseOver = false;
    private BoxCollider coll;
    private bool isOnPlayableArea;

    private void Awake()
    {
        coll = GetComponent<BoxCollider>();
        isOnPlayableArea = false;
    }

    private void OnMouseOver()
    {

            mouseOver = true;
        transform.localScale = new Vector3(1f, 1f, 1f);
        coll.size = new Vector3(6f, 6f, 0.1f);
        if(Input.GetMouseButton(1))
        {
            StartCoroutine(FadeTile());
        }
        

    }

    private void OnMouseExit()
    {
        mouseOver = false;
        transform.localScale = new Vector3(0.01f, 0.01f, 1f);
        coll.size = new Vector3(600f, 600f, 0.2f);
       
    }

    IEnumerator FadeTile()
    {
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        yield return new WaitForSeconds(0.2f);
        
    }

  

}
