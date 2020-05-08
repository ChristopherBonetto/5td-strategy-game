using UnityEngine.Tilemaps;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Human Factor/Tile Map/Rule Tile", fileName = "so_RT_name")]
public class HFRuleTileCustom : RuleTile<HFRuleTileCustom.Neighbor>
{

    [Tooltip("Make visible or not the gameObject associated")]
    public TileFlags m_flag;

    public ITilemap ITilemap { get; private set; }



    //---------------------------------------------------------------------------------------------
    // Override the RuleMatch method. This allow us to check new conditions other then default one:
    // is it null or not?
    //---------------------------------------------------------------------------------------------

    #region RuleMatch
    public List<TileBase> siblings = new List<TileBase>();
    public bool matchSiblingLayer;
    public int siblingLayer;


    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        /// <summary>
        /// The Rule Tile will check if the contents of the cell in that direction is null.
        /// If not, the rule will fail.
        /// </summary>
        public const int Null = 3;
        /// <summary>
        /// The Rule Tile will check if the contents of the cell in that direction is filled.
        /// If not, the rule will fail.
        /// </summary>
        public const int NotNull = 4;
        /// <summary>
        /// The Rule Tile will check if the contents of the cell in that direction is a compatible tile from the list.
        /// If it is not, the rule will fail.
        /// </summary>
        public const int Compatible = 5;
        /// <summary>
        /// The Rule Tile will check if the contents of the cell in that direction is not a compatible tile from the list.
        /// If it is not, the rule will fail.
        /// </summary>
        public const int NotCompatible = 6;
    }

    public override bool RuleMatch(int neighbor, TileBase other)
    {
        switch (neighbor)
        {
            case Neighbor.Null: return other == null;
            case Neighbor.NotNull: return other != null;
            case Neighbor.Compatible: return siblings.Contains(other);
            case Neighbor.NotCompatible: return !siblings.Contains(other);
        }
        return base.RuleMatch(neighbor, other);
    }
    #endregion

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
		base.GetTileData(position, tilemap, ref tileData);
		tileData.flags = m_flag;
        ITilemap = tilemap;
    }

    public override bool StartUp(Vector3Int location, ITilemap tilemap, GameObject instantiatedGameObject)
    {
        bool value = base.StartUp(location, tilemap, instantiatedGameObject);

        ITilemap = tilemap;
        Matrix4x4 matrix = tilemap.GetTransformMatrix(location);
        if (instantiatedGameObject != null)
        {
            GameObject go = instantiatedGameObject;
            go.transform.rotation = matrix.rotation;
        }

        return value;
    }
}
