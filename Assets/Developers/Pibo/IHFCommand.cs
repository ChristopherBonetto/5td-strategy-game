namespace HF
{
	/// <summary>
	/// Interface for command system
	/// </summary>
	public interface IHFCommand
	{
		bool Start(HFUnit unit);
		void Perform();
		void Abort();
		void End();
	}
}
