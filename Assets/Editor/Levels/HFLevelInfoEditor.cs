using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HFLevelInfoSO))]
public class HFLevelInfoEditor : Editor
{

    public override void OnInspectorGUI()
    {
        HFLevelInfoSO myTarget = (HFLevelInfoSO)target;

        base.OnInspectorGUI();

        //if(myTarget.LevelScene != null)
        //{
        //    //myTarget.LevelSceneName = myTarget.LevelScene.name;

        //    myTarget.TakeLevelIndex();
        //}
        //else
        //{
        //    Debug.Log(myTarget.name + " need to assign a scene");
        //}
        
    }
}
