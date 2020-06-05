using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.Refactoring
{
    public class HFUndoB : HFButton
    {
        public void Undo()
        {
            if (m_isMatchingWindowID)
            {
                HFUIManager.Instance.Undo();
            }
        }
    }
}
