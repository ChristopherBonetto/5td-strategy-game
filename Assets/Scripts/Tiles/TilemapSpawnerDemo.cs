using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapSpawnerDemo : MonoBehaviour
{
    public Grid GridPrefab;
    public Tilemap TilemapPrefab;

    [Header("parent Empty Object")]
    public Transform ParentOfLevelgenerated;

    private List<GameObject> m_prefabsSpawned;


    private void OnValidate()
    {
        if (GridPrefab)
            TilemapPrefab = GridPrefab.GetComponentInChildren<Tilemap>();
    }

    public void GenerateLevel()
    {
        if (m_prefabsSpawned == null)
            m_prefabsSpawned = new List<GameObject>();

        DestroyMapGenerated();

        // Get boundieris of tilemap.
        BoundsInt bounds = TilemapPrefab.cellBounds;

        // Search all tile with that bounds.
        TileBase[] tiles = TilemapPrefab.GetTilesBlock(bounds);


        for (int i = 0; i < bounds.size.x; i++)
        {
            for (int j = 0; j < bounds.size.y; j++)
            {
                TileBase tile = tiles[i + j * bounds.size.x];

                if (tile)
                {
                    Vector3Int position = new Vector3Int(i, 0, j);

                    // Store tile datas
                    TileData tileData = new TileData();
                    tile.GetTileData(position, null, ref tileData);

                    GameObject go = Instantiate(tileData.gameObject, position, Quaternion.identity, ParentOfLevelgenerated);
                    m_prefabsSpawned.Add(go);
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
}
