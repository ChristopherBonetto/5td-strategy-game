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

	OnWaveBeginned,
	/// <summary>
	/// 1) <see cref="HF.Refactoring.HFWaveController.WaveIndex"/> int,
	/// 2) <see cref="HF.Refactoring.HFWaveCollection.GetWaves().Count"/> int
	/// </summary>
	OnWaveIndexUpdate,
	OnWaveCleared,
	OnWaveEnded,
    /// <summary>
    /// HFUnit	unit
    /// </summary>
    OnFinishedLoadEvents,

	// UI

	OnTutorialQuestCompleted,
}
