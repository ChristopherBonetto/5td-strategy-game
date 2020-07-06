using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFEmitter : MonoBehaviour
{
    [FMODUnity.EventRef]
    public string[] SoundPath;
    private HFCustomEvent[] SoundEvent;

    private void Start()
    {
        SoundEvent = new HFCustomEvent[SoundPath.Length];
    }

    public void PlaySound(int index)
    {
        if (string.IsNullOrEmpty(SoundPath[index]))
            return;
        if (SoundEvent[index] == null)
            SoundEvent[index] = HFSoundManager.Instance.GetFreeEventFromDictionaryKey(SoundPath[index]);
        if (SoundEvent[index] == null) 
            return;
        SoundEvent[index].Play();
    }
}
