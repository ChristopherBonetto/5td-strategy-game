using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.Refactoring
{
    public class HFWaveBehaviourWait : HFWaveBehaviour
    {
        float m_timeElapsed = 0;
        float m_timeToWait = 0;


        public HFWaveBehaviourWait(float timeToWait) : base()
        {
            m_timeElapsed = 0;
            m_timeToWait = timeToWait;
        }

        public override void Execute(HFWaveController controller)
        {
            if (m_timeElapsed < m_timeToWait)
            {
                m_timeElapsed += Time.deltaTime;
            }
        }

        public override void Exit(HFWaveController controller)
        {
            if (m_timeElapsed >= m_timeToWait)
            {
                controller.SetCurrentBehaviourToPerform();
            }
        }
    }
}
