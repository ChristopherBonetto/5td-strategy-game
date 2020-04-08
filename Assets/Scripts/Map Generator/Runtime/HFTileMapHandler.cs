using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace HF.Refactoring
{
    /// <summary>
    /// Help to store tilemap info and its tiles.
    /// </summary>
    public class HFTileMapHandler
    {
        public readonly Tilemap Tilemap;
        public readonly BoundsInt AreaBounds;
        public readonly string Layer;
        public readonly int Columns;
        public readonly int Rows;
        public readonly Dictionary<Vector3Int, HFRuleTileCustom> Tiles;


        public HFTileMapHandler(Tilemap _tilemap, BoundsInt _areaBounds)
        {
            Tilemap = _tilemap;
            AreaBounds = _areaBounds;
            Layer = LayerMask.LayerToName(_tilemap.gameObject.layer);
            Columns = _areaBounds.xMax - _areaBounds.xMin;
            Rows = _areaBounds.yMax - _areaBounds.yMin;

            Tiles = new Dictionary<Vector3Int, HFRuleTileCustom>();
        }


        public void SetTiles()
        {
            Tiles.Clear();

            for (int columns = AreaBounds.xMin; columns < AreaBounds.xMax; columns++)
            {
                for (int rows = AreaBounds.yMin; rows < AreaBounds.yMax; rows++)
                {
                    // Even if the tilemap is oriented in X and Z,
                    // the calculation happens in X and Y.
                    Vector3Int position = new Vector3Int(columns, rows, 0);
                    HFRuleTileCustom tile = Tilemap.GetTile<HFRuleTileCustom>(position);

                    if (tile != null)
                    {
                        if (Tiles.ContainsKey(position))
                        {
                            Tiles.Remove(position);
                        }
                        Tiles.Add(position, tile);
                    }
                }
            }
        }
    }
}
