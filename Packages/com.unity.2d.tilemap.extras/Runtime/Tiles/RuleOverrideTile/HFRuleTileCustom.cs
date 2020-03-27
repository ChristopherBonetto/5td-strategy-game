using UnityEngine.Tilemaps;
using UnityEngine;

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
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
            case Neighbor.Null: return tile == null;
            case Neighbor.NotNull: return tile != null;
        }
        return base.RuleMatch(neighbor, tile);
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
