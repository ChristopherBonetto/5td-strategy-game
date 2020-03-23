using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HFTileMapGenerator))]
public class HFTileMapGeneratorEditor : EditorWindow
{
    public HFTileMapGenerator m_tilemapReader;

    public bool ShowPaths = false;

    // Constant string paths.
    public string prefabToPushPath = "Assets/Prefabs/Map Generated/" + "MapLevel_00" + ".prefab";
    public string tilemapToPushPath = "Assets/Prefabs/Tilemap Created/" + "TilemapLevel_00" + ".prefab";
    public string meshToPushPath = "Assets/3D/Asset/Map Mesh Generated/" + "MapMeshLevel_00" + ".asset";

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
            GUI.BeginGroup(new Rect(20, 20, 600, 400));
                GUILayout.BeginVertical();

                    GUI.backgroundColor = Color.gray;
                    if (GUILayout.Button("Generate Map", GUILayout.Width(150), GUILayout.Height(25)))
                    {
                        // Must be executed in this order.

                        m_tilemapReader.DestroyLevelGenerated();
                        m_tilemapReader.SetTileMapComponent();
                        m_tilemapReader.SetLevelParent();
                        m_tilemapReader.SetMapLayers();
                        m_tilemapReader.GenerateMap();
                    }

                    GUILayout.Space(10);

                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("Combine terrain mesh", GUILayout.Width(150), GUILayout.Height(25)))
                    {
                        m_tilemapReader.GenerateTerrainMesh();
                        m_tilemapReader.AddMeshColliderToTerrain();
                    }

                    GUILayout.Space(10);

                    GUI.backgroundColor = new Color(1, 0.2f, 0);
                    if (GUILayout.Button("Destroy Map", GUILayout.Width(150), GUILayout.Height(25)))
                    {
                        m_tilemapReader.SetLevelParent();
                        m_tilemapReader.DestroyLevelGenerated();
                    }

                    GUILayout.Space(10);
                    

                    GUI.backgroundColor = Color.cyan;
                    if (GUILayout.Button("Push map as prefab", GUILayout.Width(150), GUILayout.Height(25)))
                    {
                        m_tilemapReader.PushMesh(ref meshToPushPath);
                        m_tilemapReader.Terrain.GetComponent<MeshFilter>().sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshToPushPath);
                        m_tilemapReader.Terrain.GetComponent<MeshCollider>().sharedMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshToPushPath);

                        m_tilemapReader.PushLevelAsPrefab(m_tilemapReader.LevelParent, ref prefabToPushPath);
                        m_tilemapReader.PushLevelAsPrefab(m_tilemapReader.Grid.gameObject, ref tilemapToPushPath);
                    }

                    ShowPaths = EditorGUILayout.Toggle("Show paths", ShowPaths);
                    if (ShowPaths)
                    {
                        GUILayout.Label("Show the paths of the last prefabs pushed");
                        GUILayout.Space(2);
                        GUILayout.Label($"3D map prefab is pushed to: \n{prefabToPushPath}");
                        GUILayout.Label($"Tilemap prefab is pushed to: \n{tilemapToPushPath}");
                        GUILayout.Label($"Mesh generated is pushed to: \n{meshToPushPath}");
                    }

                GUILayout.EndVertical();
            GUI.EndGroup();
        }
        Handles.EndGUI();
    }
}
