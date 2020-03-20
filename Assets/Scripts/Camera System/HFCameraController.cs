using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFCameraController : MonoBehaviour
{
    private Camera m_cam;
    private Transform m_transform;

    [Header("Camera variables")]

    [SerializeField]
    private Transform m_Target;

    [SerializeField]
    private float m_CameraMovementSpeed;

    [SerializeField]
	private float m_DistanceFromTarget = 10.0f;

    [SerializeField]
    private Collider m_Bounds;


    [Header("Mouse variables")]

    [SerializeField]
    private float m_SensitivityOnXAngle = 1.0f;

    [SerializeField]
    private float m_SensitivityOnYAngle = 0.1f;

    [SerializeField, Range(0.05f, 1)]
    private float m_AngularFriction = 0.1f;

    [SerializeField]
    private float m_ScrollSensitivity;

    [SerializeField]
    private float m_ScrollSpeed;

    private float m_ActualMouseScrollValue;



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

    private float m_CurrentAngleY;
    private float m_CurrentAngleX;

    private float m_ActualMouseXValue;
    private float m_ActualMouseYValue;

    public const float X_MIN_ANGLE = 20.0f;
    public const float X_MAX_ANGLE = 50.0f;



    private void Start()
    {
        m_transform = transform;
        m_cam = Camera.main;
        m_ActualMouseScrollValue = m_cam.fieldOfView;
    }

    private void Update()
    {
        Rotate();
        UpdateFielOfView();
        MoveTarget();
    }

    private void LateUpdate()
    {
        Vector3 direction = new Vector3(0, 0, -m_DistanceFromTarget);
        Quaternion rotation = Quaternion.Euler(m_CurrentAngleX, m_CurrentAngleY, 0);
        m_transform.position = m_Target.position + rotation * direction;
        m_transform.LookAt(m_Target);

        m_cam.fieldOfView = Mathf.Lerp(m_cam.fieldOfView, m_ActualMouseScrollValue, Time.deltaTime * m_ScrollSpeed);
    }

    private void Rotate()
    {
        // GetMouseButton(2) = click on scroll wheel
        if (Input.GetMouseButton(2))
        {
            m_ActualMouseXValue = Input.GetAxis("Mouse X") * m_SensitivityOnXAngle;
            m_ActualMouseYValue = -Input.GetAxis("Mouse Y") * m_SensitivityOnXAngle;
        }
        // GetMouseButton(2) = click on scroll wheel
        else if (!Input.GetMouseButton(2))
        {
            if (m_ActualMouseXValue != 0 || m_ActualMouseYValue != 0)
            {
                m_ActualMouseXValue -= (m_ActualMouseXValue * m_AngularFriction);
                m_ActualMouseYValue -= (m_ActualMouseYValue * m_AngularFriction);
            }
        }

        m_CurrentAngleX += m_ActualMouseYValue;
        m_CurrentAngleY += m_ActualMouseXValue;

        m_CurrentAngleX = Mathf.Clamp(m_CurrentAngleX, X_MIN_ANGLE, X_MAX_ANGLE);
    }

    private void UpdateFielOfView()
    {
        m_ActualMouseScrollValue += -Input.GetAxis("Mouse ScrollWheel") * m_ScrollSensitivity;
        m_ActualMouseScrollValue = Mathf.Clamp(m_ActualMouseScrollValue, 30.0f, 60.0f);
    }

    private void MoveTarget()
    {
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");


        if (x != 0 || z != 0)
        {
            Vector3 forwardDir = (m_Target.position - m_transform.position).normalized;
            forwardDir.y = 0;

            float speedOnX = x * m_CameraMovementSpeed * Time.deltaTime;
            float speedOnZ = z * m_CameraMovementSpeed * Time.deltaTime;

            Vector3 origin = m_Bounds.bounds.center;
            float minBoundOnX = origin.x - m_Bounds.bounds.extents.x;
            float maxBoundOnX = origin.x + m_Bounds.bounds.extents.x;
            float minBoundOnZ = origin.z - m_Bounds.bounds.extents.z;
            float maxBoundOnZ = origin.z + m_Bounds.bounds.extents.z;

            if (speedOnX > 0 && m_Target.position.x < maxBoundOnX)
                m_Target.position += m_transform.right * speedOnX;

            if (speedOnX < 0 && m_Target.position.x > minBoundOnX)
                m_Target.position += m_transform.right * speedOnX;

            if (speedOnZ > 0 && m_Target.position.z < maxBoundOnZ)
                m_Target.position += forwardDir * speedOnZ;

            if (speedOnZ < 0 && m_Target.position.z > minBoundOnZ)
                m_Target.position += forwardDir * speedOnZ;
        }
    }
}
