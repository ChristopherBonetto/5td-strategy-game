using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpsesMassRandomizer : MonoBehaviour
{
    private Rigidbody m_rigidbody;
    private float mass;
    public float minMultiplier = .6f;
    public float maxMultiplier = 1.4f;

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        mass = Random.Range(m_rigidbody.mass * minMultiplier, m_rigidbody.mass * maxMultiplier);
        m_rigidbody.mass = mass;
        m_rigidbody.AddForce(4f*Random.Range(m_rigidbody.mass * minMultiplier, m_rigidbody.mass * maxMultiplier), 4f*Random.Range(m_rigidbody.mass * minMultiplier, m_rigidbody.mass * maxMultiplier), 4f*Random.Range(m_rigidbody.mass * minMultiplier, m_rigidbody.mass * maxMultiplier), ForceMode.Impulse);
    }
}
