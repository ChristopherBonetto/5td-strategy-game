using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squashandstretch : MonoBehaviour
{
    public float m_Acceleration;
    public float m_PosAccel;
    private Material Mat;
    public float lastvelocity=1;
    public float velocity = 1;
    public Vector3 m_lastvelocity;
    public Vector3 m_velocity;
    private Rigidbody rb;
    public Vector3 LastPosition;
    public Vector3 CurrentPosition;
    private void Start()
    {
        rb = GetComponent < Rigidbody >();
        Mat = GetComponent<Renderer>().material;
        lastvelocity = rb.velocity.magnitude;
        LastPosition = transform.position;
        m_lastvelocity = rb.velocity;
    }
    void Update()
    {
        StartCoroutine(Acceleration());
        if (rb.velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z)),90f);
        }
    }
    public IEnumerator Acceleration()
    {
        m_velocity = rb.velocity;
        velocity = rb.velocity.magnitude;
        CurrentPosition = transform.position;
        float posdiff =  Vector3.Distance(CurrentPosition, LastPosition);
        m_PosAccel = (posdiff / Time.deltaTime)/Time.deltaTime;

        Vector3 trueaccel = new Vector3((m_velocity.x - m_lastvelocity.x) / Time.deltaTime, (m_velocity.y - m_lastvelocity.y) / Time.deltaTime, (m_velocity.z - m_lastvelocity.z) / Time.deltaTime);
        float velocitydiff = velocity - lastvelocity;
        m_Acceleration = velocity / Time.deltaTime;
        Mat.SetFloat("_AccelX",trueaccel.x );
        Mat.SetFloat("_AccelY", trueaccel.y);
        Mat.SetFloat("_AccelZ", trueaccel.z);
        lastvelocity = velocity;
        LastPosition = CurrentPosition;
        Debug.Log(trueaccel);
        yield return new WaitForFixedUpdate();
    }
}
