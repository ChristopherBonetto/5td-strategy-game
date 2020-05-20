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
    public float verticalOffset = 2;
    [SerializeField] private GameObject destinationMarker;




    void Start()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
        myLineRenderer = GetComponent<LineRenderer>();

        myLineRenderer.startWidth = 0.5f;
        destinationMarker.SetActive(false);
        myLineRenderer.endWidth = 0.5f;
        myLineRenderer.positionCount = 0;


    }


    void Update()
    {


        if (myNavMeshAgent.hasPath)
        {
            DrawPath();
        }
        else destinationMarker.SetActive(false);
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
            destinationMarker.SetActive(true);
                destinationMarker.transform.position = myNavMeshAgent.path.corners[myNavMeshAgent.path.corners.Length-1];
            
       
           
         
        }
            
    }
}
