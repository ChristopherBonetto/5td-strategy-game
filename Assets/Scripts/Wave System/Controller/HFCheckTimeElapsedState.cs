using HF.WaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFCheckTimeElapsedState : HFWaveControllerState
{
    public float TimeToElaps;
    public float TimeElapsed;

    public override void Update(HFWaveController waveController)
    {
        if (TimeElapsed >= TimeToElaps)
            waveController.CurrentState = ExecutingBehaviour;
        else
            TimeElapsed += Time.deltaTime;
    }
}
