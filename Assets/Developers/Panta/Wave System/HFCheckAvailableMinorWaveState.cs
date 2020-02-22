using HF.WaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFCheckAvailableMinorWaveState : HFWaveControllerState
{
    public override void HadnleExitCondition(HFWaveController waveController)
    {
        if (waveController.MinorWaveIndex <= waveController.GetMinorWaves.Count - 1)
            waveController.CurrentState = CheckingTimeElapsed;
    }

    public override void Update(HFWaveController waveController)
    {
        // Do nothing
    }
}
