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
    public float heightDistance = 1f;
    
    public Vector3[] pointPosition = new Vector3[0];
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
 
        pointPosition = new Vector3[myNavMeshAgent.path.corners.Length];
        


        if (pointPosition.Length < 2)
        {
            return;
        }

        for(int i=1;i< pointPosition.Length;i++)
        {
            Debug.Log(myNavMeshAgent.path.corners[i]);
            if (myNavMeshAgent.path.corners[i - 1].y - myNavMeshAgent.path.corners[i].y < -heightDistance)
            {
                Debug.Log("fjfgnòdòggògdnfgdnglkdgdg");
                if(myNavMeshAgent.path.corners[i].y== myNavMeshAgent.path.corners[myNavMeshAgent.path.corners.Length-1].y)
                {
                    pointPosition[i] = new Vector3(myNavMeshAgent.path.corners[i].x, myNavMeshAgent.path.corners[i].y , myNavMeshAgent.path.corners[i].z);
                }
                else pointPosition[i] = new Vector3(myNavMeshAgent.path.corners[i].x, myNavMeshAgent.path.corners[i].y+verticalOffset, myNavMeshAgent.path.corners[i].z);

            }
            else if (myNavMeshAgent.path.corners[i - 1].y - myNavMeshAgent.path.corners[i].y > heightDistance)
            {
                Debug.Log("43255454364565475685688769987988");
                if (myNavMeshAgent.path.corners[i].y == myNavMeshAgent.path.corners[myNavMeshAgent.path.corners.Length - 1].y)
                {
                    pointPosition[i] = new Vector3(myNavMeshAgent.path.corners[i].x, myNavMeshAgent.path.corners[i].y, myNavMeshAgent.path.corners[i].z);
                }
                else pointPosition[i] = new Vector3(myNavMeshAgent.path.corners[i].x, myNavMeshAgent.path.corners[i].y - verticalOffset, myNavMeshAgent.path.corners[i].z);

            }

            destinationMarker.SetActive(true);
            destinationMarker.transform.position = myNavMeshAgent.path.corners[myNavMeshAgent.path.corners.Length - 1];




        }
            
    }
}
