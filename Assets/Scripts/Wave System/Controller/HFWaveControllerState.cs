using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.WaveSystem
{
    public abstract class HFWaveControllerState
    {
        /// <summary>
        /// Called one time after entering in a new state.
        /// </summary>
        /// <param name="waveController"></param>
        public abstract void HandleEnterPhase(HFWaveController waveController);

        /// <summary>
        /// called every frame to check the exit condition of the current state.
        /// </summary>
        /// <param name="waveController"></param>
        public abstract void HandleExitCondition(HFWaveController waveController);

        /// <summary>
        /// Will be executed in update(); (Monobehaviour)
        /// </summary>
        public abstract void Update(HFWaveController waveController);
    }
}
