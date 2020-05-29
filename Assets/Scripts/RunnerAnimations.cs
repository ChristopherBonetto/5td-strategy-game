using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunnerAnimations : MonoBehaviour
{
    public NavMeshAgent m_agent;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
            anim.SetFloat("Speed", m_agent.velocity.magnitude);
    }
}
