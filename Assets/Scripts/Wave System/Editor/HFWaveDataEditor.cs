using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace HF.Refactoring
{
    [CustomEditor(typeof(HFWaveData))]
    public class HFWaveDataEditor : Editor
    {
        protected SerializedObject m_serializedObject;
        protected SerializedProperty m_serializedProperty;

        private string m_selectedPropertyPath;
        protected SerializedProperty m_selectedProperty;

        Vector2 scrollPos;
        SerializedProperty[] m_properties;

        private void OnEnable()
        {
            m_serializedObject = new SerializedObject(target);
        }

        public override void OnInspectorGUI()
        {
            m_serializedProperty = m_serializedObject.FindProperty("m_behaviours");
            m_properties = new SerializedProperty[m_serializedProperty.arraySize];

            DrawProperties(m_serializedProperty, true);
            m_serializedObject.ApplyModifiedProperties();
        }

        protected void DrawProperties(SerializedProperty prop, bool drawChildren)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            for (int i = 0; i < m_serializedProperty.arraySize; i++)
            {
                EditorGUILayout.Space();

                m_properties[i] = m_serializedProperty.GetArrayElementAtIndex(i);

                SerializedProperty behaviourType = m_properties[i].FindPropertyRelative("Type");
                SerializedProperty unitData = m_properties[i].FindPropertyRelative("UnitData");
                SerializedProperty spawnPointID = m_properties[i].FindPropertyRelative("SpawnPointID");
                SerializedProperty timeToWait = m_properties[i].FindPropertyRelative("TimeToWait");
                SerializedProperty amountToSpawn = m_properties[i].FindPropertyRelative("AmountToSpawn");

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical(GUILayout.MaxWidth(110));
                bool isExpanded = m_properties[i].isExpanded;
                if (GUILayout.Button(((BehaviourType)behaviourType.enumValueIndex).ToString(), GUILayout.MaxWidth(100)))
                {
                    isExpanded = !isExpanded;
                    m_properties[i].isExpanded = isExpanded;
                }
                EditorGUILayout.EndVertical();
                if (GUILayout.Button("^", GUILayout.Width(20)))
                {
                    var index = i;
                    m_serializedProperty.MoveArrayElement(index - 1, index);
                }
                if (GUILayout.Button("v", GUILayout.Width(20)))
                {
                    var index = i;
                    m_serializedProperty.MoveArrayElement(index + 1, index);
                }
                EditorGUILayout.BeginVertical("box");

                if (m_properties[i].isExpanded)
                {
                    switch ((BehaviourType)behaviourType.enumValueIndex)
                    {
                        case BehaviourType.SINGLE:
                            GUI.backgroundColor = Color.cyan;
                            EditorGUILayout.PropertyField(behaviourType, drawChildren);
                            EditorGUILayout.PropertyField(unitData, drawChildren);
                            EditorGUILayout.PropertyField(spawnPointID, drawChildren);
                            amountToSpawn.intValue = 1;
                            break;

                        case BehaviourType.WAIT:
                            GUI.backgroundColor = Color.yellow;
                            EditorGUILayout.PropertyField(behaviourType, drawChildren);
                            EditorGUILayout.PropertyField(timeToWait, drawChildren);
                            break;

                        case BehaviourType.BULK:
                            GUI.backgroundColor = Color.grey;
                            EditorGUILayout.PropertyField(behaviourType, drawChildren);
                            EditorGUILayout.PropertyField(unitData, drawChildren);
                            EditorGUILayout.PropertyField(timeToWait, drawChildren);
                            EditorGUILayout.PropertyField(spawnPointID, drawChildren);
                            EditorGUILayout.PropertyField(amountToSpawn, drawChildren);
                            break;
                    }
                    GUI.backgroundColor = Color.white;
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            Add();
            RemoveLast();
        }

        private void Add()
        {
            if (GUILayout.Button("Add"))
            {
                if (m_serializedProperty == null)
                {
                    List<HFWaveData.HFWaveBehaviourData> datas = new List<HFWaveData.HFWaveBehaviourData>();
                    m_serializedProperty = m_serializedObject.FindProperty("m_behaviours");
                }
                else
                {
                    m_serializedProperty.arraySize++;
                }
            }
        }

        private void RemoveLast()
        {
            if (GUILayout.Button("Remove Last"))
            {
                if (m_serializedProperty.arraySize <= 0)
                {
                    Debug.LogWarning("The size of the araay is 0");
                    return;
                }

                if (m_serializedProperty.arraySize > 0)
                {
                    m_selectedProperty = null;
                    m_serializedProperty.arraySize--;
                }
            }
        }
    }
}
