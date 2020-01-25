#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

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
    public void GenerateLevel(GameObject parent)
    {
        // Null check.
        if (m_prefabsSpawned == null)
            m_prefabsSpawned = new List<GameObject>();

        // destroy previous level generated.
        DestroyMapGenerated();

        // Get the greatest boundaries of tilemap.
        BoundsInt bounds = GetTheMaxBoundaries(TilemapPrefab).cellBounds;

        // Cycle every tilemaps created.
        foreach (Tilemap map in TilemapPrefab)
        {
            // Search all tiles with that bounds.
            TileBase[] tiles = map.GetTilesBlock(bounds);
			

            for (int columns = 0; columns < bounds.size.x; columns++)
            {
                for (int rows = 0; rows < bounds.size.y; rows++)
                {
                    // Transfrom from list to "matrix" coordinate.
                    TileBase tile = tiles[columns + (rows * bounds.size.x)];

                    if (tile)
                    {
                        // Store position based on tilemap coordinates.
                        // Store world position.
                        Vector3Int position = new Vector3Int(columns + map.editorPreviewOrigin.x, rows + map.editorPreviewOrigin.y, 0);
                        Vector3 worldPos = GridPrefab.CellToWorld(position);

                        // Find tile data.
                        TileData tileData = new TileData();
                        tile.GetTileData(position, null, ref tileData);

                        // Instantiate.
                        GameObject go = Object.Instantiate(tileData.gameObject, new Vector3(worldPos.x, 0, worldPos.y), Quaternion.identity, parent.transform);
                        m_prefabsSpawned.Add(go);
                    }
                }
            }
        }
    }

    public void Fill(TileBase fillTile)
    {
		BoundsInt bounds = GetTheMaxBoundaries(TilemapPrefab).cellBounds;
        // Get the greatest boundieris of tilemap.
        foreach (Tilemap map in TilemapPrefab)
        {
            // Search all tiles within that box bounds.
            TileBase[] tiles = map.GetTilesBlock(bounds);

            for (int columns = 0; columns < bounds.size.x; columns++)
            {
                int min = 1000;
                int max = -1000;
				// Find real min and max (shape bounds)
                for (int rows = 0; rows < bounds.size.y; rows++)
                {
					TileBase tile = tiles[columns + (rows * bounds.size.x)];

					if (tile)
                    {
                        min = (rows < min) ? rows : min;
                        max = (rows > max) ? rows : max;
                    }
                }
				// Fill shape
				if (max > min)
				{
					for (int rows = 0; rows < bounds.size.y; rows++)
					{
						if (rows >= min && rows <= max)
						{
							tiles[columns + (rows * bounds.size.x)] = fillTile;
						}
					} 
				}
			}

			map.SetTilesBlock(bounds, tiles);
        }
    }

    public void PushLevelAsPrefab(GameObject level)
    {
        if (ParentOfLevelGenerated.transform.childCount > 0)
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

    private void DestroyMapGenerated()
    {
        foreach (GameObject item in m_prefabsSpawned)
        {
            Object.DestroyImmediate(item);
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
