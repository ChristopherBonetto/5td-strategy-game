using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HF.WaveSystem;

public class HFCheckInputState : HFWaveControllerState
{
    public override void HadnleExitCondition(HFWaveController waveController)
    {
        if (!waveController.WaitForInput)
            waveController.CurrentState = CheckingAvailableMinorWaves;
    }

    public override void Update(HFWaveController waveController)
    {
    }
}