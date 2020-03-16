using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HF.WaveSystem;

public class HFExecuteBehaviourState : HFWaveControllerState
{
    // Bulk
    private int m_amountOfTroopSpawned;

    public override void Update(HFWaveController waveController)
    {
        HFWave.MinorWave mw = waveController.GetCurrentMinorWave;

        if (mw.MinorWaveType == MinorWaveType.Single)
        {
            waveController.Controller.SpawnUnit(mw.UnitStatsData, waveController.SpawnPoints[mw.SpawnPoint]);

            waveController.MinorWaveIndex++;

            // Count ++ of the wave spawned.
        }
        else if (mw.MinorWaveType == MinorWaveType.Wait)
        {
            CheckingTimeElapsed.TimeToElaps = mw.TimeToWait;
            CheckingTimeElapsed.TimeElapsed = 0;

            waveController.MinorWaveIndex++;
        }
        else if (mw.MinorWaveType == MinorWaveType.Bulk)
        {
            CheckingTimeElapsed.TimeToElaps = mw.TimeToWait;
            CheckingTimeElapsed.TimeElapsed = 0;

            waveController.Controller.SpawnUnit(mw.UnitStatsData, waveController.SpawnPoints[mw.SpawnPoint]);

            m_amountOfTroopSpawned++;
            if (m_amountOfTroopSpawned >= mw.AmountToSpawn)
                waveController.MinorWaveIndex++;
        }

        waveController.CurrentState = CheckingTimeElapsed;
    }
}
