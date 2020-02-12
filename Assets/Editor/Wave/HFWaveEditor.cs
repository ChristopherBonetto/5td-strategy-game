using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HF.WaveSystem;

[CustomEditor(typeof(HFWave))]
public class HFWaveEditor : Editor
{
    new HFWave target;
    SerializedObject getTarget;

    SerializedProperty thisList;
    int ListSize;

    List<HFWave.Behaviour> typedList;

    #region Resources
    Texture2D deleteButton;
    #endregion

    void OnEnable()
    {
        target = (HFWave)base.target;
        getTarget = new SerializedObject(target);

        thisList = getTarget.FindProperty("BehavioursCollection");  // Find the List in our script and create a refrence of it
        typedList = target.BehavioursCollection;                          // Find the List in our script and create a refrence of it with the same type.

        // Resources
        deleteButton = Resources.Load<Texture2D>("Editor/custom_editor_button_delete");
    }

    public override void OnInspectorGUI()
    {
        //Update our list
        getTarget.Update();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Display our list to the inspector window
        for (int i = 0; i < thisList.arraySize; i++)
        {
            // Store properties.
            SerializedProperty MyListRef = thisList.GetArrayElementAtIndex(i);
            SerializedProperty MyFoldout = MyListRef.FindPropertyRelative("Foldout");
            SerializedProperty MyRandomEmemy = MyListRef.FindPropertyRelative("RandomEnemy");
            SerializedProperty MyEnemyPrefab = MyListRef.FindPropertyRelative("EnemyPrefab");
            SerializedProperty MySpawnPoint = MyListRef.FindPropertyRelative("SpawnPoint");
            SerializedProperty MyTimeToWait = MyListRef.FindPropertyRelative("TimeToWait");
            SerializedProperty MyAmountToSpawn = MyListRef.FindPropertyRelative("AmountToSpawn");


            if (typedList[i].Type == BehaviourType.Single)
            {
                GUILayout.BeginHorizontal();

                // Foldout 
                MyFoldout.boolValue = EditorGUILayout.Foldout(MyFoldout.boolValue, i.ToString());

                // Assign Behaviour type (enum)
                typedList[i].Type = (BehaviourType)EditorGUILayout.EnumPopup(typedList[i].Type);

                // Buttons: Moves up, moves down, delete.
                MoveUpTheElement(i);

                MoveDownTheElement(i);

                if (GUILayout.Button(deleteButton))
                {
                    thisList.DeleteArrayElementAtIndex(i);
                    getTarget.ApplyModifiedProperties();
                    return;
                }

                GUILayout.EndHorizontal();


                if (MyFoldout.boolValue)
                {
                    MyRandomEmemy.boolValue = EditorGUILayout.Toggle("Random Enemy", MyRandomEmemy.boolValue);    // Update soon...

                    if (!MyRandomEmemy.boolValue)   //  Drag and drop a prefab
                        MyEnemyPrefab.objectReferenceValue = EditorGUILayout.ObjectField("My Enemy Prefab", MyEnemyPrefab.objectReferenceValue, typeof(GameObject), true);
                    // else                        
                                                    // else choose randomly (weighted) the prefab from a collection

                    MySpawnPoint.intValue = EditorGUILayout.IntField("Spawn Point ID", MySpawnPoint.intValue);
                }

                // Add some space.
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
            else if (typedList[i].Type == BehaviourType.Wait)
            {
                GUILayout.BeginHorizontal();

                // Foldout
                MyFoldout.boolValue = EditorGUILayout.Foldout(MyFoldout.boolValue, i.ToString());

                // Assign Behaviour type (enum)
                typedList[i].Type = (BehaviourType)EditorGUILayout.EnumPopup(typedList[i].Type);

                // Buttons: Moves up, moves down, delete.
                MoveUpTheElement(i);

                MoveDownTheElement(i);

                if (GUILayout.Button(deleteButton))
                {
                    thisList.DeleteArrayElementAtIndex(i);
                    getTarget.ApplyModifiedProperties();
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
            else if (typedList[i].Type == BehaviourType.Bulk)
            {
                GUILayout.BeginHorizontal();

                // Foldout
                MyFoldout.boolValue = EditorGUILayout.Foldout(MyFoldout.boolValue, i.ToString());

                // Assign Behaviour type (enum)
                typedList[i].Type = (BehaviourType)EditorGUILayout.EnumPopup(typedList[i].Type);

                // Buttons: Moves up, moves down, delete.
                MoveUpTheElement(i);

                MoveDownTheElement(i);

                if (GUILayout.Button(deleteButton))
                {
                    thisList.DeleteArrayElementAtIndex(i);
                    getTarget.ApplyModifiedProperties();
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
            target.BehavioursCollection.Add(new HFWave.Behaviour());
        }

        //Apply the changes to our list
        getTarget.ApplyModifiedProperties();
    }

    #region List Control
    private void MoveUpTheElement(int i)
    {
        if (GUILayout.Button("^"))
        {
            thisList.MoveArrayElement(i, i - 1);
            getTarget.ApplyModifiedProperties();
            return;
        }
    }

    private void MoveDownTheElement(int i)
    {
        if (GUILayout.Button("v"))
        {
            thisList.MoveArrayElement(i, i + 1);
            getTarget.ApplyModifiedProperties();
            return;
        }
    }
    #endregion
}
