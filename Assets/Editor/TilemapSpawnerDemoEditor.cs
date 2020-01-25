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
    }

    private void OnGUI()
    {
        m_grid.GridPrefab = EditorGUILayout.ObjectField("Grid", m_grid.GridPrefab, typeof(Grid), true) as Grid;
        m_fillTile = EditorGUILayout.ObjectField("Default TileBase", m_fillTile, typeof(TileBase), true) as TileBase;

        Handles.BeginGUI();
        {
            GUIStyle style = new GUIStyle("box");

            GUILayout.BeginVertical();

			if (GUILayout.Button("Fill Shape"))
			{
				if (m_fillTile)
				{
					m_grid.TilemapPrefab = m_grid.GridPrefab.GetComponentsInChildren<Tilemap>();
					m_grid.Fill(m_fillTile); 
				}
				else
				{
					ShowNotification(new GUIContent("No fill tile has been specified!"));
				}
			}
			else if (GUILayout.Button("Generate Map"))
            {
				if (m_grid.GridPrefab)
				{
					m_grid.TilemapPrefab = m_grid.GridPrefab.GetComponentsInChildren<Tilemap>();
					m_grid.GenerateLevel(m_grid.ParentOfLevelGenerated);
				}
				else
				{
					ShowNotification(new GUIContent("No grid tilemap has been specified!"));
				}
            }
            else if (GUILayout.Button("Push"))
            {
                if (m_grid.GridPrefab)
                {
                    m_grid.PushLevelAsPrefab(m_grid.ParentOfLevelGenerated);
				}
				else
				{
					ShowNotification(new GUIContent("No grid tilemap has been specified!"));
				}
			}
            GUILayout.EndVertical();
        }
        Handles.EndGUI();
    }
}
