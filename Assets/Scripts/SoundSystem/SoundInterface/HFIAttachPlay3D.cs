using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFIAttachPlay3D : HFIEvent3D
{
    public void AttachAndPlay(GameObject inObj, HFCustomEvent inEvent)
    {
        Rigidbody tempRb = inObj.GetComponent<Rigidbody>();

        if(tempRb != null)
        {
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(inEvent.EventIstance, inObj.transform, tempRb);
            inEvent.Play();
        }
    }
}
