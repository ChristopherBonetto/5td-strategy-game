using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(HFScenesManager),true)]
public class HFScenesManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        HFScenesManager myTarget = (HFScenesManager)target;

        base.OnInspectorGUI();

        myTarget.TakeAllSceneInBuild();

        if(myTarget.LevelContainer != null)
        {
            myTarget.GiveIndexToAllLevels();
        }
        else
        {
            Debug.Log("There is no Level Container to read all levels");
        }
        
    }
}

#endif
