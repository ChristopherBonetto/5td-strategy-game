using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class TilemapSpawnerDemoEditor : EditorWindow
{
    public TilemapSpawnerDemo m_grid;
    public TileBase m_fillTile;

    [MenuItem("GoodNorth/LevelEditor")]
    public static void CreateWindow()
    {
        GetWindow<TilemapSpawnerDemoEditor>("LevelEditor");
    }

    private void OnEnable()
    {
        m_grid = new TilemapSpawnerDemo();
        m_grid.GridPrefab = FindObjectOfType<Grid>();
    }

    private void OnGUI()
    {
        m_grid.GridPrefab = EditorGUILayout.ObjectField(m_grid.GridPrefab, typeof(Grid), true) as Grid;
        m_fillTile = EditorGUILayout.ObjectField(m_fillTile, typeof(TileBase), true) as TileBase;

        Handles.BeginGUI();
        {
            GUIStyle style = new GUIStyle("box");

            GUILayout.BeginVertical();

            if (GUILayout.Button("Generate Map"))
            {
                m_grid.TilemapPrefab = m_grid.GridPrefab.GetComponentsInChildren<Tilemap>();
                m_grid.Fill(m_fillTile);
                m_grid.GenerateLevel(m_grid.ParentOfLevelGenerated);
            }
            else if (GUILayout.Button("Push"))
            {
                if (m_grid.GridPrefab != null)
                {
                    m_grid.PushLevelAsPrefab(m_grid.ParentOfLevelGenerated);
                }
            }
            GUILayout.EndVertical();
        }
        Handles.EndGUI();
    }
}
