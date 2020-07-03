using UnityEngine;

public class Billboard : MonoBehaviour
{
    public bool BillboardX = true;
    public bool BillboardY = true;
    public bool BillboardZ = true;
    public bool DynamicResize = true;
    public float DynamicMultiplier;
    public float OffsetToCamera;
    protected Vector3 localStartPosition;

    // Use this for initialization
    void Start()
    {
        localStartPosition = transform.localPosition;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                                                               Camera.main.transform.rotation * Vector3.up);
        if (!BillboardX || !BillboardY || !BillboardZ)
            transform.rotation = Quaternion.Euler(BillboardX ? transform.rotation.eulerAngles.x : 0f, BillboardY ? transform.rotation.eulerAngles.y : 0f, BillboardZ ? transform.rotation.eulerAngles.z : 0f);
        if (DynamicResize) {
            float size = (Camera.main.transform.position - transform.position).magnitude;
            transform.localScale = new Vector3(size,size,size) * DynamicMultiplier/10000;
        }
        transform.localPosition = localStartPosition;
        transform.position = transform.position + transform.rotation * Vector3.forward * OffsetToCamera;
    }
}