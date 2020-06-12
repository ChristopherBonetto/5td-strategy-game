using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFIAttachPlay3D : HFIEvent3D
{
    public void AttachAndPlay(HFCustomEvent inEvent, GameObject inObj)
    {
        inEvent.PlayOneShootAttached(inObj);
    }
}
