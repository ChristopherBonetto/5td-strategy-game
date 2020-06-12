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
    OnInitializeLevel,	// Call this to pool allies units.

	/// <summary>
	/// no args
	/// </summary>
	OnLevelReady,

	/// <summary>
	/// <see cref="bool"/> is win?
	/// </summary>
	OnEndLevel,

	/// <summary>
	/// <see cref="int"/> reward value.
	/// /// <see cref="bool"/> earned/losed. 
	/// </summary>
	OnGemChanged,

	/// <summary>
	/// <see cref="bool"/> freeze / unfreeze. 
	/// </summary>
	OnPauseMode,

	/// <summary>
	/// <see cref="int"/> total player money
	/// </summary>
	OnRewardGained,

	/// <summary>
	/// <see cref="int"/> total player money
	/// </summary>
	OnPurchrased,


	/*
	 *--------------------------------------- 
	 * Units
	 * ---------------------------------------
	 */

	/// <summary>
	/// <see cref="EntityBehavior"/> unit.
	/// </summary>
	OnEntityDeath,

	/// <summary>
	/// <see cref="EntityBehavior"/> unit.
	/// <see cref="int"/> team.
	/// </summary>
	OnUnitSelected,

	/// <summary>
	/// <see cref="EntityBehavior"/> unit.
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

	/// <summary>
	/// <see cref="EntityBehavior"/> ally troop.
	/// </summary>
	OnUnitFight,

	/// <summary>
	/// no args
	/// </summary>
	OnUnitLift,

	/// <summary>
	/// no args
	/// </summary>
	OnUnitDropBuilding,


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

	/// <summary>
	/// <see cref="string"/> message
	/// </summary>
	OnError,
}
