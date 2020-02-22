using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HF.WaveSystem;

public class HFExecuteBehaviourState : HFWaveControllerState
{
    public override void HadnleExitCondition(HFWaveController waveController)
    {
    }

    public override void Update(HFWaveController waveController)
    {
        HFWave.MinorWave mw = waveController.GetCurrentMinorWave;

        if (mw.MinorWaveType == MinorWaveType.Single)
        {
            Vector3 position = waveController.SpawnPoints[mw.SpawnPoint].position;
            waveController.Controller.SpawnUnit(mw.UnitStatsData, position);

            waveController.MinorWaveIndex++;

            // Count ++ of the wave spawned.
        }
        else if (mw.MinorWaveType == MinorWaveType.Wait)
        {
            CheckingTimeElapsed.TimeToElaps = mw.TimeToWait;
            CheckingTimeElapsed.TimeElapsed = 0;

            waveController.MinorWaveIndex++;
        }

        waveController.CurrentState = CheckingInput;
    }
}
