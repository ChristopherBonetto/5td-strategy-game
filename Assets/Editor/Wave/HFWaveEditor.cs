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
    List<HFWave.Behaviour> m_typedList;

    #region Resources
    Texture2D deleteButton;
    #endregion

    void OnEnable()
    {
        HFWave m_target = (HFWave)target;
        m_typedList = m_target.BehavioursCollection;
        m_list = serializedObject.FindProperty("BehavioursCollection");

        // Resources
        deleteButton = Resources.Load<Texture2D>("Editor/custom_editor_button_delete");
    }

    public override void OnInspectorGUI()
    {
        //Update our list
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Display our list to the inspector window
        for (int i = 0; i < m_list.arraySize; i++)
        {
            SerializedProperty list = m_list.GetArrayElementAtIndex(i);
            SerializedProperty foldout = list.FindPropertyRelative("Foldout");
            SerializedProperty waitForInput = list.FindPropertyRelative("WaitForInput");
            SerializedProperty randomEnemy = list.FindPropertyRelative("RandomEnemy");
            SerializedProperty enemyPrefab = list.FindPropertyRelative("EnemyPrefab");
            SerializedProperty spawnPoint = list.FindPropertyRelative("SpawnPoint");
            SerializedProperty timeToWait = list.FindPropertyRelative("TimeToWait");
            SerializedProperty amountToSpawn = list.FindPropertyRelative("AmountToSpawn");


            EditorGUILayout.Space();
            EditorGUILayout.Space();


            EditorGUILayout.BeginHorizontal();
            
            foldout.boolValue = EditorGUILayout.ToggleLeft("Show", foldout.boolValue, GUILayout.Width(100));

            m_typedList[i].BehaviourType = (BehaviourType)EditorGUILayout.EnumPopup(m_typedList[i].BehaviourType);

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

            if (foldout.boolValue)
            {
                if (m_typedList[i].BehaviourType == BehaviourType.Single)
                {
                    EditorGUILayout.Space();


                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.LabelField("Request next behaviour when?");
                    m_typedList[i].RequestType = (RequestType)EditorGUILayout.EnumPopup(m_typedList[i].RequestType);


                    EditorGUILayout.EndHorizontal();


                    EditorGUILayout.BeginHorizontal();

                    if (m_typedList[i].RequestType == RequestType.Post)
                    {
                        EditorGUILayout.LabelField("Wait for player input ?");
                        waitForInput.boolValue = EditorGUILayout.Toggle(waitForInput.boolValue);
                    }

                    EditorGUILayout.EndHorizontal();

                    //randomEnemy.boolValue = EditorGUILayout.Toggle("Pick random enemies ?", randomEnemy.boolValue);

                    if (!randomEnemy.boolValue)
                        EditorGUILayout.ObjectField(enemyPrefab);

                    spawnPoint.intValue = EditorGUILayout.IntField("Spawn Point ID", spawnPoint.intValue);
                }
                else if (m_typedList[i].BehaviourType == BehaviourType.Wait)
                {
                    EditorGUILayout.Space();


                    timeToWait.floatValue = EditorGUILayout.FloatField("Time to wait", timeToWait.floatValue);

                    //if (m_list.arraySize >= i + 1 && m_typedList.Count >= i + 1)
                    //    m_typedList[i].RequestType = m_typedList[i + 1].RequestType;

                    //EditorGUILayout.BeginHorizontal();

                    //EditorGUILayout.LabelField("Request next behaviour when?");
                    //m_typedList[i].RequestType = (RequestType)EditorGUILayout.EnumPopup(m_typedList[i].RequestType);


                    //EditorGUILayout.EndHorizontal();

                    //EditorGUILayout.LabelField("Request type will be: " + m_typedList[i].RequestType);
                }
                //else if (m_typedList[i].BehaviourType == BehaviourType.Bulk)
                //{
                //    EditorGUILayout.Space();


                //    EditorGUILayout.BeginHorizontal();

                //    EditorGUILayout.LabelField("Request next behaviour when?");
                //    m_typedList[i].RequestType = (RequestType)EditorGUILayout.EnumPopup(m_typedList[i].RequestType);

                //    EditorGUILayout.EndHorizontal();

                //    EditorGUILayout.BeginHorizontal();

                //    if (m_typedList[i].RequestType == RequestType.Post)
                //    {
                //        EditorGUILayout.LabelField("Wait for player input ?");
                //        waitForInput.boolValue = EditorGUILayout.Toggle(waitForInput.boolValue);
                //    }

                //    EditorGUILayout.EndHorizontal();

                //    randomEnemy.boolValue = EditorGUILayout.Toggle("Pick random enemies ?", randomEnemy.boolValue);

                //    if (!randomEnemy.boolValue)
                //        EditorGUILayout.ObjectField(enemyPrefab);

                //    amountToSpawn.intValue = EditorGUILayout.IntField("Amount to spawn", amountToSpawn.intValue);
                //    spawnPoint.intValue = EditorGUILayout.IntField("Spawn Point ID", spawnPoint.intValue);
                //}
            }
        }

         EditorGUILayout.Space();
         EditorGUILayout.Space();

        if (GUILayout.Button("Add New Behaviour"))
        {
            m_typedList.Add(new HFWave.Behaviour());
        }

        //Apply changes
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
