using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HF.WaveSystem;

[CustomEditor(typeof(HFWaveController))]
[CanEditMultipleObjects]
public class HFWaveControllerEditor : Editor
{
    SerializedProperty m_AIController;

    UnityEditorInternal.ReorderableList waev_list;

    private void OnEnable()
    {
        // Reorderable list creation
        var property = this.serializedObject.FindProperty("m_SpawnPoints");
        this.waev_list = HFReorderableList.CreateAutoLayout(property);

        // other variables creation
        m_AIController = this.serializedObject.FindProperty("Controller");
    }

    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();

        
        EditorGUILayout.Space();
        EditorGUILayout.Space();


        EditorGUILayout.LabelField("", EditorStyles.boldLabel);
        HFReorderableList.DoLayoutListWithFoldout(this.waev_list);


        EditorGUILayout.Space();
        EditorGUILayout.Space();


        // Create objects fields.
        EditorGUILayout.ObjectField(m_AIController);


        this.serializedObject.ApplyModifiedProperties();
    }
}
