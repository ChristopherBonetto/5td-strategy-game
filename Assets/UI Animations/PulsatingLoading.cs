using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PulsatingLoading : MonoBehaviour
{
    public Text my_text;

  

    void Start()
    {
        my_text = GetComponent<Text>();
      
    }


    void Update()
    {
        my_text.transform.DOScale(2f, 10f);
    }
}
