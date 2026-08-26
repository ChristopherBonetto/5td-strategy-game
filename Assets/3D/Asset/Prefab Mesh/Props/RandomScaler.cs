using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomScaler : MonoBehaviour
{
    public float MinScale = .2f;
    public float MaxScale = 1f;


    private void Awake()
    {
        transform.localScale = Vector3.one * Random.Range(MinScale, MaxScale);
    }
}
