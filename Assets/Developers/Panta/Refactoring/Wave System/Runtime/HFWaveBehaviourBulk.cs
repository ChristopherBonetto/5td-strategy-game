using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.Refactoring
{
    public class HFWaveBehaviourBulk : HFWaveBehaviour
    {
        int m_spawnPointID = 0;
        int m_amountToSpawn = 0;
        int m_spawnedUnitCount = 0;
        float m_timeToWaitBetweenUnits = 0;
        float m_timeElapsed = 0;
        HFBaseStats m_unitStatsData = null;



        public HFWaveBehaviourBulk(int spawnPointID, int amountToSpawn, float waitBetweenUnits, HFBaseStats unitStats) : base()
        {
            m_spawnPointID = spawnPointID;
            m_amountToSpawn = amountToSpawn;
            m_spawnedUnitCount = 0;
            m_unitStatsData = unitStats;
            m_timeToWaitBetweenUnits = waitBetweenUnits;
        }


        public override void Execute(HFWaveController controller)
        {
            if (m_timeElapsed < m_timeToWaitBetweenUnits)
            {
                m_timeElapsed += Time.deltaTime;
            }
            else
            {
                for (int i = 0; i < m_amountToSpawn; i++)
                {
                    // Instatiate unit with the given position and data.

                    m_spawnedUnitCount++;
                }

                // Reset the timer
                m_timeElapsed = 0;
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
