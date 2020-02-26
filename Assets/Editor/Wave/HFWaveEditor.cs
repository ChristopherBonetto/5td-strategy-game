using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HF.WaveSystem;

[CustomEditor(typeof(HFWave))]
public class HFWaveEditor : Editor
{
    HFWave m_target;
    SerializedProperty m_list;
    List<HFWave.MinorWave> m_typedList;

    // Resources
    Texture2D deleteButton;

    void OnEnable()
    {
        HFWave m_target = (HFWave)target;
        if (m_target.MinorWavesCollection == null)
            m_target.MinorWavesCollection = new List<HFWave.MinorWave>();
        m_typedList = m_target.MinorWavesCollection;
        m_list = serializedObject.FindProperty("MinorWavesCollection");

        // Resources
        deleteButton = Resources.Load<Texture2D>("Editor/custom_editor_button_delete");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();


        EditorGUILayout.Space();
        EditorGUILayout.Space();


        //Display our list to the inspector window
        for (int i = 0; i < m_list.arraySize; i++)
        {
            // Find list
            SerializedProperty list = m_list.GetArrayElementAtIndex(i);
            // Find properties
            SerializedProperty foldout = list.FindPropertyRelative("Foldout");
            SerializedProperty unitStatsData = list.FindPropertyRelative("UnitStatsData");
            SerializedProperty spawnPoint = list.FindPropertyRelative("SpawnPoint");
            SerializedProperty timeToWait = list.FindPropertyRelative("TimeToWait");
            SerializedProperty amountToSpawn = list.FindPropertyRelative("AmountToSpawn");


            // Add some space
            EditorGUILayout.Space();
            EditorGUILayout.Space();


            EditorGUILayout.BeginHorizontal();
            // Foldout?
            foldout.boolValue = EditorGUILayout.ToggleLeft("Show", foldout.boolValue, GUILayout.Width(100));
            // Get typed list
            m_typedList[i].MinorWaveType = (MinorWaveType)EditorGUILayout.EnumPopup(m_typedList[i].MinorWaveType);

            #region Buttons
            MoveDownTheElement(i);
            MoveUpTheElement(i);

            if (GUILayout.Button("Remove"))
            {
                m_list.DeleteArrayElementAtIndex(i);
                serializedObject.ApplyModifiedProperties();
                return;
            }
            #endregion

            EditorGUILayout.EndHorizontal();

            // If it's foldout
            if (foldout.boolValue)
            {
                // Fill "single" behaviour case
                if (m_typedList[i].MinorWaveType == MinorWaveType.Single)
                {
                    EditorGUILayout.Space();;

                    unitStatsData.objectReferenceValue = EditorGUILayout.ObjectField("unit stats data", unitStatsData.objectReferenceValue, typeof(HFBaseStats));
                    spawnPoint.intValue = EditorGUILayout.IntField("Spawn Point ID", spawnPoint.intValue);
                }
                // Fill "wait" behaviour case
                else if (m_typedList[i].MinorWaveType == MinorWaveType.Wait)
                {
                    EditorGUILayout.Space();

                    timeToWait.floatValue = EditorGUILayout.FloatField("Time to wait", timeToWait.floatValue);
                }
            }
        }


         EditorGUILayout.Space();
         EditorGUILayout.Space();


        if (GUILayout.Button("Add New Behaviour"))
        {
            m_typedList.Add(new HFWave.MinorWave());
        }

        serializedObject.ApplyModifiedProperties();
    }

    #region List Control
    private void MoveUpTheElement(int i)
    {
        if (GUILayout.Button("^"))
        {
            m_list.MoveArrayElement(i, i - 1);
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void MoveDownTheElement(int i)
    {
        if (GUILayout.Button("v"))
        {
            m_list.MoveArrayElement(i, i + 1);
            serializedObject.ApplyModifiedProperties();
        }
    }
    #endregion
}
