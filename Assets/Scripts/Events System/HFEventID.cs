public enum HFEventID
{
	/// <summary>
	/// GameStates preState,
	/// GameStates postState
	/// </summary>
	OnBeforeChangeState,
	/// <summary>
	/// GameStates newState
	/// </summary>
	OnGameStateChanged,

    /// <summary>
    /// HFLevelInfoSO
    /// </summary>
    OnInitializeLevel,
	/// <summary>
	/// empty
	/// </summary>
	OnLevelReady,
	/// <summary>
	/// bool	winCondition
	/// </summary>
	OnEndLevel,

    /// <summary>
    /// float	rewardValue,
    /// HFUnit	instigatorUnit
    /// </summary>
    GainReward,
    /// <summary>
    /// HFUnit	unit
    /// </summary>
    OnUnitDeath,

	/// <summary>
	/// This one differ from <see cref="HFEventID.OnUnitDeath"/>.
	/// it's used to update the view.
	/// 1) int <see cref="HF.WaveSystem.HFWaveController.CountOfEnemyKilled"/> current enemies killed
	/// 2) int <see cref="HF.WaveSystem.HFWaveReader.GetNumberOfEnemiesInTheWave(HF.WaveSystem.HFWaveModel)"/> total enemies to kill
	/// </summary>
	OnEnemyKilled,
	/// <summary>
	/// 1) int <see cref="HF.WaveSystem.HFWaveController.WaveIndex"/> current wave
	/// 1) int <see cref="HF.WaveSystem.HFWaveReader.GetNumberOfWaves(HF.WaveSystem.HFWaveModel)"/> total waves
	/// </summary>
	OnWaveIndexUpdate,
	OnNewWaveBegin,
	OnWaveEnd,


	/// <summary>
	/// int current wave,
	/// int total waves,
	/// </summary>
    OnCallNextWave,
}
