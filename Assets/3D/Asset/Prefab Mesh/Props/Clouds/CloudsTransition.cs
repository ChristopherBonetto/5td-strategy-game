using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CloudsTransition : MonoBehaviour
{
    public float fadeTime = .5f;
    public GameObject fadeLeft;
    public GameObject fadeRight;

    private void Start()
    {
        Destroy(fadeLeft, fadeTime);
        Destroy(fadeRight, fadeTime);
        
    }
}
