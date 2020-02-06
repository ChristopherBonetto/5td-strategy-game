using System.Collections.Generic;

public interface IHFStatModifier
{
	IEnumerable<float> GetFloatAddModifiers(HFStatistics stat);
	IEnumerable<float> GetPctModifiers(HFStatistics stat);
	IEnumerable<int> GetIntAddModifiers(HFStatistics stat);
}
