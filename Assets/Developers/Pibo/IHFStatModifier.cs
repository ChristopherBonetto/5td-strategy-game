using System.Collections.Generic;

/// <summary>
/// Interface for statistics modifier system
/// </summary>
public interface IHFStatModifier
{
	IEnumerable<float> GetFloatAddModifiers(HFStatistics stat);
	IEnumerable<float> GetPctModifiers(HFStatistics stat);
	IEnumerable<int> GetIntAddModifiers(HFStatistics stat);
	IEnumerable<string> GetStringModifiers(HFStatistics stat);
}
