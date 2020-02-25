using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.WaveSystem
{
    public abstract class HFWaveControllerState
    {
        // Static instance of all states.
        // I don't need to create an instance of those.
        public static HFCheckTimeElapsedState CheckingTimeElapsed = new HFCheckTimeElapsedState();
        public static HFExecuteBehaviourState ExecutingBehaviour = new HFExecuteBehaviourState();

        /// <summary>
        /// Will be executed in update(); (Monobehaviour)
        /// </summary>
        /// <param name="waveController"></param>
        abstract public void Update(HFWaveController waveController);
    }
}
