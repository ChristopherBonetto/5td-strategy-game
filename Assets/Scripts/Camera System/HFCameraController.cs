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

    [SerializeField]
	private float m_distanceFromTarget = 10.0f;

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
    private float m_scrollSensitivity;

    [SerializeField]
    private float m_scrollSpeed;
    private float m_actualMouseScrollValue;



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

    public const float X_MIN_ANGLE = 20.0f;
    public const float X_MAX_ANGLE = 50.0f;



    private void Start()
    {
        m_transform = transform;
        m_cam = Camera.main;
        m_actualMouseScrollValue = m_cam.fieldOfView;
    }

    private void Update()
    {
        Rotate();
        UpdateFielOfView();
        MoveTarget();
    }

    private void LateUpdate()
    {
        Vector3 direction = new Vector3(0, 0, -m_distanceFromTarget);
        Quaternion rotation = Quaternion.Euler(m_currentAngleX, m_currentAngleY, 0);
        m_transform.position = m_target.position + rotation * direction;
        m_transform.LookAt(m_target);

        m_cam.fieldOfView = Mathf.Lerp(m_cam.fieldOfView, m_actualMouseScrollValue, Time.deltaTime * m_scrollSpeed);
    }

    private void Rotate()
    {
        // GetMouseButton(2) = click on scroll wheel
        if (Input.GetMouseButton(2))
        {
            m_actualMouseXValue = Input.GetAxis("Mouse X") * m_sensitivityOnXAngle;
            m_actualMouseYValue = -Input.GetAxis("Mouse Y") * m_sensitivityOnYAngle;
        }
        // GetMouseButton(2) = click on scroll wheel
        else if (!Input.GetMouseButton(2))
        {
            if (m_actualMouseXValue != 0 || m_actualMouseYValue != 0)
            {
                m_actualMouseXValue -= (m_actualMouseXValue * m_angularFriction);
                m_actualMouseYValue -= (m_actualMouseYValue * m_angularFriction);
            }
        }

        m_currentAngleX += m_actualMouseYValue;
        m_currentAngleY += m_actualMouseXValue;

        m_currentAngleX = Mathf.Clamp(m_currentAngleX, X_MIN_ANGLE, X_MAX_ANGLE);
    }

    private void UpdateFielOfView()
    {
        m_actualMouseScrollValue += -Input.GetAxis("Mouse ScrollWheel") * m_scrollSensitivity;
        m_actualMouseScrollValue = Mathf.Clamp(m_actualMouseScrollValue, 30.0f, 60.0f);
    }

    private void MoveTarget()
    {
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");


        if (x != 0 || z != 0)
        {
            Vector3 forwardDir = (m_target.position - m_transform.position).normalized;
            forwardDir.y = 0;

            float speedOnX = x * m_cameraMovementSpeed * Time.deltaTime;
            float speedOnZ = z * m_cameraMovementSpeed * Time.deltaTime;

            Vector3 origin = m_Bounds.bounds.center;
            float minBoundOnX = origin.x - m_Bounds.bounds.extents.x;
            float maxBoundOnX = origin.x + m_Bounds.bounds.extents.x;
            float minBoundOnZ = origin.z - m_Bounds.bounds.extents.z;
            float maxBoundOnZ = origin.z + m_Bounds.bounds.extents.z;

            m_target.position += forwardDir * speedOnZ + m_transform.right * speedOnX;

            float clampOnX = Mathf.Clamp(m_target.position.x, minBoundOnX, maxBoundOnX);
            float clampOnZ = Mathf.Clamp(m_target.position.z, minBoundOnZ, maxBoundOnZ);
            m_target.position = new Vector3(clampOnX, m_target.position.y, clampOnZ);
        }
    }
}
