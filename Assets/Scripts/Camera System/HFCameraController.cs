using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFCameraController : MonoBehaviour
{
    private Camera m_cam;
    private Transform m_transform;

    [Header("Camera variables")]

    [SerializeField]
    private Transform m_target;

    [SerializeField]
    private float m_cameraMovementSpeed;


    [Header("Forward Movement Handler")]

    //--------------------------------------------------------------------------------------
    // The forward can be modified updating the distance (m_currentDistanceFromTarget) 
    // from target, it's a float value. The distance will be lerped from the point 'A' to 'B' 
    // at a given speed. The new distance (or m_actualDistance) is given by: 
    // (the mouse scroll whell value) * (forward speed sensititvity). If you want more smooth 
    // on distance value update, reduce the speed and increase the sensibility.
    //--------------------------------------------------------------------------------------
       
    [SerializeField]
	private float m_initialDistanceFromTarget = 60.0f;
    
    /// <summary>
    /// Last distance value (Initial lerp point).
    /// </summary>
    private float m_lastDistanceFromTarget;

    /// <summary>
    /// Current distance value (the percentage updated or 't').
    /// </summary>
    private float m_currentDistanceFromTarget;

    /// <summary>
    /// New distance value (end lerp point).
    /// </summary>
    private float m_actualDistanceFromTarget;

    [SerializeField, Tooltip("It's the value multiplied by mouse scroll wheel value")]
    private float m_forwardSensitivity;

    [SerializeField, Tooltip("It's the updating speed on the distance")]
    private float m_forwardSpeed;

    [SerializeField]
    private float MIN_DistanceFromTarget;

    [SerializeField]
    private float Max_DistanceDromTarget;


    [Header("Reference to the map bounce")]

    [SerializeField]
    private Collider m_Bounds;


    [Header("Mouse variables")]

    [SerializeField]
    private float m_sensitivityOnXAngle = 1.0f;

    [SerializeField]
    private float m_sensitivityOnYAngle = 0.1f;

    [SerializeField, Range(0.05f, 1)]
    private float m_angularFriction = 0.1f;

    [SerializeField]
    private float panBorderThickness = 20f;


    //------------------------------------------------
    // In order to update the camera angle we need some 
    // variables. 
    // CurrentAngle takes care about the angle given 
    // by the mouse position.
    // Actualangle takes care the actual mouse position.
    // I need to store the actual mouse position to apply
    // a smooth transition.
    // Constant variables clamp the rotation on X axis.
    //------------------------------------------------

    private float m_currentAngleY;
    private float m_currentAngleX;

    private float m_actualMouseXValue;
    private float m_actualMouseYValue;

    [SerializeField, Tooltip("Min angle value on x")]
    private float X_MIN_Angle = 20.0f;

    [SerializeField, Tooltip("Max angle value on x")]
    private float X_MAX_Angle = 50.0f;

    private void Start()
    {
        m_transform = transform;
        m_cam = Camera.main;
        m_actualDistanceFromTarget = m_initialDistanceFromTarget;
    }

    private void Update()
    {
        Rotate();
        MoveTarget();
        UpdateDistance();

    }

    private void LateUpdate()
    {
        Vector3 direction = new Vector3(0, 0, -m_currentDistanceFromTarget);
        Quaternion rotation = Quaternion.Euler(m_currentAngleX, m_currentAngleY, 0);
        m_transform.position = m_target.position + rotation * direction;
        m_transform.LookAt(m_target);
    }

    private void Rotate()
    {
        // GetMouseButton(2) = click on scroll wheel
        if (Input.GetMouseButton(0)|| Input.GetMouseButton(2))
        {
            m_actualMouseXValue = Input.GetAxis("Mouse X") * m_sensitivityOnXAngle;
            m_actualMouseYValue = -Input.GetAxis("Mouse Y") * m_sensitivityOnYAngle;
        }
        // GetMouseButton(2) = click on scroll wheel
        else if (!Input.GetMouseButton(0) || Input.GetMouseButton(2))
        {
            if (m_actualMouseXValue != 0 || m_actualMouseYValue != 0)
            {
                m_actualMouseXValue -= (m_actualMouseXValue * m_angularFriction);
                m_actualMouseYValue -= (m_actualMouseYValue * m_angularFriction);

                HFEventManager.TriggerEvent<TutorialID>(HFEventID.OnTutorialQuestCompleted, TutorialID.Rotate_Camera);
            }
        }

        m_currentAngleX += m_actualMouseYValue;
        m_currentAngleY += m_actualMouseXValue;

        if(Input.GetKey(KeyCode.E))
        {
            m_currentAngleY -= m_sensitivityOnYAngle;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            m_currentAngleY += m_sensitivityOnYAngle;
        }


        m_currentAngleX = Mathf.Clamp(m_currentAngleX, X_MIN_Angle, X_MAX_Angle);

        if(Input.GetKey(KeyCode.R))
        {
            m_currentAngleX = Mathf.Lerp(m_currentAngleX,X_MIN_Angle, m_sensitivityOnXAngle * m_angularFriction);
        }
        else if (Input.GetKey(KeyCode.T))
        {
            m_currentAngleX = Mathf.Lerp(m_currentAngleX,X_MAX_Angle,m_sensitivityOnXAngle*m_angularFriction);
        }
    }

    private void UpdateDistance()
    {
        m_actualDistanceFromTarget += -Input.GetAxis("Mouse ScrollWheel") * m_forwardSensitivity;
        m_actualDistanceFromTarget = Mathf.Clamp(m_actualDistanceFromTarget, MIN_DistanceFromTarget, Max_DistanceDromTarget);

        m_lastDistanceFromTarget = m_currentDistanceFromTarget;
        m_currentDistanceFromTarget = Mathf.Lerp(m_lastDistanceFromTarget, m_actualDistanceFromTarget, Time.deltaTime * m_forwardSpeed);
    }

    private void MoveTarget()
    {
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");


        if (x != 0 || z != 0)
        {
            m_target.rotation = Quaternion.Euler(0, m_transform.rotation.eulerAngles.y, 0);

            float speedOnX = x * m_cameraMovementSpeed * Time.deltaTime;
            float speedOnZ = z * m_cameraMovementSpeed * Time.deltaTime;

            Vector3 origin = m_Bounds.bounds.center;
            float minBoundOnX = origin.x - m_Bounds.bounds.extents.x;
            float maxBoundOnX = origin.x + m_Bounds.bounds.extents.x;
            float minBoundOnZ = origin.z - m_Bounds.bounds.extents.z;
            float maxBoundOnZ = origin.z + m_Bounds.bounds.extents.z;

            m_target.position += m_target.forward.normalized * speedOnZ + m_transform.right.normalized * speedOnX;

            float clampOnX = Mathf.Clamp(m_target.position.x, minBoundOnX, maxBoundOnX);
            float clampOnZ = Mathf.Clamp(m_target.position.z, minBoundOnZ, maxBoundOnZ);
            m_target.position = new Vector3(clampOnX, m_target.position.y, clampOnZ);

            HFEventManager.TriggerEvent<TutorialID>(HFEventID.OnTutorialQuestCompleted, TutorialID.Move_Camera);
        }

        //Vector3 pos = m_target.transform.localPosition;

        //if (Input.mousePosition.y >= Screen.height - panBorderThickness)
        //{
        //    pos.z += m_cameraMovementSpeed * Time.deltaTime;
        //}
        //if (Input.mousePosition.y <= panBorderThickness)
        //{
        //    pos.z -= m_cameraMovementSpeed * Time.deltaTime;
        //}
        //if (Input.mousePosition.x >= Screen.width - panBorderThickness)
        //{
        //    pos.x += m_cameraMovementSpeed * Time.deltaTime;
        //}
        //if (Input.mousePosition.x <= panBorderThickness)
        //{
        //    pos.x -= m_cameraMovementSpeed * Time.deltaTime;
        //}

        ////pos.x = Mathf.Clamp(pos.x, -m_Bounds.bounds.extents.x, m_Bounds.bounds.extents.x);
        ////pos.z = Mathf.Clamp(pos.z, -m_Bounds.bounds.extents.z, m_Bounds.bounds.extents.z);
        //m_target.transform.localPosition = pos;






    }


}

