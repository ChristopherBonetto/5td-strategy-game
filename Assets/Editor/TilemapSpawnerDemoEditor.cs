using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TilemapSpawnerDemo))]
public class TilemapSpawnerDemoEditor : Editor
{
    TilemapSpawnerDemo m_target;


    private void OnSceneGUI()
    {
        m_target = (TilemapSpawnerDemo)target;

        Handles.BeginGUI();
        {
            GUIStyle style = new GUIStyle("box");

            GUILayout.BeginArea(new Rect(10, 10, 150, 50), style);
            {
                GUILayout.Space(10);

                if (GUILayout.Button("Generate Map"))
                {
                    m_target.GenerateLevel();
                }
            }
            GUILayout.EndArea();
        }
        Handles.EndGUI();
    }
}
