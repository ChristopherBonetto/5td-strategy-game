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
        else if (mw.MinorWaveType == MinorWaveType.Bulk)
        {
            CheckingTimeElapsed.TimeToElaps = mw.TimeToWait;
            CheckingTimeElapsed.TimeElapsed = 0;

            Vector3 position = waveController.SpawnPoints[mw.SpawnPoint].position;
            waveController.Controller.SpawnUnit(mw.UnitStatsData, position);

            m_amountOfTroopSpawned++;
            if (m_amountOfTroopSpawned >= mw.AmountToSpawn)
                waveController.MinorWaveIndex++;
        }

        waveController.CurrentState = CheckingTimeElapsed;
    }
}
