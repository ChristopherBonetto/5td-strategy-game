namespace HF
{
	public interface IHFTargetable
	{
		UnityEngine.Vector3 Position { get; }
		bool TryInteraction(HFUnit otherUnit);
		IHFCommand GetDefaultInteraction();
	}
}
