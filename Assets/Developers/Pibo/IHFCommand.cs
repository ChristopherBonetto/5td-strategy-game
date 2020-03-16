namespace HF
{
	/// <summary>
	/// Interface for command system
	/// </summary>
	public interface IHFCommand
	{
		bool Start(HFUnit unit);
		void Perform(HFUnit unit);
		void Abort(HFUnit unit);
		void End(HFUnit unit);
	}
}
