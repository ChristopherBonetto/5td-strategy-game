using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace HF.Refactoring
{
    /// <summary>
    /// Component used in editor mode only.
    /// Itself is the map parent.
    /// </summary>
    public class HFMapGenerator : MonoBehaviour
    {
        [HideInInspector]
        public Grid MapGrid;

        public string StoreTileMapAtPath = "Prefabs/Tilemap Created/";
        public string StoreMapAtPath = "Prefabs/Map Generated/"; 

        private HFTileMapHandler[] m_tilemapHandlers;
        private Dictionary<string, GameObject> m_layers = new Dictionary<string, GameObject>();


        // Since this is a monobheaviour class,
        // whent go in play mode i want to disable this script
        private void Start()
        {
            this.enabled = !Application.isPlaying;
        }

        /// <summary>
        /// Find the Grid component in the scene.
        /// </summary>
        /// <returns></returns>
        public void SetGrid()
        {
            MapGrid = FindObjectOfType<Grid>();

            if (MapGrid == null)
            {
                Debug.LogError($"[{this.GetType().Name}] : There isn't any Grid component in this scene");
            }
        }

        /// <summary>
        /// Set the Tilemap Handlers
        /// </summary>
        public void SetTilemapHandlers()
        {
            if (MapGrid == null)
            {
                Debug.LogError($"[{this.GetType().Name}] : There isn't any Grid component in this scene");
                return;
            }

            Tilemap[] tilemaps = MapGrid.GetComponentsInChildren<Tilemap>();
            m_tilemapHandlers = new HFTileMapHandler[tilemaps.Length];

            // Initialize every handler
            for (int i = 0; i < tilemaps.Length; i++)
            {
                m_tilemapHandlers[i] = new HFTileMapHandler(tilemaps[i], tilemaps[i].cellBounds);
            }
        }

        /// <summary>
        /// Store the tile of every tilemap into the associated handler
        /// </summary>
        public void SetTileToHandlers()
        {
            for (int i = 0; i < m_tilemapHandlers.Length; i++)
            {
                m_tilemapHandlers[i].SetTiles();
            }
        }

        /// <summary>
        /// Destroy the previous map generated.
        /// </summary>
        public void DestroyPreviousMap()
        {
            if (m_layers.Count <= 0) return;

            foreach (GameObject gameObject in m_layers.Values)
            {
                DestroyImmediate(gameObject);
            }
        }

        public void GenerateMap()
        {
            // If there is already a map generated, destroy it.
            DestroyPreviousMap();

            SetGrid();
            if (MapGrid == null)
            {
                return;
            }

            SetTilemapHandlers();
            SetTileToHandlers();

            m_layers.Clear();

            foreach (HFTileMapHandler mapHandler in m_tilemapHandlers)
            {
                GameObject layer = null;

                // Set up the layer for map generated
                if (!m_layers.ContainsKey(mapHandler.Layer))
                {
                    layer = new GameObject(mapHandler.Layer);
                    layer.transform.SetParent(transform);
                    m_layers.Add(mapHandler.Layer, layer);
                }
                else
                {
                    layer = m_layers[mapHandler.Layer];
                }

                // Force all tiles to update.
                mapHandler.Tilemap.RefreshAllTiles();

                foreach (Vector3Int pos in mapHandler.Tiles.Keys)
                {
                    // It's used only to match the rule mathod.
                    Matrix4x4 identity = Matrix4x4.identity;

                    // Initialize the gameObject to spawn to default.
                    GameObject objectToSpawn = mapHandler.Tiles[pos].m_DefaultGameObject;

                    // Check if the rule matches
                    foreach (RuleTile.TilingRule rule in mapHandler.Tiles[pos].m_TilingRules)
                    {
                        if (mapHandler.Tiles[pos].RuleMatches(rule, pos, mapHandler.Tiles[pos].ITilemap, ref identity))
                        {
                            // To get the right gameobject
                            // first we create a tile data, tile data contains all data of the tile (prefab also).
                            // The we fill the TileData with the TileData at the position declared.
                            TileData data = new TileData();
                            mapHandler.Tiles[pos].GetTileData(pos, mapHandler.Tiles[pos].ITilemap, ref data);


                            // Handle null condition.

                            if (data.gameObject != null)
                            {
                                objectToSpawn = data.gameObject;
                                objectToSpawn.transform.rotation = Quaternion.identity;
                            }
                            else
                            {
                                Debug.LogWarning($"The tile at position: {pos} has no prefab, so i spawn the default one.");
                            }

                            break;
                        }
                    }

                    Instantiate(
                        objectToSpawn, 
                        mapHandler.Tilemap.CellToLocalInterpolated(pos + new Vector3(0.5f, 0.5f, 0.5f)) + Vector3.up * mapHandler.Tilemap.transform.position.y,
                        Quaternion.identity,
                        layer.transform
                        );
                }
            }
        }
    }
}