public enum HFEventID
{
	/// <summary>
	/// <see cref="GameStates"/> preState.
	/// <see cref="GameStates"/> postState.
	/// </summary>
	OnBeforeChangeState,

	/// <summary>
	/// <see cref="GameStates"/> newState.
	/// </summary>
	OnGameStateChanged,

    /// <summary>
    /// <see cref="HFLevelInfoSO"/> current level selected.
    /// </summary>
    OnInitializeLevel,

	/// <summary>
	/// 
	/// </summary>
	OnLevelReady,

	/// <summary>
	/// <see cref="bool"/> is win?
	/// </summary>
	OnEndLevel,

    /// <summary>
    /// <see cref="int"/> reward value.
    /// <see cref="HF.HFUnit"/> ostile unit.
    /// </summary>
    GainReward,

	/// <summary>
	/// <see cref="bool"/> freeze / unfreeze. 
	/// </summary>
	OnPauseMode,


	/*
	 *--------------------------------------- 
	 * Units
	 * ---------------------------------------
	 */

	/// <summary>
	/// <see cref="HF.HFUnit"/> unit.
	/// </summary>
	OnUnitDeath,

	/// <summary>
	/// <see cref="HF.HFUnit"/> unit.
	/// <see cref="int"/> team.
	/// </summary>
	OnUnitSelected,

	/// <summary>
	/// <see cref="HF.HFUnit"/> unit.
	/// <see cref="int"/> team.
	/// </summary>
	OnUnitSpecialized,

	/// <summary>
	/// <see cref="HF.HFUnit"/> unit.
	/// <see cref="int"/> team.
	/// </summary>
	OnUnitUpgraded,

	/// <summary>
	/// <see cref="int"/> team.
	/// <see cref="HFBaseStats"/> List<HFBaseStats>.
	/// </summary>
	OnUnitsPossessed,


	/*
	 *--------------------------------------- 
	 * Wave
	 * ---------------------------------------
	 */

	OnWaveBeginned,

	/// <summary>
	/// <see cref="int"/> wave index.
	/// <see cref="int"/> waves count.
	/// </summary>
	OnWaveIndexUpdate,

	/// <summary>
	/// 
	/// </summary>
	OnWaveCleared,

	/// <summary>
	/// 
	/// </summary>
	OnWaveEnded,

    /// <summary>
    /// <see cref="HF.HFUnit"/> unit.
    /// </summary>
    OnFinishedLoadEvents,

	/*
	 *--------------------------------------- 
	 * Tutorial
	 * ---------------------------------------
	 */

	/// <summary>
	/// <see cref="TutorialID"/> id.
	/// </summary>
	OnTutorialQuestCompleted,

	/// <summary>
	/// <see cref="TutorialID"/> id.
	/// </summary>
	OnTutorialQuestOn,
}
