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
    /// int	rewardValue,
    /// HFUnit	instigatorUnit
    /// </summary>
    GainReward,


	//----------------------------------------------------------------------------------
	// Units
	//----------------------------------------------------------------------------------

    /// <summary>
    /// HFUnit	unit
    /// </summary>
    OnUnitDeath,
	/// <summary>
	/// 1) <see cref="HF.HFUnit"/> unit
	/// 2) <see cref="HF.HFUnit.Team"/> team
	/// </summary>
	OnUnitSelected,
	/// <summary>
	/// 1) <see cref="HF.HFUnit"/> unit
	/// 2) <see cref="HF.HFUnit.Team"/> team
	/// </summary>
	OnUnitSpecialized,
	/// <summary>
	/// 1) <see cref="HF.HFUnit"/> unit
	/// 2) <see cref="HF.HFUnit.Team"/> team
	/// </summary>
	OnUnitUpgraded,
	/// <summary>
	/// 1) <see cref="HF.HFController.Team"/> int
	/// 2) <see cref="HFBaseStats"/> List<HFBaseStats>
	/// </summary>
	OnUnitsPossessed,


	//----------------------------------------------------------------------------------
	// Wave
	//----------------------------------------------------------------------------------

	/// <summary>
	/// This one differ from <see cref="HFEventID.OnUnitDeath"/>.
	/// it's used to update the view.
	/// 1) int <see cref="HF.Refactoring.HFWaveController.CountOfEnemyKilled"/> current enemies killed
	/// 2) int <see cref="HF.WaveSystem.HFWaveReader.GetNumberOfEnemiesInTheWave(HF.WaveSystem.HFWaveModel)"/> total enemies to kill
	/// </summary>
	OnEnemyCountUpdate,
	/// <summary>
	/// 1) int <see cref="HF.Refactoring.HFWaveController.WaveIndex"/> current wave
	/// 1) int <see cref="HF.Refactoring.HFR.GetNumberOfWaves(HF.WaveSystem.HFWaveModel)"/> total waves
	/// </summary>
	OnWaveIndexUpdate,
	OnWaveBeginned,
	OnWaveCleared,

    /// <summary>
    /// HFUnit	unit
    /// </summary>
    OnFinishedLoadEvents
}
