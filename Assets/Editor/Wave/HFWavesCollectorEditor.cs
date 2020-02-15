using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HF.WaveSystem;

[CustomEditor(typeof(HFWavesCollector))]
[CanEditMultipleObjects]
public class HFWavesCollectorEditor : Editor
{
    UnityEditorInternal.ReorderableList list1;

    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", EditorStyles.boldLabel);
        HFReorderableList.DoLayoutListWithFoldout(this.list1);

        this.serializedObject.ApplyModifiedProperties();
    }

    private void OnEnable()
    {
        var property = this.serializedObject.FindProperty("m_WavesCollection");

        this.list1 = HFReorderableList.CreateAutoLayout(property);
    }
}
