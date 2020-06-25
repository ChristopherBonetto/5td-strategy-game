using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;

namespace HF.Refactoring
{
    public class HFWaveBehaviourSingle : HFWaveBehaviour
    {
        int m_spawnPointID = 0;
        int m_amountToSpawn = 0;
        int m_spawnedUnitCount = 0;
        UnitType m_unitType = UnitType.STANDARD_ALLY;



        public HFWaveBehaviourSingle(int spawnPointID, int amountToSpawn, UnitType unitType) : base()
        {
            m_spawnPointID = spawnPointID;
            m_amountToSpawn = amountToSpawn;
            m_spawnedUnitCount = 0;
            m_unitType = unitType;
        }

        public override void Execute(HFWaveController controller)
        {
            for (int i = 0; i < m_amountToSpawn; i++)
            {
                // Instantitate prefab at the given position with the given data.
                Troop troop = GameController.Instance.CreateNewTroop(m_unitType, PlayerType.AI, controller.SpawnPoints[m_spawnPointID].SpawnPosition,false);
                troop.SetTargetCastle(controller.SpawnPoints[m_spawnPointID].TargetCastle, controller.SpawnPoints[m_spawnPointID].EngagePoint.position.SnapLocation());
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
