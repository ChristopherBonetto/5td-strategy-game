#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.IO;

public class TilemapSpawnerDemo
{
    // Tilemap
    public Grid GridPrefab;
    public Tilemap[] TilemapPrefab;

    // Level Generated
    private GameObject m_ParentOfLevelgenerated;
    public GameObject ParentOfLevelGenerated
    {
        get
        {
            if (m_ParentOfLevelgenerated == null)
            {
                m_ParentOfLevelgenerated = new GameObject("ParentOfLevelGenerated");
            }
            return m_ParentOfLevelgenerated;
        }
    }

    private List<GameObject> m_prefabsSpawned;


    private void OnValidate()
    {
        if (GridPrefab)
            TilemapPrefab = GridPrefab.GetComponentsInChildren<Tilemap>(true);
    }

    /// <summary>
    /// Generate level from Tilemap.
    /// </summary>
    public void GenerateLevel()
    {
        if (m_prefabsSpawned == null)
            m_prefabsSpawned = new List<GameObject>();

        DestroyMapGenerated();

        // Get the greatest boundaries of tilemap.
        BoundsInt bounds = GetTheMaxBoundaries(TilemapPrefab).cellBounds;
        Debug.Log(bounds);

        foreach (var map in TilemapPrefab)
        {
            // Search all tiles with that bounds.
            TileBase[] tiles = map.GetTilesBlock(bounds);


            for (int columns = 0; columns < bounds.size.x; columns++)
            {
                for (int rows = 0; rows < bounds.size.y; rows++)
                {
                    TileBase tile = tiles[columns + (rows * bounds.size.x)];

                    if (tile)
                    {
                        Vector3Int position = new Vector3Int(columns + map.editorPreviewOrigin.x, rows + map.editorPreviewOrigin.y, 0);
                        Vector3 worldPos = GridPrefab.CellToWorld(position);

                        // Store tile datas
                        TileData tileData = new TileData();
                        tile.GetTileData(position, null, ref tileData);

                        GameObject go = GameObject.Instantiate(tileData.gameObject, worldPos, Quaternion.identity, ParentOfLevelGenerated.transform);
                        m_prefabsSpawned.Add(go);
                    }
                }
            }
        }
    }

    public void Fill(TileBase fillTile)
    {

        // Get the greatest boundieris of tilemap.
        BoundsInt bounds = GetTheMaxBoundaries(TilemapPrefab).cellBounds;
        Debug.Log(bounds);

        foreach (var map in TilemapPrefab)
        {
            // Search all tile with that bounds.
            TileBase[] tiles = map.GetTilesBlock(bounds);


            for (int columns = 0; columns < bounds.size.x; columns++)
            {
                int min = 1000;
                int max = -1000;
                for (int rows = 0; rows < bounds.size.y; rows++)
                {
                    TileBase tile = tiles[columns + (rows * bounds.size.x)];

                    if (tile)
                    {
                        int tileY = rows + map.editorPreviewOrigin.y;
                        if (tileY < min)
                        {
                            min = tileY;
                        }
                        if (tileY > max)
                        {
                            max = tileY;
                        }
                    }
                }
                Debug.Log("Col " + columns + ": min " + min + " max " + max);
            }
        }
    }

    public void PushLevelAsPrefab()
    {
        if (ParentOfLevelGenerated.transform.childCount > 0)
        {
            // Set the path as within the Assets folder,
            // and name it as the GameObject's name with the .Prefab format
            string localPath = "Assets/Prefabs/LevelGenerated/" + "Level_00" + ".prefab";

            // Make sure the file name is unique, in case an existing Prefab has the same name.
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

            // Create the new Prefab.
            PrefabUtility.SaveAsPrefabAsset(ParentOfLevelGenerated, localPath);
        }
    }

    private void DestroyMapGenerated()
    {
        foreach (var item in m_prefabsSpawned)
        {
            GameObject.DestroyImmediate(item);
        }
        m_prefabsSpawned.Clear();
    }

    /// <summary>
    /// Return the Tilemap with the greatest boudaries.
    /// </summary>
    /// <param name="maps"></param>
    /// <returns></returns>
    private Tilemap GetTheMaxBoundaries(Tilemap[] maps)
    {
        Tilemap map = maps[0];
        BoundsInt bounds = map.cellBounds;
        int max = bounds.xMax + bounds.yMax + bounds.zMax;

        for (int i = 1; i < maps.Length; i++)
        {
            BoundsInt tempBounds = maps[i].cellBounds;
            int tempMax = tempBounds.xMax + tempBounds.yMax + tempBounds.zMax;

            if (max < tempMax)
            {
                map = maps[i];
                bounds = map.cellBounds;
                max = bounds.xMax + bounds.yMax + bounds.zMax;
            }
        }

        return map;
    }
}
#endif
