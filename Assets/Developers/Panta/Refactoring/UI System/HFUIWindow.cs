using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.Refactoring
{
    public abstract class HFUIWindow : MonoBehaviour
    {
        public abstract HFUIWindowID ID { get; }

        public virtual void OnShow()
        {
            gameObject.SetActive(true);

            // Start a coroutin with an animation;
        }

        public virtual void OnHide()
        {
            // Start a coroutine with an animation and when end
            // set active to false;

            gameObject.SetActive(false);
        }
    }
}