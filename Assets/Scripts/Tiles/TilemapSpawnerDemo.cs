using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapSpawnerDemo : MonoBehaviour
{
    public Grid GridPrefab;
    public Tilemap[] TilemapPrefab;

    [Header("parent Empty Object")]
    public Transform ParentOfLevelgenerated;

    private List<GameObject> m_prefabsSpawned;


    private void OnValidate()
    {
        if (GridPrefab)
            TilemapPrefab = GridPrefab.GetComponentsInChildren<Tilemap>(true);
    }

    public void GenerateLevel()
    {
        if (m_prefabsSpawned == null)
            m_prefabsSpawned = new List<GameObject>();

        DestroyMapGenerated();

        // Get boundieris of tilemap.
        BoundsInt bounds = GetTheMaxBoundaries(TilemapPrefab).cellBounds;
        Debug.Log(bounds);

        foreach (var map in TilemapPrefab)
        {
            // Search all tile with that bounds.
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

                        GameObject go = Instantiate(tileData.gameObject, worldPos, Quaternion.identity, ParentOfLevelgenerated);
                        m_prefabsSpawned.Add(go);
                    }
                }
            }
        }
    }

    private void DestroyMapGenerated()
    {
        foreach (var item in m_prefabsSpawned)
        {
            DestroyImmediate(item);
        }

        m_prefabsSpawned.Clear();
    }

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
