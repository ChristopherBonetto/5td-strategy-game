using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeRight : MonoBehaviour
{
    public float speed = 5f;
    public float acceleration = 1.5f;
    public float accelerationDelay = .5f;
    private float currentTimer = 0f;

    private void Start()
    {
        currentTimer = 0f;
    }


    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
        if (currentTimer >= accelerationDelay)
        {
            currentTimer = 0;
            speed = (speed * acceleration);
        }
        currentTimer += Time.deltaTime;

    }
}
