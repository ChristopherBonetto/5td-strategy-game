using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HFScenesManager))]
public class HFScenesManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        HFScenesManager myTarget = (HFScenesManager)target;

        base.OnInspectorGUI();

        myTarget.TakeAllSceneInBuild();

        myTarget.GiveIndexToAllLevels();
    }
}
