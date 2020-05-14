using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HF;
using Types;

namespace HF.Refactoring
{
    public enum BehaviourType
    {
        SINGLE,
        WAIT,
        BULK
    }

    [CreateAssetMenu(fileName = "so_Wave_00_Lvl_00", menuName = "Good North/Wave system/New Wave Data")]
    /// <summary>
    /// Scriptable Object that collect behaviours of a single wave.
    /// </summary>
    public class HFWaveData : ScriptableObject
    {
        [SerializeField]
        private List<HFWaveBehaviourData> m_behaviours = new List<HFWaveBehaviourData>(1);


        [System.Serializable]
        /// <summary>
        /// Inner class to define a single behaviour of the wave.
        /// </summary>
        public class HFWaveBehaviourData
        {
            public BehaviourType Type = BehaviourType.SINGLE;
            public UnitType UnitType = UnitType.PEASANT;
            public int SpawnPointID = 0;
            public float TimeToWait = 0;
            public int AmountToSpawn = 0;
        }


        /// <summary>
        /// Get all behaviours of the wave.
        /// </summary>
        public List<HFWaveBehaviourData> GetBehaviours()
        {
            return m_behaviours;
        }


        /// <summary>
        /// Getthe count of enemies in this level.
        /// </summary>
        /// <returns></returns>
        public int GetCountOfEnemies()
        {
            int valueToReturn = 0;

            foreach (var behaviour in m_behaviours)
            {
                valueToReturn += behaviour.AmountToSpawn;
            }

            return valueToReturn;
        }
    }   
}
