using UnityEngine.Tilemaps;
using UnityEngine;

[CreateAssetMenu(menuName = "Human Factor/Tile Map/Rule Tile", fileName = "New_HF_RuleTile")]
public class HFRuleTileCustom : RuleTile
{
    [Tooltip("Make visible or not the gameObject associated")]
    public TileFlags m_flag;

    /// <summary>
    /// Store the Itilemap interface where it's drawn on.
    /// </summary>
    public ITilemap ITilemap { get; private set; }

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
