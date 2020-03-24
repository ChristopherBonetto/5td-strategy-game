#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class HFTileMapGenerator
{
    #region Tilemap Reader

    /// <summary>
    /// Store the Grid's children that has TileMap Component.
    /// </summary>
    [System.Serializable]
    public class TileMapContainer
    {
        /// <summary>
        /// Use to filter layers.
        /// </summary>
        [Tooltip("Used to filter layers")]
        public string Tag = "";


        //--------------------------------------------------
        // Tilemap bounds
        //--------------------------------------------------

        /// <summary>
        /// Get the total columns of the tilemap
        /// </summary>
        public readonly int Columns;
        /// <summary>
        /// Get the total row of the tilemap
        /// </summary>
        public readonly int Rows;
        /// <summary>
        /// Get the bounds area of the tilemap
        /// </summary>
        public readonly BoundsInt Area = new BoundsInt(Vector3Int.zero, Vector3Int.zero);
        public readonly Tilemap TileMap;


        //---------------------------------------------------
        // Store the tile to spawn in every position drawn.
        //---------------------------------------------------

        private Dictionary<Vector3Int, HFRuleTileCustom> m_TilesToSpawn;
        /// <summary>
        /// Get all stored tiles in the tilemap.
        /// </summary>
        public Dictionary<Vector3Int, HFRuleTileCustom> TilesToSpawn
        {
            get => m_TilesToSpawn != null ? m_TilesToSpawn : m_TilesToSpawn = new Dictionary<Vector3Int, HFRuleTileCustom>();
        }


        //---------------------------------------------------
        // Constructor
        //---------------------------------------------------

        public TileMapContainer(Tilemap _tileMap, BoundsInt _area)
        {
            Tag = _tileMap.gameObject.tag;

            TileMap = _tileMap;

            Area = _area;
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
            // Go through the all columns 
            // to get the right bound on the x axis, iterate from the min to max
            for (int columns = Area.xMin; columns < Area.xMax; columns++)
            {
                // iterate in the same way for the row.
                for (int rows = Area.yMin; rows < Area.xMax; rows++)
                {
                    // Store position.
                    // Position is refered to tilemap position or 
                    // think about it as matrix coordinate
                    Vector3Int position = new Vector3Int(columns, rows, 0);

                    // Store the tile at that position
                    HFRuleTileCustom ruleTile = TileMap.GetTile<HFRuleTileCustom>(position);

                    if (ruleTile != null)
                    {
                        if (TilesToSpawn.ContainsKey(position))
                        {
                            TilesToSpawn.Remove(position);
                        }
                        TilesToSpawn.Add(position, ruleTile);
                    }
                }
            }
        }
    }

    public Grid Grid;
    private TileMapContainer[] m_tileMapAndRendererContainer = new TileMapContainer[1];

    #endregion

    #region Editor Creation Map

    //------------------------------------------------------
    // It's the Grid (the father of all tiles).
    // This will be the upper most gameObject of the level.
    //------------------------------------------------------

    private GameObject m_LevelParent = null;
    public GameObject LevelParent => m_LevelParent;
    

    //------------------------------------------------------
    // Terrain is the gameObject that handle all prefabs that
    // compose the terrain of the level, this will be used to 
    // generate and combine the mesh.
    //------------------------------------------------------

    private GameObject m_Terrain = null;
    public GameObject Terrain => m_Terrain;


    //------------------------------------------------------
    // All tilemaps under the Grid component.
    //------------------------------------------------------

    private Dictionary<int, GameObject> m_TileMapLayers;
    public Dictionary<int, GameObject> TileMapLayers 
    {
        get => m_TileMapLayers != null ? m_TileMapLayers : m_TileMapLayers = new Dictionary<int, GameObject>();
    }

    private Mesh m_mapMeshGenerated = null;

    #endregion

    /// <summary>
    /// Set the containers that hold the tilemaps.
    /// </summary>
    public void SetTileMapComponent()
    {
        Tilemap[] tilemaps = Grid.GetComponentsInChildren<Tilemap>();

        int length = tilemaps.Length;
        m_tileMapAndRendererContainer = new TileMapContainer[length];

        for (int i = 0; i < length; i++)
        {
            m_tileMapAndRendererContainer[i] = new TileMapContainer(tilemaps[i], tilemaps[i].cellBounds);
            m_tileMapAndRendererContainer[i].GetTilesAndCoordinates();
        }
    }



    /// <summary>
    /// Spawn the prefab associated to each tile.
    /// </summary>
    public void GenerateMap()
    {
        // Go through the containers
        foreach (TileMapContainer container in m_tileMapAndRendererContainer)
        {
            // if the tilemap in the container is NOT marked as "Terrain" ?
            // pick the stored tilemap layer :
            // else pick the terrain G.O.
            GameObject goToFill = 
                container.TileMap.tag != "Terrain" ? 
                TileMapLayers[container.TileMap.GetHashCode()] : 
                Terrain;

            // Without this tiles don't give us the data we need.
            // Force all tile to refresh.
            // This means that update all tile data for every tile.
            container.TileMap.RefreshAllTiles();

            foreach (Vector3Int position in container.TilesToSpawn.Keys)
            {
                // It's used only to match the rule mathod.
                Matrix4x4 identity = Matrix4x4.identity;

                // Initialize the gameObject to spawn to default.
                GameObject objectToSpawn = container.TilesToSpawn[position].m_DefaultGameObject;

                // Check if the rule matches
                foreach (RuleTile.TilingRule rule in container.TilesToSpawn[position].m_TilingRules)
                {
                    if (container.TilesToSpawn[position].RuleMatches(rule, position, container.TilesToSpawn[position].ITilemap, ref identity))
                    {
                        // To get the right gameobject
                        // first we create a tile data, tile data contains all data of the tile (prefab also).
                        // The we fill the TileData with the TileData at the position declared.
                        TileData data = new TileData();
                        container.TilesToSpawn[position].GetTileData(position, container.TilesToSpawn[position].ITilemap, ref data);


                        // Handle null condition.

                        if (data.gameObject != null)
                        {
                            objectToSpawn = data.gameObject;
                            objectToSpawn.transform.rotation = Quaternion.identity;
                        }
                        else
                        {
                            Debug.LogWarning($"The tile at position: {position} has no prefab, so i spawn the default one.");
                        }

                        break;
                    }
                }

                // Instantiate GameObject with an offset.

                GameObject.Instantiate(
                    objectToSpawn,
                    container.TileMap.CellToLocalInterpolated(position + new Vector3(0.5f, 0.5f, 0.5f)) + Vector3.up * container.TileMap.transform.position.y,
                    Quaternion.identity,
                    goToFill.transform
                    );
            }
        }
    }

    public void SetMapLayers()
    {
        foreach (TileMapContainer container in m_tileMapAndRendererContainer)
        {
            if (container.TileMap.tag == "Terrain" && Terrain == null)
            {
                m_Terrain = new GameObject(container.TileMap.name);
                ResetGameObjectValue(Terrain);
                m_Terrain.transform.SetParent(m_LevelParent.transform);
            }
            else if (container.TileMap.tag == "Terrain" && Terrain != null)
            {
                // Skip
                continue;
            }
            else
            {
                GameObject go = new GameObject(container.TileMap.name);
                ResetGameObjectValue(go);
                go.transform.SetParent(m_LevelParent.transform);
                TileMapLayers.Add(container.TileMap.GetHashCode(), go);
            }
        }
    }

    public void SetLevelParent()
    {
        if (m_LevelParent != null) return;

        m_LevelParent = new GameObject(Grid.name);
        m_LevelParent.transform.position = Vector3.zero;
        m_LevelParent.transform.rotation = Quaternion.identity;
    }

    public void GenerateTerrainMesh()
    {
        MeshCombiner mesh = m_Terrain.GetComponent<MeshCombiner>();

        if (mesh == null)
        {
            m_Terrain.AddComponent(typeof(MeshCombiner));
            mesh = m_Terrain.GetComponent<MeshCombiner>();
            mesh.CreateMultiMaterialMesh = true;
            mesh.DeactivateCombinedChildren = true;
        }
        mesh.CombineMeshes(false);
        m_mapMeshGenerated = m_Terrain.GetComponent<MeshFilter>().sharedMesh;
    }

    public void AddMeshColliderToTerrain()
    {
        if (m_Terrain.GetComponent<MeshCollider>() == null)
        {
            m_Terrain.AddComponent(typeof(MeshCollider));
        }
    }

    public void DestroyLevelGenerated()
    {
        if (m_LevelParent == null) return;

        TileMapLayers.Clear();
        m_Terrain = null;
        m_mapMeshGenerated = null;
        GameObject.DestroyImmediate(m_LevelParent);
    }

    public void PushLevelAsPrefab(GameObject level, ref string localPath)
    {
        // Make sure the file name is unique, in case an existing Prefab has the same name.
        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

        // Create the new Prefab.
        PrefabUtility.SaveAsPrefabAsset(level, localPath);
    }

    public void PushMesh(ref string path)
    {
        // Make sure the file name is unique, in case an existing Prefab has the same name.
        path = AssetDatabase.GenerateUniqueAssetPath(path);

        // Create the new Prefab.
        AssetDatabase.CreateAsset(m_mapMeshGenerated, path);
    }

    private void ResetGameObjectValue(GameObject go)
    {
        go.transform.position = Vector3.zero;
        go.transform.rotation = Quaternion.identity;
    }
}
#endif
