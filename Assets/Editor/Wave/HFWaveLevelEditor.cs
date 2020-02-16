using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HF.WaveSystem;

[CustomEditor(typeof(HFWaveLevel))]
[CanEditMultipleObjects]
public class HFWaveLevelEditor : Editor
{
    SerializedProperty collector;
    SerializedProperty controller;
    SerializedProperty button;
    UnityEditorInternal.ReorderableList list1;

    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("", EditorStyles.boldLabel);
        HFReorderableList.DoLayoutListWithFoldout(this.list1);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.ObjectField(collector);
        EditorGUILayout.ObjectField(controller);
        EditorGUILayout.ObjectField(button);



        this.serializedObject.ApplyModifiedProperties();
    }

    private void OnEnable()
    {
        var property = this.serializedObject.FindProperty("m_SpawnPoints");
        this.list1 = HFReorderableList.CreateAutoLayout(property);

        collector = this.serializedObject.FindProperty("m_WaveCollector");
        controller = this.serializedObject.FindProperty("m_Controller");
        button = this.serializedObject.FindProperty("m_NextWavebutton");
    }
}
