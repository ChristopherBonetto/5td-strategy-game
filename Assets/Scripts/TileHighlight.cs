using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlight : MonoBehaviour
{
    private Material startMaterial;
    public Material highlightMaterial;
    bool mouseOver = false;

    private void Awake()
    {
        startMaterial = GetComponent<Renderer>().material;
    }

    private void OnMouseOver()
    {
        mouseOver = true;
        GetComponent<Renderer>().material = highlightMaterial;
    }

    private void OnMouseExit()
    {
        mouseOver = false;
        GetComponent<Renderer>().material = startMaterial;
    }

}
