using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.WaveSystem
{
    public abstract class HFWaveControllerState
    {
        // Static instance of all states.
        public static HFCheckInputState CheckingInput = new HFCheckInputState();
        public static HFCheckAvailableMinorWaveState CheckingAvailableMinorWaves = new HFCheckAvailableMinorWaveState();
        public static HFCheckTimeElapsedState CheckingTimeElapsed = new HFCheckTimeElapsedState();
        public static HFExecuteBehaviourState ExecutingBehaviour = new HFExecuteBehaviourState();

        abstract public void HadnleExitCondition(HFWaveController waveController);
        abstract public void Update(HFWaveController waveController);
    }
}
