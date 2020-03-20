using UnityEngine;

[CreateAssetMenu(fileName = "PoolID_name", menuName = "Human Factor/PoolID")]
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
