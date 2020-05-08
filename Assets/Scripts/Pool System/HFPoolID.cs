using UnityEngine;

[CreateAssetMenu(fileName = "so_PoolID_name", menuName = "Human Factor/Pool system/Pool ID")]
public class HFPoolID : ScriptableObject
{
	public int ID;

	[ContextMenu("Generate ID")]
	public int GenerateID()
	{
		ID = GetHashCode();
		return ID;
	}
}
