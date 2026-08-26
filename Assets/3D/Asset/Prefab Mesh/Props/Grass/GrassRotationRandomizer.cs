using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassRotationRandomizer : MonoBehaviour
{
    private void Awake()
    {
        float rotation = Random.Range(0, 360);
        transform.localRotation = Quaternion.Euler(0,rotation,0);
    }
}
