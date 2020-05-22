using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent( typeof(LineRenderer) )]
public class CurvedLineRenderer : MonoBehaviour 
{
	//PUBLIC
	public float lineSegmentSize = 0.15f;
	public float lineWidth = 0.1f;
	[Header("Gizmos")]
	public bool showGizmos = true;
	public float gizmoSize = 0.1f;
	public Color gizmoColor = new Color(1,0,0,0.5f);
    public float VerticalOffset;
	//PRIVATE
	private Vector3[] linePoints = new Vector3[0];
	private Vector3[] linePositions = new Vector3[0];
	private Vector3[] linePositionsOld = new Vector3[0];

	// Update is called once per frame
	public void Update () 
	{
		GetPoints();
		SetPointsToLine();
	}

	void GetPoints()
	{
		//find curved points in children
		linePoints = this.GetComponent<NavMeshAgent>().path.corners;

        for (int i = 1; i < linePoints.Length; i++)
        {

            if (linePoints[i].y > linePoints[i - 1].y)
            {
                linePoints[i] = new Vector3(linePoints[i].x, linePoints[i].y + VerticalOffset, linePoints[i].z);

                //myLineRenderer.SetPosition(i, pointPosition);
            }
            else
            {
                linePoints[i] = new Vector3(linePoints[i].x, linePoints[i].y, linePoints[i].z);
                //myLineRenderer.SetPosition(i, pointPosition);
            }
            //destinationMarker.SetActive(true);
            //    destinationMarker.transform.position = myNavMeshAgent.path.corners[myNavMeshAgent.path.corners.Length-1];




        }
        //add positions
        linePositions = new Vector3[linePoints.Length];
		for( int i = 0; i < linePoints.Length; i++ )
		{
			linePositions[i] = linePoints[i];
		}
	}

	void SetPointsToLine()
	{
		//create old positions if they dont match
		if( linePositionsOld.Length != linePositions.Length )
		{
			linePositionsOld = new Vector3[linePositions.Length];
		}

		//check if line points have moved
		bool moved = false;
		for( int i = 0; i < linePositions.Length; i++ )
		{
			//compare
			if( linePositions[i] != linePositionsOld[i] )
			{
				moved = true;
			}
		}

		//update if moved
		if( moved == true )
		{
			LineRenderer line = this.GetComponent<LineRenderer>();

			//get smoothed values
			Vector3[] smoothedPoints = LineSmoother.SmoothLine( linePositions, lineSegmentSize );

            //set line settings
            line.positionCount = smoothedPoints.Length;
			line.SetPositions( smoothedPoints );
            line.startWidth = lineWidth;
            line.endWidth = lineWidth;
		}
	}

	void OnDrawGizmosSelected()
	{
		Update();
	}

	void OnDrawGizmos()
	{
		if( linePoints.Length == 0 )
		{
			GetPoints();
		}

		//settings for gizmos

	}
}
