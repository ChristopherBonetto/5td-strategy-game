using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.Refactoring
{
    public abstract class HFWaveBehaviour
    {
        public HFWaveBehaviour() { }


        /// <summary>
        /// Execute the behaviour
        /// </summary>
        public abstract void Execute(HFWaveController controller);


        /// <summary>
        /// Exit the behaviour and assign the next one.
        /// </summary>
        public abstract void Exit(HFWaveController controller);
    }
}
