using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HFPoolManager))]
public class HFObjectPoolerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open"))
        {
            HFObjectPoolerEditorWindow.Open((HFPoolManager)target);
        }
    }
}
