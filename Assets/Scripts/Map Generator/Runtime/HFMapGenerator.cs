using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#region
using UnityEditor;
#endregion
#if UNITY_EDITOR
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

        private HFTileMapHandler[] m_tilemapHandlers;   // All tilemaps drawn
        private Dictionary<string, GameObject> m_layers = new Dictionary<string, GameObject>(); // All drawn tilemaps' layer


        /*
         * --------------------------------------------------------------------
         * Tilemap creation optimizations.
         * --------------------------------------------------------------------
         */

        private List<HFTileMapHandler> m_orderedLayers = new List<HFTileMapHandler>();

        // Checker vectors ordered clockwise.
        private readonly Vector3Int[] m_checkerVectors =
        {
            new Vector3Int(1,1,0),     //top right,
            new Vector3Int(-1,1,0),    //top left,
            new Vector3Int(0,1,0),     //top,
            new Vector3Int(1,0,0),     //right,
            new Vector3Int(-1,0,0),    //left,
            new Vector3Int(0,-1,0),    //bottom,
            new Vector3Int(1,-1,0),    //bottom right,
            new Vector3Int(-1,-1,0),   //bottom left,
        };


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

        public void Streamline()
        {
            // Reset the list
            m_orderedLayers.Clear();

            // the toppest layer will be the first element in the array.
            for (int i = m_tilemapHandlers.Length - 1; i >= 0; i--)
            {
                if (m_tilemapHandlers[i].Layer == "Bakeable")
                {
                    m_orderedLayers.Add(m_tilemapHandlers[i]);
                }
            }


            // store what tilemap's tile to remove
            Dictionary<HFTileMapHandler, List<Vector3Int>> positionToRemove = new Dictionary<HFTileMapHandler, List<Vector3Int>>();

            // Iterate from the 2 elemnt
            for (int i = 1; i < m_orderedLayers.Count; i++)
            {
                // create a list to store keys to remove
                List<Vector3Int> positionToDelete = new List<Vector3Int>();

                // reference to the tilemap
                HFTileMapHandler tilemap = m_orderedLayers[i];


                // Iterate every keys in the tilemap
                foreach (Vector3Int position in tilemap.Tiles.Keys)
                {
                    bool isPositionFull = false;

                    // Check in each position
                    foreach (Vector3Int checkdirection in m_checkerVectors)
                    {
                        isPositionFull = tilemap.Tiles.ContainsKey(position + checkdirection);

                        // If find null in a direction, break the loop. 
                        // This means the tile is usefull to the renderer.
                        if (!isPositionFull) break;
                    }

                    // If the all checked position are full 
                    if (isPositionFull)
                    {
                        // Check the topper tilemap
                        bool topIsFull = m_orderedLayers[i - 1].Tiles.ContainsKey(position);

                        // If it is then add it the list to remove.
                        if (topIsFull)
                            positionToDelete.Add(position);
                    }
                }

                positionToRemove.Add(tilemap, positionToDelete);
            }


            // Since i can't resize the dictionary in the for loop, i have to store all values,
            // and afterwords remove they.
            foreach (var item in positionToRemove.Keys)
            {
                foreach (var pos in positionToRemove[item])
                {
                    item.Tiles.Remove(pos);
                }
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

            Streamline();

            foreach (HFTileMapHandler mapHandler in m_tilemapHandlers)
            {
                GameObject layer = null;

                // Set up the layer for map generated
                if (!m_layers.ContainsKey(mapHandler.Layer))
                {
                    layer = new GameObject(mapHandler.Layer);
                    layer.layer = mapHandler.Tilemap.gameObject.layer;
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

                    // store spawn position
                    Vector3 fixedPosition = mapHandler.Tilemap.CellToLocalInterpolated(pos + new Vector3(0.5f, 0.5f, 0.5f)) + Vector3.up * mapHandler.Tilemap.transform.position.y;

                    GameObject objSpawned = (GameObject)PrefabUtility.InstantiatePrefab(
                        objectToSpawn, 
                        layer.transform
                        );

                    objSpawned.transform.position = fixedPosition;
                    objSpawned.layer = layer.layer;
                }
            }
        }
    }
}
#endif