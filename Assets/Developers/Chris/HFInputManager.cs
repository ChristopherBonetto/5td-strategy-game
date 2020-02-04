using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum MouseIndex
{
    Left = 0,
    Right = 1,
    Center = 2
}

public class HFInputManager : MonoBehaviour
{
    //Custom key to open or close intenvory
    [SerializeField] private KeyCode m_keyToOpenSettings;
    

    private void Update()
    {
        CheckMouseInput();

        
    }


    /// <summary>
    /// Checks for Mouse inputs and invokes associated events.
    /// </summary>
    private void CheckMouseInput()
    {
        #region Left click

        //if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())


        #endregion
    }

}






