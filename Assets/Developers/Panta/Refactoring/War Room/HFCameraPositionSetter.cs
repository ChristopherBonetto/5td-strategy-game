using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HF.Refactoring 
{
    public class HFCameraPositionSetter : MonoBehaviour
    {
        [Tooltip("Represents the camera target count when this component is clicked")]
        public int CameraPositionCount;

        private void OnMouseUp() 
        {
            HFCameraWarRoom.Instance.SetPositionCount(CameraPositionCount);
        }
    }
}
