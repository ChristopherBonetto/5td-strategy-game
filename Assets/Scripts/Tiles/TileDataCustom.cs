using UnityEngine.Tilemaps;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Tile", fileName = "New Tile")]
public class TileDataCustom : TileBase
{
    public Sprite Sprite;
    public GameObject PrefabAssociated;


    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = Sprite;
        tileData.gameObject = PrefabAssociated;
        tileData.flags = TileFlags.InstantiateGameObjectRuntimeOnly;
    }
}
