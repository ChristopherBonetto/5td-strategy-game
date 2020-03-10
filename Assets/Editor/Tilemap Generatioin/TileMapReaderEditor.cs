using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class TileMapReaderEditor : EditorWindow
{
    public TileMapReader m_tilemapReader;
    public TileBase m_fillTile;

    [MenuItem("GoodNorth/LevelEditor")]
    public static void CreateWindow()
    {
        GetWindow<TileMapReaderEditor>("LevelEditor");
    }

    private void OnEnable()
    {
        m_tilemapReader = new TileMapReader();
        m_tilemapReader.Grid = SceneView.FindObjectOfType<Grid>();
    }

    private void OnGUI()
    {
        Handles.BeginGUI();
        {
            GUIStyle style = new GUIStyle("box");

            GUILayout.BeginVertical();
            
            GUILayout.Space(20);

		    if (GUILayout.Button("Generate Map"))
            {
                m_tilemapReader.CheckParent();
                m_tilemapReader.SetTileMapContainer();
                m_tilemapReader.SetLayers();
                m_tilemapReader.Spawn3DLevel();
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Destroy Map"))
            {
                m_tilemapReader.DestroyLevelGenerated();
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Push as prefab"))
            {
                m_tilemapReader.PushLevelAsPrefab(m_tilemapReader.m_LevelParent);
            }
            GUILayout.EndVertical();
        }
        Handles.EndGUI();
    }
}
