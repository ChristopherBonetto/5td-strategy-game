using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class CloudsMovement : MonoBehaviour
{

    public Transform startPosition;

    public float minSpeed = 0.2f;
    public float maxSpeed = 0.4f;
    public float minScale = 1f;
    public float maxScale = 1.4f;
    private float speed;
    private Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        transform.DOScale(Random.Range(minScale, maxScale), 0f);
        speed = Random.Range(minSpeed, maxSpeed);
    }
    private void Update()
    {
        rb.velocity = new Vector3(speed, rb.velocity.y, rb.velocity.z);
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
            transform.position = new Vector3(startPosition.transform.position.x, transform.position.y, transform.position.z);
            speed = Random.Range(minSpeed, maxSpeed);
            transform.DOScale(Random.Range(minScale, maxScale), 0f);
        }
    }
}
