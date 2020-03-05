using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    OnGenericUnitDeath,
    /// <summary>
    /// HFUnit	unit
    /// </summary>
    OnPlayerUnitDeath,

	/// <summary>
	/// int current wave,
	/// int total waves,
	/// </summary>
    OnCallNextWave,
}
