using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState
{
    Main,
    Map,
    Inkwell,
    Banner,
    Drawer
}

public class HFCameraActions : MonoBehaviour
{

    private Animator m_cameraAnimator;

    private CameraState m_currentCameraState = CameraState.Main;
    public CameraState CurrentCameraState
    {
        get
        {
            return m_currentCameraState;
        }
        set
        {
            if(m_currentCameraState == CameraState.Main)
            {
                m_currentCameraState = value;
                CameraStateAnimations(m_currentCameraState);
            }
            else
            {
                if(value == CameraState.Main)
                {
                    m_currentCameraState = value;
                    CameraStateAnimations(m_currentCameraState);
                }
            }
            
        }
    }



    private void Awake()
    {
        m_cameraAnimator = gameObject.GetComponent<Animator>();
    }

    private void Start()
    {
        m_currentCameraState = CameraState.Main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CurrentCameraState = CameraState.Main;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CurrentCameraState = CameraState.Banner;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CurrentCameraState = CameraState.Drawer;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CurrentCameraState = CameraState.Map;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            CurrentCameraState = CameraState.Inkwell;
        }
    }

    public void CameraStateAnimations(CameraState inState)
    {
        switch (inState)
        {
            case CameraState.Main:
                m_cameraAnimator.SetTrigger("main");
                break;

            case CameraState.Map:
                m_cameraAnimator.SetTrigger("map");
                break;

            case CameraState.Inkwell:
                m_cameraAnimator.SetTrigger("inkwell");
                break;

            case CameraState.Banner:
                m_cameraAnimator.SetTrigger("banner");
                break;

            case CameraState.Drawer:
                m_cameraAnimator.SetTrigger("drawer");
                break;

            default:
                break;
        }
    }
}