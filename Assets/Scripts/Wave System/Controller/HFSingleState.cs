using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HF.WaveSystem;

public class HFSingleState : HFWaveControllerState
{
    private bool m_isEnemySpawned = false;

    public HFSingleState(HFWaveController waveController)
    {
        m_isEnemySpawned = false;
    }

    public override void HandleEnterCondition(HFWaveController waveController)
    {
        if (waveController.GetCurrentMinorWave.MinorWaveType == MinorWaveType.Single)
        {
            m_isEnemySpawned = false;
        }
    }

    public override void HandleExitCondition(HFWaveController waveController)
    {
        if (m_isEnemySpawned)
        {
            waveController.MinorWaveIndex++;
            HandleEnterCondition(waveController);
        }
    }

    public override void Update(HFWaveController waveController)
    {
        if (!m_isEnemySpawned)
        {
            waveController.Controller.SpawnUnit(waveController.GetCurrentMinorWave.UnitStatsData, waveController.SpawnPoints[waveController.GetCurrentMinorWave.SpawnPoint]);
            m_isEnemySpawned = true;
        }
    }
}
