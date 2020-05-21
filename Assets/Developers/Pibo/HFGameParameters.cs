public class HFGameParameters
{
	public const float TileSize = 3f;

	public const int PlayerTeam = 0;
	public const int NoTeam = 99;
}

public static class HFGameExtensionMethods
{
	public static UnityEngine.Vector3 SnapLocation(this UnityEngine.Vector3 location)
	{
		location /= HFGameParameters.TileSize;
		location.x = UnityEngine.Mathf.Round(location.x);
		//location.y = Mathf.Round(location.y);
		location.z = UnityEngine.Mathf.Round(location.z);
		location *= HFGameParameters.TileSize;

		return location;
	}
}
