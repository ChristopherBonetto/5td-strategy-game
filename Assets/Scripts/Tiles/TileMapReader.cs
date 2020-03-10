using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class TileMapReader : MonoBehaviour
{
    #region Tilemap Reader
    /// <summary>
    /// Store the Grid's children that has TileMap and TileMapRenderer Components.
    /// </summary>
    [System.Serializable]
    public class TileMapContainer
    {
        [Tooltip("Think about it as photoshop layer")]
        public string Tag = "";

        public readonly int Columns;
        public readonly int Rows;

        public readonly BoundsInt Area = new BoundsInt(Vector3Int.zero, Vector3Int.zero);

        public readonly Tilemap TileMap;
        public readonly TilemapRenderer TileMapRenderer;

        public Dictionary<Vector3Int, HFRuleTileCustom> TilesToSpawn;


        public TileMapContainer(Tilemap _tileMap, TilemapRenderer _tilemapRenderer, BoundsInt _area)
        {
            TileMap = _tileMap;
            TileMapRenderer = _tilemapRenderer;

            Area = _area;

            Tag = TileMap.gameObject.tag;

            TilesToSpawn = new Dictionary<Vector3Int, HFRuleTileCustom>();

            Columns = Area.xMax - Area.xMin;
            Rows = Area.yMax - Area.yMin;
        }

        /// <summary>
        /// return all tiles in the tile area.
        /// The area is = to the painted area.
        /// </summary>
        /// <returns></returns>
        public void GetTilesAndCoordinates()
        {
            for (int columns = Area.xMin; columns < Area.xMax; columns++)
            {
                for (int rows = Area.yMin; rows < Area.xMax; rows++)
                {
                    Vector3Int position = new Vector3Int(columns, rows, 0);
                    HFRuleTileCustom ruleTile = TileMap.GetTile<HFRuleTileCustom>(position);

                    if (ruleTile != null)
                    {
                        if (TilesToSpawn.ContainsKey(position)) TilesToSpawn.Remove(position);
                        TilesToSpawn.Add(position, ruleTile);
                    }
                }
            }
        }
    }

    public Grid Grid;
    public TileMapContainer[] m_tileMapAndRendererContainer = new TileMapContainer[1];
    #endregion

    #region Editor Creation Map
    public GameObject m_LevelParent { get; private set; } = null;
    private GameObject m_Terrain = null;
    private Dictionary<int, GameObject> m_LayerGameObjects;

    public Dictionary<int, GameObject> LayerGameObjects 
    { 
        get
        {
            if (m_LayerGameObjects == null)
            {
                m_LayerGameObjects = new Dictionary<int, GameObject>();
            }
            return m_LayerGameObjects;
        }
    }
    #endregion

    /// <summary>
    /// Set the containers that hold the tilemaps.
    /// </summary>
    public void SetTileMapContainer()
    {
        // It supposed that each gameobject that hold the Tilemap component,
        // also has TilemapRenderer. So the lenght is equal.

        Tilemap[] tilemaps = Grid.GetComponentsInChildren<Tilemap>();
        TilemapRenderer[] tilemapRenderes = Grid.GetComponentsInChildren<TilemapRenderer>();

        int length = tilemaps.Length;

        m_tileMapAndRendererContainer = new TileMapContainer[length];

        for (int i = 0; i < length; i++)
        {
            m_tileMapAndRendererContainer[i] = new TileMapContainer(tilemaps[i], tilemapRenderes[i], tilemaps[i].cellBounds);
            m_tileMapAndRendererContainer[i].GetTilesAndCoordinates();
        }
    }

    public void SetLayers()
    {
        foreach (TileMapContainer container in m_tileMapAndRendererContainer)
        {
            if (container.TileMap.tag == "Terrain" && m_Terrain == null)
            {
                m_Terrain = new GameObject(container.TileMap.name);
                m_Terrain.transform.position = Vector3.zero;
                m_Terrain.transform.rotation = Quaternion.identity;
                m_Terrain.transform.SetParent(m_LevelParent.transform);
            }
            else if (container.TileMap.tag == "Terrain" && m_Terrain != null)
            {
                // Skip
                continue;
            }
            else
            {
                GameObject go = new GameObject(container.TileMap.name);
                go.transform.position = Vector3.zero;
                go.transform.rotation = Quaternion.identity;
                go.transform.SetParent(m_LevelParent.transform);
                LayerGameObjects.Add(container.TileMap.GetHashCode(), go);
            }
        }
    }

    /// <summary>
    /// Spawn the prefab associated to each tile.
    /// </summary>
    public void Spawn3DLevel()
    {
        foreach (TileMapContainer container in m_tileMapAndRendererContainer)
        {
            Debug.Log(container.TileMap.name);
            GameObject goToFill = container.TileMap.tag != "Terrain" ? LayerGameObjects[container.TileMap.GetHashCode()] : m_Terrain;

            // Force the tile to refresh and activate StartUp(), GetTileData() methods.
            container.TileMap.RefreshAllTiles();

            foreach (Vector3Int position in container.TilesToSpawn.Keys)
            {
                // It's used only to match the rule mathod.
                Matrix4x4 identity = Matrix4x4.identity;

                // Set default object to spawn.
                GameObject objectToSpawn = container.TilesToSpawn[position].m_DefaultGameObject;

                // Check if the rule matchs
                foreach (RuleTile.TilingRule rule in container.TilesToSpawn[position].m_TilingRules)
                {
                    if (container.TilesToSpawn[position].RuleMatches(rule, position, container.TilesToSpawn[position].ITilemap, ref identity))
                    {
                        objectToSpawn = rule.m_GameObject;
                        objectToSpawn.transform.rotation = Quaternion.identity;
                        // If find a match break this inner loop.
                        break;
                    }
                }

                Instantiate(
                    objectToSpawn,
                    container.TileMap.CellToLocalInterpolated(position + new Vector3(0.5f, 0.5f, 0.5f)) + Vector3.up * container.TileMap.transform.position.y,
                    Quaternion.identity,
                    goToFill.transform
                    );
            }
        }
    }

    public void CheckParent()
    {
        if (m_LevelParent == null)
        {
            m_LevelParent = new GameObject(Grid.name);
            m_LevelParent.transform.position = Vector3.zero;
            m_LevelParent.transform.rotation = Quaternion.identity;
        }
    }

    public void GenerateLevelFromTilemap()
    {
        CheckParent();
        Spawn3DLevel();
    }

    public void DestroyLevelGenerated()
    {
        LayerGameObjects.Clear();
        m_Terrain = null;
        DestroyImmediate(m_LevelParent);
    }

    public void PushLevelAsPrefab(GameObject level)
    {
        // Set the path as within the Assets folder,
        // and name it as the GameObject's name with the .Prefab format
        string localPath = "Assets/Prefabs/LevelCreated/" + "Level_00" + ".prefab";

        // Make sure the file name is unique, in case an existing Prefab has the same name.
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

        // Create the new Prefab.
        PrefabUtility.SaveAsPrefabAsset(level, localPath);
    }
}
