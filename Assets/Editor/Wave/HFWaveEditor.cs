using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HF.WaveSystem;

[CustomEditor(typeof(HFWave))]
public class HFWaveEditor : Editor
{
    BehaviourType DisplayFieldType;

    HFWave t;
    SerializedObject GetTarget;
    SerializedProperty ThisList;
    int ListSize;
    List<HFWave.Behaviour> list;

    void OnEnable()
    {
        t = (HFWave)target;
        GetTarget = new SerializedObject(t);
        ThisList = GetTarget.FindProperty("BehavioursCollection"); // Find the List in our script and create a refrence of it
        list = t.BehavioursCollection;
    }

    public override void OnInspectorGUI()
    {
        //Update our list
        GetTarget.Update();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Display our list to the inspector window
        for (int i = 0; i < ThisList.arraySize; i++)
        {
            // Store properties.
            SerializedProperty MyListRef = ThisList.GetArrayElementAtIndex(i);
            SerializedProperty MyFoldout = MyListRef.FindPropertyRelative("Foldout");
            SerializedProperty MyRandomEmemy = MyListRef.FindPropertyRelative("RandomEnemy");
            SerializedProperty MyEnemyPrefab = MyListRef.FindPropertyRelative("EnemyPrefab");
            SerializedProperty MySpawnPoint = MyListRef.FindPropertyRelative("SpawnPoint");
            SerializedProperty MyTimeToWait = MyListRef.FindPropertyRelative("TimeToWait");
            SerializedProperty MyAmountToSpawn = MyListRef.FindPropertyRelative("AmountToSpawn");


            if (list[i].Type == BehaviourType.Single)
            {
                GUILayout.BeginHorizontal();
                MyFoldout.boolValue = EditorGUILayout.Foldout(MyFoldout.boolValue, i.ToString());
                t.BehavioursCollection[i].Type = (BehaviourType)EditorGUILayout.EnumPopup(list[i].Type);

                // Buttons
                MoveUpTheElement(i);
                MoveDownTheElement(i);
                if (GUILayout.Button("Delete"))
                {
                    ThisList.DeleteArrayElementAtIndex(i);
                    GetTarget.ApplyModifiedProperties();
                    return;
                }
                GUILayout.EndHorizontal();


                if (MyFoldout.boolValue)
                {
                    MyRandomEmemy.boolValue = EditorGUILayout.Toggle("Random Enemy", MyRandomEmemy.boolValue);    // Update soon...
                    if (!MyRandomEmemy.boolValue)
                        MyEnemyPrefab.objectReferenceValue = EditorGUILayout.ObjectField("My Enemy Prefab", MyEnemyPrefab.objectReferenceValue, typeof(GameObject), true);
                    MySpawnPoint.intValue = EditorGUILayout.IntField("Spawn Point ID", MySpawnPoint.intValue);
                }

                // Add some space.
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
            else if (list[i].Type == BehaviourType.Wait)
            {
                GUILayout.BeginHorizontal();
                MyFoldout.boolValue = EditorGUILayout.Foldout(MyFoldout.boolValue, i.ToString());
                list[i].Type = (BehaviourType)EditorGUILayout.EnumPopup(list[i].Type);

                // Buttons
                MoveUpTheElement(i);
                MoveDownTheElement(i);
                if (GUILayout.Button("Delete"))
                {
                    ThisList.DeleteArrayElementAtIndex(i);
                    GetTarget.ApplyModifiedProperties();
                    return;
                }
                GUILayout.EndHorizontal();


                if (MyFoldout.boolValue)
                {
                    MyTimeToWait.floatValue = EditorGUILayout.FloatField("Time to wait", MyTimeToWait.floatValue);
                }

                // Add some space.
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
            else if (list[i].Type == BehaviourType.Bulk)
            {
                GUILayout.BeginHorizontal();
                MyFoldout.boolValue = EditorGUILayout.Foldout(MyFoldout.boolValue, i.ToString());
                t.BehavioursCollection[i].Type = (BehaviourType)EditorGUILayout.EnumPopup(list[i].Type);

                // Buttons
                MoveUpTheElement(i);
                MoveDownTheElement(i);
                if (GUILayout.Button("Delete"))
                {
                    ThisList.DeleteArrayElementAtIndex(i);
                    GetTarget.ApplyModifiedProperties();
                    return;
                }
                GUILayout.EndHorizontal();

                
                if (MyFoldout.boolValue)
                {
                    MyRandomEmemy.boolValue = EditorGUILayout.Toggle("Random Enemy", MyRandomEmemy.boolValue);    // Update soon...
                    if (!MyRandomEmemy.boolValue)
                        MyEnemyPrefab.objectReferenceValue = EditorGUILayout.ObjectField("My Enemy Prefab", MyEnemyPrefab.objectReferenceValue, typeof(GameObject), true);
                    MySpawnPoint.intValue = EditorGUILayout.IntField("Spawn Point ID", MySpawnPoint.intValue);
                    MyAmountToSpawn.intValue = EditorGUILayout.IntField("Amount to spawn", MySpawnPoint.intValue);
                }

                // Add some space.
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
        }

         EditorGUILayout.Space();
         EditorGUILayout.Space();

        if (GUILayout.Button("Add New Behaviour"))
        {
            t.BehavioursCollection.Add(new HFWave.Behaviour());
        }

        //Apply the changes to our list
        GetTarget.ApplyModifiedProperties();
    }

    #region List Control
    private void MoveUpTheElement(int i)
    {
        if (GUILayout.Button("^"))
        {
            ThisList.MoveArrayElement(i, i - 1);
            GetTarget.ApplyModifiedProperties();
            return;
        }
    }

    private void MoveDownTheElement(int i)
    {
        if (GUILayout.Button("v"))
        {
            ThisList.MoveArrayElement(i, i + 1);
            GetTarget.ApplyModifiedProperties();
            return;
        }
    }
    #endregion
}
