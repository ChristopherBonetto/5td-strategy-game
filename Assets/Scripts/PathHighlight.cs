using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]

public class PathHighlight : MonoBehaviour
{
    private LineRenderer myLineRenderer;
    private NavMeshAgent myNavMeshAgent;




    void Start()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
        myLineRenderer = GetComponent<LineRenderer>();

        myLineRenderer.startWidth = 0.15f;
        myLineRenderer.endWidth = 0.15f;
        myLineRenderer.positionCount = 0;
    }

    // Update is called once per frame
    void Update()
    {


        if (myNavMeshAgent.hasPath)
        {
            DrawPath();
        }
    }
    void DrawPath()
    {
        myLineRenderer.positionCount = myNavMeshAgent.path.corners.Length;
        myLineRenderer.SetPosition(0, transform.position);

     
        
       
       


        if (myNavMeshAgent.path.corners.Length < 2)
        {
            return;
        }

        for(int i=1;i<myNavMeshAgent.path.corners.Length;i++)
        {
            Vector3 pointPosition = new Vector3(myNavMeshAgent.path.corners[i].x, myNavMeshAgent.path.corners[i].y, myNavMeshAgent.path.corners[i].z);
            myLineRenderer.SetPosition(i, pointPosition);
         
        }
            
    }
}
