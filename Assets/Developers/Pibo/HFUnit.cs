using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum InputType
{
	None = 0,
	Player = 1,
	AI = 2
}

public enum MovementType
{
	Teleport = 0,
	Agent = 1,
	Direct = 2
}

public class HFUnit : MonoBehaviour, IHFDamageable
{
	#region Variables

	/*** Components */

	private Transform m_transform = null;

	private Renderer m_renderer = null;

	private NavMeshAgent m_navAgent = null;

	private Animator m_anim = null;

	/*** Input */

	private HFController m_controller = null;

	public InputType ControllerType { get; protected set; }

	/*** Commands */

	private Queue<IHFCommand> m_pendingCommands = new Queue<IHFCommand>();

	private IHFCommand m_currentCommand;

	private bool m_isCommandComplete;

	/*** Navigation */

	private bool m_isMoving;

	private bool m_isDirectMoving;

	private Vector3 m_directDestination;

	private Vector3 m_lastPosition;

	/*** Selection */

	private bool m_isSelected;

	[SerializeField]
	private Material m_selectedMaterial = null;

	[SerializeField]
	private Material m_unselectedMaterial = null;

	/*** IHFDamageable interface */

	/// <summary>
	/// Max health
	/// </summary>
	public float MaxHealth { get; protected set; }

	/// <summary>
	/// Current health
	/// </summary>
	public float CurrentHealth { get; protected set; }

	/// <summary>
	/// Invincibility flag
	/// </summary>
	public bool CanSufferDamage { get; protected set; }

	/// <summary>
	/// Team ID number
	/// </summary>
	public int Team { get; protected set; }

	/// <summary>
	/// Killed flag
	/// </summary>
	public bool IsKilled { get; protected set; }

	/*** Statistics */

	[SerializeField]
	private HFBaseStats m_baseStats = null;
	private Dictionary<HFStatistics, float> m_stats;

	[SerializeField]
	private HFStatUpgrade[] m_upgrades = new HFStatUpgrade[0];
	private List<IHFStatModifier> m_mods;

	public HFUnitType UnitType => m_baseStats.UnitType;

	#endregion

	#region Core loop

	void Awake()
	{
		m_transform = transform;
		m_renderer = GetComponent<Renderer>();
		m_navAgent = GetComponent<NavMeshAgent>();
		m_anim = GetComponent<Animator>();

		HFHelpers.NullCheck(gameObject, m_baseStats, "base stats");
		HFHelpers.NullCheck(gameObject, m_renderer, "renderer");
		HFHelpers.NullCheck(gameObject, m_navAgent, "navigation agent");
		// #TEMP
		//HFHelpers.NullCheck(gameObject, m_anim, "animator");

		m_mods = new List<IHFStatModifier>();
		UpdateModifiers();

		m_stats = new Dictionary<HFStatistics, float>();
		UpdateStats();

		ResetHealth(true);

		UnPossess();

		m_navAgent.enabled = (m_stats[HFStatistics.Speed] > 0f);
		m_navAgent.speed = m_stats[HFStatistics.Speed];
	}

	void OnEnable()
	{
		HFEventManager.SubscribeTo<GameStates>(HFEventID.OnGameStateChanged, HandleGameStateChange);
	}

	void OnDisable()
	{
		HFEventManager.UnsubscribeFrom<GameStates>(HFEventID.OnGameStateChanged, HandleGameStateChange);
	}

	void Update()
	{
		if (m_isCommandComplete)
		{
			TryStartAction();
		}
		else if (m_currentCommand != null)
		{
			m_currentCommand.Perform();
		}
	}

	#endregion

	#region Statistics

	private void UpdateModifiers()
	{
		m_mods.Clear();
		for (int i = 0; i < m_upgrades.Length; i++)
		{
			if (m_upgrades[i])
			{
				m_mods.Add(m_upgrades[i] as IHFStatModifier);
			}
		}
	}

	private void UpdateStats()
	{
		m_stats.Clear();
		HFStatistics[] allStats = HFHelpers.EnumToArray<HFStatistics>();
		foreach (HFStatistics stat in allStats)
		{
			m_stats.Add(stat, CalculateStat(stat));
		}

		// IHFDamageable update
		ResetHealth(false);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="stat"></param>
	/// <returns></returns>
	private float CalculateStat(HFStatistics stat)
	{
		return (m_baseStats.GetFloat(stat) + GetAddModifiers(stat)) * (1f + GetPctModifiers(stat));
	}

	private float GetAddModifiers(HFStatistics stat)
	{
		float total = 0f;
		foreach (IHFStatModifier mod in m_mods)
		{
			foreach (float add in mod.GetFloatAddModifiers(stat))
			{
				total += add;
			} 
		}
		return total;
	}

	private float GetPctModifiers(HFStatistics stat)
	{
		float totalPct = 0f;
		foreach (IHFStatModifier mod in m_mods)
		{
			foreach (float add in mod.GetPctModifiers(stat))
			{
				totalPct += add;
			}
		}
		return totalPct / 100f;
	}

	private void GainReward()
	{
		if (m_stats[HFStatistics.RewardValue] > 0f)
		{
			HFEventManager.TriggerEvent<float, HFUnit>(HFEventID.GainReward, Mathf.Round(m_stats[HFStatistics.RewardValue]), this);
		}
	}

	#endregion

	#region Input

	public void Possess(HFController controller)
	{
		if (!controller)
		{
			UnPossess();
		}
		else if (controller is HFAIController)
		{
			ControllerType = InputType.AI;
			Team = controller.Team;
		}
		else
		{
			ControllerType = InputType.Player;
			Team = controller.Team;
		}

		m_controller = controller;
	}

	public void UnPossess()
	{
		ControllerType = InputType.None;
		m_controller = null;
		Team = 99;
	}

	private void HandleGameStateChange(GameStates newState)
	{
		if (newState == GameStates.EndLevel && m_baseStats.RewardCondition == HFRewardCondition.Survive && !IsKilled)
		{
			GainReward();
		}
	}

	#endregion

	#region Commands

	/// <summary>
	/// Add a new command to the queue.
	/// </summary>
	/// <param name="newCommand">Command to add</param>
	public void AddCommand(IHFCommand newCommand)
	{
		m_pendingCommands.Enqueue(newCommand);
	}

	/// <summary>
	/// Clear command queue and current command and add a new one.
	/// </summary>
	/// <param name="newCommand">Command to add</param>
	public void SetCommand(IHFCommand newCommand)
	{
		ClearCommands();
		ActionComplete(true);
		AddCommand(newCommand);
	}

	/// <summary>
	/// Clear command queue. Current command will continue executing unless explicitly stopped.
	/// </summary>
	public void ClearCommands()
	{
		m_pendingCommands.Clear();
	}

	/// <summary>
	/// Try and dequeue a command from pending list.
	/// </summary>
	private void TryStartAction()
	{
		IHFCommand nextCommand = GetNextCommand();
		if (nextCommand != null && nextCommand.Start(this))
		{
			m_isCommandComplete = false;
			m_currentCommand = nextCommand;
		}
	}

	/// <summary>
	/// Dequeue a command if any is left.
	/// </summary>
	/// <returns>First command in the list or null if empty</returns>
	protected IHFCommand GetNextCommand()
	{
		return m_pendingCommands.Count > 0 ? m_pendingCommands.Dequeue() : null;
	}

	/// <summary>
	/// Mark a command for clearing.
	/// </summary>
	/// <param name="bTryContinue">Whether to scroll command queue or clear it and stop</param>
	public virtual void ActionComplete(bool bTryContinue)
	{
		m_isCommandComplete = true;

		if (!bTryContinue)
		{
			ClearCommands();

			//m_currentCommand?.Abort();
		}
		
		//m_currentCommand?.End();
		m_currentCommand = null;
	}

	#endregion

	#region Navigation

	public bool WaitForDestination()
	{
		UpdateAnimMoveSpeed();

		bool bAgentMoveComplete = (m_navAgent &&
									m_navAgent.updatePosition &&
									m_navAgent.hasPath &&
									m_navAgent.remainingDistance <= m_navAgent.stoppingDistance &&
									m_navAgent.pathStatus == NavMeshPathStatus.PathComplete);

		bool bDirectMoveComplete = (m_isDirectMoving && DirectMove());

		if (bAgentMoveComplete || bDirectMoveComplete)
		{
			OnDestinationReached();
			return true;
		}
		else
		{
			return false;
		}
	}

	protected virtual void OnDestinationReached()
	{
		m_isMoving = false;
		m_isDirectMoving = false;

		UpdateLocation();
		UpdateAnimState();

		ActionComplete(true);
	}

	private void UpdateLocation()
	{
		// #TODO Snap location
		Debug.Log(gameObject.name + " has reached destination.");
	}

	public bool SetMove(Vector3 position, MovementType moveType)
	{
		if (m_stats[HFStatistics.Speed] <= 0f)
		{
			return false;
		}

		m_isMoving = true;

		m_lastPosition = m_transform.position;

		if (moveType == MovementType.Agent)
		{
			m_isDirectMoving = false;
			m_navAgent.Warp(m_transform.position);
			m_navAgent.ResetPath();
			m_navAgent.updatePosition = true;
			m_navAgent.SetDestination(position);
		}
		else
		{
			m_isDirectMoving = true;
			m_directDestination = position;
			LookAt(position);

			// Teleport is a direct move but will reach destination on next frame and anim speed will be zero
			if (moveType == MovementType.Teleport)
			{
				m_transform.position = position;
				m_lastPosition = position;
				m_navAgent.Warp(position);
			}
		}

		UpdateAnimState();

		return true;
	}

	private bool DirectMove()
	{
		m_transform.position = Vector3.Lerp(m_transform.position, m_directDestination, Time.deltaTime * m_stats[HFStatistics.Speed]);

		return (Vector3.SqrMagnitude(m_directDestination - m_transform.position) < 0.005f);
	}

	private void LookAt(Vector3 position)
	{
		// #TODO Lerp LookAt
		// Maintain view height
		position.y = m_transform.position.y;
		Vector3 direction = position - m_transform.position;
		if (direction != Vector3.zero)
		{
			m_transform.rotation = Quaternion.LookRotation(direction);
		}
	}

	#endregion

	#region Animations

	private void UpdateAnimState()
	{
		if (m_anim)
		{
			m_anim.SetBool("Moving", m_isMoving);
		}
	}

	private void UpdateAnimMoveSpeed()
	{
		if (m_anim)
		{
			// #TODO Update anim move speed scale factor
			m_anim.SetFloat("MoveSpeed", Vector3.Magnitude(m_transform.position - m_lastPosition) / Time.deltaTime * 1f);
			m_lastPosition = m_transform.position;
		}
	}

	#endregion

	#region Interactions

	public virtual bool TryInteraction(HFUnit otherUnit)
	{
		Debug.Log(gameObject.name + " interacts with " + otherUnit.gameObject.name);

		if (otherUnit != this && otherUnit.Team != Team)
		{
			if (otherUnit.UnitType == HFUnitType.Unit)
			{
				otherUnit.TakeDamage(new DamageInfo(m_stats[HFStatistics.UnitDamage]));
			}
			else if (otherUnit.UnitType == HFUnitType.Turret || otherUnit.UnitType == HFUnitType.Castle)
			{
				otherUnit.TakeDamage(new DamageInfo(m_stats[HFStatistics.BuildingDamage]));
			}
		}

		ActionComplete(true);
		return true;
	}

	#endregion

	#region Selection

	public void Select()
	{
		m_isSelected = true;
		if (m_selectedMaterial)
		{
			m_renderer.material = m_selectedMaterial;
		}
	}

	public void Unselect()
	{
		m_isSelected = false;
		if (m_unselectedMaterial)
		{
			m_renderer.material = m_unselectedMaterial;
		}
	}

	#endregion

	#region IHFDamageable interface

	/// <summary>
	/// Should be used to receive damage
	/// </summary>
	/// <param name="info">Struct containing damage event information</param>
	/// <returns>Actual health value decrease amount</returns>
	public float TakeDamage(DamageInfo info)
	{
		float previous = CurrentHealth;
		if (info.Amount > 0f && CanSufferDamage)
		{
			CurrentHealth = Mathf.Max(CurrentHealth - info.Amount, 0f);
		}
		if (CanSufferDamage && CurrentHealth == 0f)
		{
			OnDeath();
		}
		{
			// #TEMP
			if (CanSufferDamage)
			{
				Debug.Log(gameObject.name + " has suffered damage: " + (previous - CurrentHealth).ToString());
			}
			else
			{
				Debug.Log(gameObject.name + " can't suffer damage");
			}
		}
		return previous - CurrentHealth;
	}

	/// <summary>
	/// Should be used to receive heal
	/// </summary>
	/// <param name="info">Struct containing heal event information</param>
	/// <returns>Actual health value increase amount</returns>
	public float Heal(HealInfo info)
	{
		float previous = CurrentHealth;
		if (info.Amount > 0f && CanSufferDamage)
		{
			CurrentHealth = Mathf.Min(CurrentHealth + info.Amount, MaxHealth);
		}
		return CurrentHealth - previous;
	}

	/// <summary>
	/// Handle killed conditions, effects and events
	/// </summary>
	protected virtual void OnDeath()
	{
		IsKilled = true;

		// #TEMP
		transform.localScale = new Vector3(transform.localScale.x, 0.1f, transform.localScale.z);

		if (m_baseStats.RewardCondition == HFRewardCondition.Kill)
		{
			GainReward();
		}
	}

	/// <summary>
	/// Should be called whenever MaxHealth is changed
	/// Current health will be clamped if greater than MaxHealth
	/// </summary>
	/// <param name="bCurrentToMax">True if current health must be reset to maximum, will be unchanged or clamped otherwise</param>
	private void ResetHealth(bool bCurrentToMax)
	{
		MaxHealth = m_stats[HFStatistics.MaxHealth];
		CurrentHealth = (bCurrentToMax ? MaxHealth : Mathf.Min(CurrentHealth, MaxHealth));
		CanSufferDamage = (MaxHealth > 0f);
	}

	#endregion
}



#region Commands inl

public class HFMoveCommand : IHFCommand
{
	private HFUnit m_unit;

	private readonly Vector3 m_destination;
	private readonly MovementType m_moveType;

	public HFMoveCommand(Vector3 destination, MovementType moveType = MovementType.Agent)
	{
		m_destination = destination;
		m_moveType = moveType;
	}

	public bool Start(HFUnit unit)
	{
		m_unit = unit;
		
		return (m_unit != null && m_unit.SetMove(m_destination, m_moveType));
	}

	public void Perform()
	{
		if (m_unit.WaitForDestination())
		{
			End();
		}
	}

	public void Abort()
	{
		End();
	}

	public void End()
	{ }
}

public class HFInteractCommand : IHFCommand
{
	private HFUnit m_unit;

	private readonly HFUnit m_otherUnit;

	public HFInteractCommand(HFUnit otherUnit)
	{
		m_otherUnit = otherUnit;
	}

	public bool Start(HFUnit unit)
	{
		m_unit = unit;

		return (m_unit != null && m_otherUnit != null);
	}

	public void Perform()
	{
		if (m_unit.TryInteraction(m_otherUnit))
		{
			End();
		}
	}

	public void Abort()
	{
		End();
	}

	public void End()
	{ }
}

#endregion
