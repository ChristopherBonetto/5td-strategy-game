using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.Refactoring
{
    public class HFWaveBehaviourSingle : HFWaveBehaviour
    {
        int m_spawnPointID = 0;
        int m_amountToSpawn = 0;
        int m_spawnedUnitCount = 0;
        HFBaseStats m_unitStatsData = null;



        public HFWaveBehaviourSingle(int spawnPointID, int amountToSpawn, HFBaseStats unitStats) : base()
        {
            m_spawnPointID = spawnPointID;
            m_amountToSpawn = amountToSpawn;
            m_spawnedUnitCount = 0;
            m_unitStatsData = unitStats;
        }

        public override void Execute(HFWaveController controller)
        {
            for (int i = 0; i < m_amountToSpawn; i++)
            {
                // Instantitate prefab at the given position with the given data.

                m_spawnedUnitCount++;
            }
        }

        public override void Exit(HFWaveController controller)
        {
            if (m_spawnedUnitCount >= m_amountToSpawn)
            {
                controller.SetCurrentBehaviourToPerform();
            }
        }
    }
}
