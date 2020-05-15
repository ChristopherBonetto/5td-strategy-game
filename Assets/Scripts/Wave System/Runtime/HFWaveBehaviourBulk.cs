using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;

namespace HF.Refactoring
{
    public class HFWaveBehaviourBulk : HFWaveBehaviour
    {
        int m_spawnPointID = 0;
        int m_amountToSpawn = 0;
        int m_spawnedUnitCount = 0;
        float m_timeToWaitBetweenUnits = 0;
        float m_timeElapsed = 0;
        UnitType m_unitType = UnitType.PEASANT;



        public HFWaveBehaviourBulk(int spawnPointID, int amountToSpawn, float waitBetweenUnits, UnitType unitType) : base()
        {
            m_spawnPointID = spawnPointID;
            m_amountToSpawn = amountToSpawn;
            m_spawnedUnitCount = 0;
            m_unitType = unitType;
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
                // Instatiate unit with the given position and data.
                Troop troop = GameController.Instance.CreateNewTroop(m_unitType, PlayerType.AI, controller.SpawnPoints[m_spawnPointID].SpawnPosition);

                // Increment index
                m_spawnedUnitCount++;

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
