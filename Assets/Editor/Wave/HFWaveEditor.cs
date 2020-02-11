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
            SerializedProperty MyListRef = ThisList.GetArrayElementAtIndex(i);
            SerializedProperty MyRandomEmemy = MyListRef.FindPropertyRelative("RandomEnemy");
            SerializedProperty MyEnemyPrefab = MyListRef.FindPropertyRelative("EnemyPrefab");
            SerializedProperty MySpawnPoint = MyListRef.FindPropertyRelative("SpawnPoint");
            SerializedProperty MyTimeToWait = MyListRef.FindPropertyRelative("TimeToWait");


            if (list[i].Type == BehaviourType.Single)
            {
                GUILayout.BeginHorizontal();

                t.BehavioursCollection[i].Type = (BehaviourType)EditorGUILayout.EnumPopup(list[i].Type);

                if (GUILayout.Button("v"))
                {
                    ThisList.MoveArrayElement(i, i + 1);
                    GetTarget.ApplyModifiedProperties();
                    return;
                }
                else if (GUILayout.Button("^"))
                {
                    ThisList.MoveArrayElement(i, i - 1);
                    GetTarget.ApplyModifiedProperties();
                    return;
                }

                else if (GUILayout.Button("Delete minor wave"))
                {
                    ThisList.DeleteArrayElementAtIndex(i);
                    GetTarget.ApplyModifiedProperties();
                    return;
                }
                GUILayout.EndHorizontal();


                //MyRandomEmemy.boolValue = EditorGUILayout.Toggle("Random Enemy", MyRandomEmemy.boolValue);    // Update soon...

                MyEnemyPrefab.objectReferenceValue = EditorGUILayout.ObjectField("My Enemy Prefab", MyEnemyPrefab.objectReferenceValue, typeof(GameObject), true);
                MySpawnPoint.intValue = EditorGUILayout.IntField("Spawn Point ID", MySpawnPoint.intValue);

                // Add some space.
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
            else if (list[i].Type == BehaviourType.Wait)
            {
                GUILayout.BeginHorizontal();

                list[i].Type = (BehaviourType)EditorGUILayout.EnumPopup(list[i].Type);


                if (GUILayout.Button("v"))
                {
                    ThisList.MoveArrayElement(i, i + 1);
                    GetTarget.ApplyModifiedProperties();
                    return;
                }
                else if (GUILayout.Button("^"))
                {
                    ThisList.MoveArrayElement(i, i - 1);
                    GetTarget.ApplyModifiedProperties();
                    return;
                }

                else if (GUILayout.Button("Delete minor wave"))
                {
                    ThisList.DeleteArrayElementAtIndex(i);
                    GetTarget.ApplyModifiedProperties();
                    return;
                }
                GUILayout.EndHorizontal();

                MyTimeToWait.floatValue = EditorGUILayout.FloatField("Time to wait", MyTimeToWait.floatValue);

                // Add some space.
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
        }

         EditorGUILayout.Space();
         EditorGUILayout.Space();

        //Or add a new item to the List<> with a button
        EditorGUILayout.LabelField("Add a new item with a button");

        if (GUILayout.Button("Add New Behaviour"))
        {
            t.BehavioursCollection.Add(new HFWave.Behaviour());
        }

        //Apply the changes to our list
        GetTarget.ApplyModifiedProperties();
    }
}
