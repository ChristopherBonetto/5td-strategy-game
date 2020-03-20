using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.WaveSystem
{
    public abstract class HFWaveControllerState
    {
        //---------------------------------------------
        // Static instance of all states.
        // I don't need to create an instance of those.
        //---------------------------------------------

        private static HFCheckTimeElapsedState m_CheckingTimeElapsed;
        public static HFCheckTimeElapsedState CheckingTimeElapsed
        { 
            get
            {
                if (m_CheckingTimeElapsed == null)
                    m_CheckingTimeElapsed = new HFCheckTimeElapsedState();
                return m_CheckingTimeElapsed;
            }
        }

        private static HFExecuteBehaviourState m_ExecutingBehaviour;
        public static HFExecuteBehaviourState ExecutingBehaviour
        {
            get
            {
                if (m_ExecutingBehaviour == null)
                    m_ExecutingBehaviour = new HFExecuteBehaviourState();
                return m_ExecutingBehaviour;
            }
        }

        /// <summary>
        /// Will be executed in update(); (Monobehaviour)
        /// </summary>
        /// <param name="waveController"></param>
        public abstract void Update(HFWaveController waveController);
    }
}
