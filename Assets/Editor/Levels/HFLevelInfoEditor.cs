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

    }
}
