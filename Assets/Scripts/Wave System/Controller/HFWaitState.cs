using HF.WaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFWaitState : HFWaveControllerState
{
    public float TimeToElaps;
    public float TimeElapsed;

    public HFWaitState(HFWaveController waveController)
    {
        TimeElapsed = 0;
        TimeToElaps = waveController.GetCurrentMinorWave.TimeToWait;
    }

    public override void HandleEnterPhase(HFWaveController waveController)
    {
        TimeElapsed = 0;
        TimeToElaps = waveController.GetCurrentMinorWave.TimeToWait;
    }

    public override void HandleExitCondition(HFWaveController waveController)
    {
        if (TimeElapsed > TimeToElaps)
        {
            waveController.MinorWaveIndex++;
            waveController.ChangeState();
        }
    }

    public override void Update(HFWaveController waveController)
    {
        TimeElapsed += Time.deltaTime;
    }
}
