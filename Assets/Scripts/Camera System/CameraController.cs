using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera m_cam;
    private Transform m_transform;

    [Header("Camera variables")]

    [SerializeField]
    private Transform m_Target;
    public Transform Target => m_Target;

    [SerializeField]
	private float m_DistanceFromTarget = 10.0f;
    public float DistanceFromTarget => m_DistanceFromTarget;

    [Header("Mouse variables")]

    [SerializeField]
    private float m_SensitivityOnXAngle = 1.0f;
    public float SensitivityOnXAngle => m_SensitivityOnXAngle;

    [SerializeField]
    private float m_SensitivityOnYAngle = 0.1f;
    public float SensitivityOnYAngle => m_SensitivityOnYAngle;

    [SerializeField]
    private float m_AngularFriction = 0.1f;
    public float AngularFriction => m_AngularFriction;

    [SerializeField]
    private float m_ScrollSensitivity;
    public float ScrollSensitivity => m_ScrollSensitivity;


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
    public float CurrentAngleY => m_CurrentAngleY;

    private float m_CurrentAngleX;
    public float CurrentAngleX => m_CurrentAngleX;

    private float m_ActualMouseXValue;
    public float ActualMouseXValue => m_ActualMouseXValue;

    private float m_ActualMouseYValue;
    public float ActualMouseYValue => m_ActualMouseYValue;

    public const float X_MIN_ANGLE = 10.0f;
    public const float X_MAX_ANGLE = 50.0f;



    private void Start()
    {
        m_transform = transform;
        m_cam = Camera.main;
    }

    private void Update()
    {
        Rotate();
        UpdateFielOfView();
    }

    private void LateUpdate()
    {
        Vector3 direction = new Vector3(0, 0, -DistanceFromTarget);
        Quaternion rotation = Quaternion.Euler(CurrentAngleX, CurrentAngleY, 0);
        m_transform.position = Target.position + rotation * direction;
        m_transform.LookAt(Target);
    }

    private void Rotate()
    {
        if (Input.GetMouseButton(1))
        {
            m_ActualMouseXValue = Input.GetAxis("Mouse X");
            m_ActualMouseYValue -= Input.GetAxis("Mouse Y");
        }
        else if (!Input.GetMouseButton(1))
        {
            if (ActualMouseXValue != 0 || ActualMouseYValue != 0)
            {
                m_ActualMouseXValue -= (m_ActualMouseXValue * AngularFriction);
                m_ActualMouseYValue -= (m_ActualMouseYValue * AngularFriction);
            }
        }

        m_CurrentAngleX += m_ActualMouseYValue * SensitivityOnXAngle;
        m_CurrentAngleY += m_ActualMouseXValue * SensitivityOnYAngle;
        m_CurrentAngleX = Mathf.Clamp(m_CurrentAngleX, X_MIN_ANGLE, X_MAX_ANGLE);
    }

    private void UpdateFielOfView()
    {
        m_cam.fieldOfView -= Input.mouseScrollDelta.y * ScrollSensitivity;
        m_cam.fieldOfView = Mathf.Clamp(m_cam.fieldOfView, 50.0f, 70.0f);
    }
}
