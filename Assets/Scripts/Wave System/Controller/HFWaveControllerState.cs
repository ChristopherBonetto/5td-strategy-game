using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.WaveSystem
{
    public abstract class HFWaveControllerState
    {
        public abstract void HandleEnterCondition(HFWaveController waveController);
        public abstract void HandleExitCondition(HFWaveController waveController);

        /// <summary>
        /// Will be executed in update(); (Monobehaviour)
        /// </summary>
        public abstract void Update(HFWaveController waveController);
    }
}
