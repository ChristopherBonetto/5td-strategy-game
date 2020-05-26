using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingAnimations : MonoBehaviour
{
    public Text[] dots;
    public float delay= 0.5f;
    bool isactive = false;

    void Start()
    {
        dots[0].enabled = false;
        dots[1].enabled = false;
        dots[2].enabled = false;
        Debug.Log("start");
    }


    void Update()
    {
        StartCoroutine(TypeDots());
        Debug.Log("update");

    }
    IEnumerator TypeDots()
    {
        isactive = true;
        dots[0].enabled = true;
        yield return new WaitForSeconds(delay);
        dots[1].enabled = true;
        yield return new WaitForSeconds(delay);
        dots[2].enabled = true;
        yield return new WaitForSeconds(delay);
        dots[0].enabled = false;
        dots[1].enabled = false;
        dots[2].enabled = false;
        yield return new WaitForSeconds(delay);
        isactive = false;
    }
}
