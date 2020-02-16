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
	/// float	rewardValue,
	/// HFUnit	instigatorUnit
	/// </summary>
	GainReward,
    /// <summary>
    /// HFUnit unit
    /// </summary>
    OnRequestNewBehaviour,
}
