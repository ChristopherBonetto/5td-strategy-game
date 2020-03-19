using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float panSpeed = 20f;
    public float panBoarderThickness = 10f;
    public Vector2 panLimit;
    public float minFOW;
    public float maxFOW;
    public float scrollSpeed;
    private Camera cam;
    public float cameraRotationSpeed;
    public GameObject map;


    void Start()
    {
        cam = GetComponent<Camera>();
    }


    void Update()
    {
        Vector3 pos = transform.position;

        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - panBoarderThickness)
        {
            pos.z += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= panBoarderThickness)
        {
            pos.z -= panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - panBoarderThickness)
        {
            pos.x += panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= panBoarderThickness)
        {
            pos.x -= panSpeed * Time.deltaTime;
        }

        float mouseX = Input.GetAxis("Mouse X");

        if (Input.GetMouseButton(1))
        {
            transform.Rotate(0, 0, mouseX * cameraRotationSpeed);
            //map.transform.Rotate(0, mouseX * cameraRotationSpeed,0);
        }


        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cam.fieldOfView = cam.fieldOfView -= scroll * scrollSpeed;
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOW, maxFOW);

        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.z = Mathf.Clamp(pos.z, -panLimit.y, panLimit.y);
        transform.position = pos;
    }
}
