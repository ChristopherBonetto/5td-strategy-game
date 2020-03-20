using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(HFScenesManager.Instance != null)
        {

        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            //HFSavingManager.Save();
            //int a = HFScenesManager.Instance.ReturnIndexLastLevelCompleted();
            //Debug.Log(a);
        }
    }
}
