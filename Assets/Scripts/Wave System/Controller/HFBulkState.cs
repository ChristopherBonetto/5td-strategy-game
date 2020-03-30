using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HF.WaveSystem;

public class HFBulkState : HFWaveControllerState
{
    // Bulk
    private int m_amountOfTroopSpawned;
    public float TimeToElaps;
    public float TimeElapsed;

    public HFBulkState(HFWaveController waveController)
    {
        m_amountOfTroopSpawned = 0;
        TimeElapsed = 0;
        TimeToElaps = waveController.GetCurrentMinorWave.TimeToWait;
    }


    public override void HandleEnterPhase(HFWaveController waveController)
    {
        m_amountOfTroopSpawned = 0;
        TimeElapsed = 0;
        TimeToElaps = waveController.GetCurrentMinorWave.TimeToWait;
    }

    public override void HandleExitCondition(HFWaveController waveController)
    {
        if (m_amountOfTroopSpawned >= waveController.GetCurrentMinorWave.AmountToSpawn)
        {
            waveController.MinorWaveIndex++;
            waveController.ChangeState();
        }
    }

    public override void Update(HFWaveController waveController)
    {
        TimeElapsed += Time.deltaTime;

        if (TimeElapsed > TimeToElaps)
        {
            waveController.Controller.SpawnUnit(waveController.GetCurrentMinorWave.UnitStatsData, waveController.SpawnPoints[waveController.GetCurrentMinorWave.SpawnPoint]);
            m_amountOfTroopSpawned++;
            TimeElapsed = 0;
        }
    }

}
