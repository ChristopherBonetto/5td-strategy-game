using UnityEngine.Tilemaps;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Human Factor/Tile Map/Rule Tile", fileName = "New_HF_RuleTile")]
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
        public const int CompatibleTile = 5;
    }

    public override bool RuleMatch(int neighbor, TileBase other)
    {
        switch (neighbor)
        {
            case Neighbor.Null: return other == null;
            case Neighbor.NotNull: return other != null;
            case Neighbor.CompatibleTile:
                bool isMatchCondition = neighbor == RuleTile.TilingRule.Neighbor.This;
                bool isMatch = other == this;

                if (!isMatch)
                {
                    isMatch = siblings.Contains(other);
                }

                if (!isMatch && matchSiblingLayer)
                {
                    if (other is RuleOverrideTile)
                        other = (other as RuleOverrideTile).m_InstanceTile;

                    if (other is HFRuleTileCustom)
                        isMatch = siblingLayer == (other as HFRuleTileCustom).siblingLayer;
                }

                return isMatch == isMatchCondition;

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
