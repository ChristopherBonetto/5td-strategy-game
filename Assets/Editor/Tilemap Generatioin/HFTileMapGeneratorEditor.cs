using UnityEngine;
using UnityEditor;

public class HFTileMapGeneratorEditor : EditorWindow
{
    public HFTileMapGenerator m_tilemapReader;

    [MenuItem("HF/Tile map editor")]
    public static void CreateWindow()
    {
        GetWindow<HFTileMapGeneratorEditor>("Tilemap Editor");
    }

    private void OnEnable()
    {
        m_tilemapReader = new HFTileMapGenerator();
        m_tilemapReader.Grid = SceneView.FindObjectOfType<Grid>();

        if (m_tilemapReader.Grid == null)
        {
            Debug.LogWarning("You are in the wrong scene or the Tilemap is not set in the scene");
            Close();
        }
    }

    private void OnGUI()
    {
        Handles.BeginGUI();
        {
            GUI.BeginGroup(new Rect(20, 20, 200, 150));
                GUILayout.BeginVertical();

                    GUI.backgroundColor = Color.gray;
                    if (GUILayout.Button("Generate Map", GUILayout.Width(150), GUILayout.Height(25)))
                    {
                        // Must be executed in this order.

                        m_tilemapReader.SetTileMapComponent();
                        m_tilemapReader.SetLevelParent();
                        m_tilemapReader.SetMapLayers();
                        m_tilemapReader.GenerateMap();
                    }

                    GUI.backgroundColor = new Color(1, 0.2f, 0);
                    if (GUILayout.Button("Destroy Map", GUILayout.Width(150), GUILayout.Height(25)))
                    {
                        m_tilemapReader.SetLevelParent();
                        m_tilemapReader.DestroyLevelGenerated();
                    }

                    GUI.backgroundColor = Color.cyan;
                    if (GUILayout.Button("Push map as prefab", GUILayout.Width(150), GUILayout.Height(25)))
                    {
                        m_tilemapReader.PushLevelAsPrefab(m_tilemapReader.LevelParent);
                    }

                GUILayout.EndVertical();
            GUI.EndGroup();
        }
        Handles.EndGUI();
    }
}
