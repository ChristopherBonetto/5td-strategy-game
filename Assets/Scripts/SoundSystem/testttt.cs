using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testttt : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string ciao;

    public HFCustomEvent wow;

    private HFIEvent3D lol;

    // Start is called before the first frame update
    void Start()
    {
        lol = new HFIAttachPlay3D();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            wow = HFSoundManager.Instance.GetFreeEventFromDictionaryKey(ciao);
            lol.AttachAndPlay(this.gameObject, wow);
        }
    }
}
