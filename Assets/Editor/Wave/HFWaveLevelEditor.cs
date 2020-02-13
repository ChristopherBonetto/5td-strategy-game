using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HF.WaveSystem;

[CustomEditor(typeof(HFWaveLevel))]
[CanEditMultipleObjects]
public class HFWaveLevelEditor : Editor
{
    HFWaveLevel m_target;

    SerializedObject m_getTarget;
    SerializedProperty m_list;

    UnityEditorInternal.ReorderableList m_outputList;

    private void OnEnable()
    {
        m_target = target as HFWaveLevel;
        m_getTarget = new SerializedObject(m_target);
        m_list = m_getTarget.FindProperty("m_WaveCollection");


        m_outputList = HFReorderableList.CreateAutoLayout(m_list,
            new string[] { "Prefab" });
    }

    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Waves collection", EditorStyles.boldLabel);
        m_outputList.DoLayoutList();

        this.serializedObject.ApplyModifiedProperties();
    }
}
