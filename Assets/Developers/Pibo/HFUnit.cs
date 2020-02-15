using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace HF
{
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

		/*** Statistics */

		[Space]
		[Header("Statistics")]

		[SerializeField]
		private HFBaseStats m_baseStats = null;
		private Dictionary<HFStatistics, float> m_stats;

		[SerializeField]
		private HFStatUpgrade[] m_upgrades = new HFStatUpgrade[0];
		private List<IHFStatModifier> m_mods;

		public HFUnitType UnitType => m_baseStats.UnitType;

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

		/*** Attack */

		[Space]
		[Header("Attack")]

		[SerializeField]
		private HFBullet m_bulletPrefab = null;

		private LayerMask m_unitLayer = 1 << 0;

		private Collider m_targetCollider;

		[SerializeField]
		private Transform m_spawnPoint = null;

		private HFUnit m_targetEnemy;

		private float m_lastAttackTime;

		/*** Selection */

		[Space]
		[Header("Selection")]

		[SerializeField]
		private Material m_selectedMaterial = null;

		private Material m_unselectedMaterial = null;

		private bool m_isSelected;

		/*** IHFDamageable interface */

		/// <summary>
		/// Max health
		/// </summary>
		public float MaxHealth => m_stats[HFStatistics.MaxHealth];

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

		#endregion

		#region Core loop

		void Awake()
		{
			m_transform = transform;
			m_renderer = GetComponentInChildren<Renderer>();
			m_navAgent = GetComponent<NavMeshAgent>();
			m_anim = GetComponent<Animator>();

			HFHelpers.NullCheck(gameObject, m_baseStats, "base stats");
			HFHelpers.NullCheck(gameObject, m_renderer, "renderer");
			HFHelpers.NullCheck(gameObject, m_navAgent, "navigation agent");
			// #TEMP
			//HFHelpers.NullCheck(gameObject, m_anim, "animator");

			m_stats = new Dictionary<HFStatistics, float>();
			m_mods = new List<IHFStatModifier>();
			UpdateStats();

			ResetHealth(true, true);

			UnPossess();
		}

		void OnEnable()
		{
			HFEventManager.SubscribeTo<GameStates>(HFEventID.OnGameStateChanged, HandleGameStateChange);
		}

		void Start()
		{
			HFEventManager.TriggerEvent(HFEventID.OnRequestNewBehaviour, WaveSystem.RequestType.Pre);
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
			else
			{
				RefreshCommands();
			}
		}

		#endregion

		#region Statistics

		public void SetStats(HFBaseStats newStats)
		{
			m_baseStats = newStats;
			UpdateStats();
		}

		private void UpdateStats()
		{
			UpdateModifiers();

			m_stats.Clear();
			HFStatistics[] allStats = HFHelpers.EnumToArray<HFStatistics>();
			foreach (HFStatistics stat in allStats)
			{
				m_stats.Add(stat, CalculateStat(stat));
			}

			// IHFDamageable update
			ResetHealth(false, false);

			// Navigation update
			m_navAgent.enabled = (m_stats[HFStatistics.Speed] > 0f);
			m_navAgent.speed = m_stats[HFStatistics.Speed];
		}

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

		/// <summary>
		/// Calculate modified value for a given statistic
		/// </summary>
		/// <param name="stat">Statistic type</param>
		/// <returns>Statistic value</returns>
		private float CalculateStat(HFStatistics stat)
		{
			return (m_baseStats.GetFloat(stat) + GetAddModifiers(stat)) * (1f + GetPctModifiers(stat));
		}

		/// <summary>
		/// Calculate all additive modifiers for a given statistic
		/// </summary>
		/// <param name="stat">Statistic type</param>
		/// <returns>Sum of additive modifiers</returns>
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

		/// <summary>
		/// Calculate all percentage modifiers for a given statistic
		/// </summary>
		/// <param name="stat">Statistic type</param>
		/// <returns>Sum of percentage modifiers</returns>
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

		/// <summary>
		/// Assign reward
		/// </summary>
		private void GainReward()
		{
			if (m_stats[HFStatistics.RewardValue] > 0f)
			{
				HFEventManager.TriggerEvent(HFEventID.GainReward, Mathf.Round(m_stats[HFStatistics.RewardValue]), this);
				Debug.Log("Given reward: " + Mathf.Round(m_stats[HFStatistics.RewardValue]) + " by " + gameObject.name);
			}
		}

		#endregion

		#region Input

		public void Possess(HFController controller)
		{
			UnPossess();

			if (!controller)
			{
				return;
			}

			ControllerType = (controller is HFAIController ? InputType.AI : InputType.Player);
			Team = controller.Team;
			m_unselectedMaterial = controller.BaseMaterial;
			m_controller = controller;

			Unselect();
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

		private void RefreshCommands()
		{
			AddCommand(new HFAttackCommand());
			ActionComplete(true);
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
			while (nextCommand != null)
			{
				if (nextCommand.Start(this))
				{
					m_isCommandComplete = false;
					m_currentCommand = nextCommand;
					break;
				}
				nextCommand = GetNextCommand();
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

		#region Attack

		public bool FindNewTarget()
		{
			// Fail if can't deal any damage
			if (m_stats[HFStatistics.BuildingDamage] == 0f && m_stats[HFStatistics.UnitDamage] == 0f)
			{
				return false;
			}

			// #TEMP Acquisition range is one tile unit larger than attack range
			Collider[] colliders = Physics.OverlapSphere(m_transform.position, (m_stats[HFStatistics.AttackRange] + 1f) * HFGameParameters.TileSize, m_unitLayer);

			if (colliders.Length > 0)
			{
				float targetDistance = Mathf.Infinity;
				m_targetCollider = colliders[0];

				for (int i = 0; i < colliders.Length; i++)
				{
					float distanceTest = Vector3.Magnitude(colliders[i].bounds.center - m_transform.position);
					if (distanceTest < targetDistance)
					{
						m_targetCollider = colliders[i];
						targetDistance = distanceTest;
					}
				}

				UpdateAnimState();

				m_targetEnemy = m_targetCollider.gameObject.GetComponent<HFUnit>();

			}

			return (m_targetEnemy != null);
		}

		public void TryAttack()
		{
			if (IsInRange() && m_targetEnemy && m_targetEnemy.enabled)
			{
				// Look at target
				Vector3 direction = (m_targetCollider.transform.position - transform.position);

				// #TODO Move towards target

				// #TODO Rotate shooting mesh towards target
				//Quaternion lookDirection = Quaternion.LookRotation(direction);

				//m_mesh.transform.rotation = Quaternion.Lerp(m_mesh.transform.rotation, lookDirection, Time.deltaTime * 3f);
				//m_mesh.transform.rotation = Quaternion.Euler(0, m_mesh.transform.rotation.eulerAngles.y, 0);

				// Wait for cooldown
				float rateOfFire = m_stats[HFStatistics.AttackRate];
				bool bCanShootAgain = rateOfFire > 0f && (Time.time > m_lastAttackTime + 1f / rateOfFire);

				if (bCanShootAgain)
				{
					float attackDistance = m_stats[HFStatistics.AttackRange];

					if (attackDistance <= 1f)
					{
						MeleeAttack(m_targetEnemy);
						m_lastAttackTime = Time.time;
					}
					else
					{
						// Check if target is within shoot angle and shoot distance
						// If not specified use full width
						float shootAngle = m_stats[HFStatistics.ShootAngle];
						if (shootAngle == 0f)
						{
							shootAngle = 180f;
						}
						// #TODO Only calculate angle based on rotating mesh
						float targetAngle = Vector3.Angle(m_transform.forward, direction);
						float targetDistance = direction.magnitude;

						if (Mathf.Abs(targetAngle) < shootAngle && targetDistance < attackDistance * HFGameParameters.TileSize)
						{
							RangedAttack(m_targetCollider);
							m_lastAttackTime = Time.time;
						}
					}
				}
			}
			else
			{
				// Lost target
				UpdateAnimState();
				m_targetCollider = null;
				m_targetEnemy = null;

				if (m_currentCommand is HFAttackCommand)
				{
					m_currentCommand.End();
				}
			}
		}

		private void MeleeAttack(HFUnit target)
		{
			DamageInfo damageInfo = new DamageInfo(
				m_stats[HFStatistics.UnitDamage],
				m_stats[HFStatistics.BuildingDamage]
				);
			target.TakeDamage(damageInfo);
		}

		private void RangedAttack(Collider target)
		{
			HFBullet bullet = Instantiate(m_bulletPrefab, m_spawnPoint.position, m_spawnPoint.rotation);
			HFBulletParameters bulletParams = new HFBulletParameters(
				this,
				m_stats[HFStatistics.UnitDamage],
				m_stats[HFStatistics.BuildingDamage],
				m_stats[HFStatistics.BulletSpeed]
				);
			bullet.SetParameters(bulletParams);
			bullet.SetTarget(target);
		}

		private bool IsInRange()
		{
			// #TEMP Acquisition range is one tile unit larger than attack range
			return (Vector3.Magnitude(m_targetCollider.bounds.center - m_transform.position) <= (m_stats[HFStatistics.AttackRange] + 1f) * HFGameParameters.TileSize);
		}
		
		#endregion

		#region Animations

		private void UpdateAnimState()
		{
			if (m_anim)
			{
				m_anim.SetBool("Moving", m_isMoving);
				m_anim.SetBool("Fighting", (m_targetEnemy != null));
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
				// #TEMP
				MeleeAttack(otherUnit);
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
			if (!CanSufferDamage)
			{
				Debug.Log(gameObject.name + " can't suffer damage");
				return 0f;
			}

			float previous = CurrentHealth;
			float receivedDamage = (
				UnitType == HFUnitType.Unit
				? info.UnitAmount
				: (
					(UnitType == HFUnitType.Turret || UnitType == HFUnitType.Castle)
					? info.BuildingAmount
					: 0f));

			if (receivedDamage > 0f)
			{
				CurrentHealth = Mathf.Max(previous - receivedDamage, 0f);
			}

			float actualDamage = previous - CurrentHealth;

			Debug.Log(gameObject.name + " has suffered damage: " + actualDamage.ToString());

			if (CurrentHealth == 0f)
			{
				CanSufferDamage = false;
				OnDeath();
			}

			return actualDamage;
		}

		/// <summary>
		/// Should be used to receive heal
		/// </summary>
		/// <param name="info">Struct containing heal event information</param>
		/// <returns>Actual health value increase amount</returns>
		public float Heal(HealInfo info)
		{
			if (!CanSufferDamage)
			{
				Debug.Log(gameObject.name + " can't be healed");
				return 0f;
			}

			float previous = CurrentHealth;

			if (info.Amount > 0f && CanSufferDamage)
			{
				CurrentHealth = Mathf.Min(previous + info.Amount, m_stats[HFStatistics.MaxHealth]);
			}

			float actualHeal = CurrentHealth - previous;

			return actualHeal;
		}

		/// <summary>
		/// Handle killed conditions, effects and events
		/// </summary>
		protected virtual void OnDeath()
		{
			IsKilled = true;

			// #TEMP
			transform.localScale = new Vector3(transform.localScale.x, 0.1f, transform.localScale.z);

			HFEventManager.TriggerEvent(HFEventID.OnRequestNewBehaviour, WaveSystem.RequestType.Post);

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
		private void ResetHealth(bool bCurrentToMax, bool bResetKill = false)
		{
			IsKilled = (bResetKill ? false : IsKilled);

			if (!IsKilled)
			{
				float maxHealth = m_stats[HFStatistics.MaxHealth];
				CurrentHealth = (bCurrentToMax ? maxHealth : Mathf.Min(CurrentHealth, maxHealth));
				CanSufferDamage = (maxHealth > 0f);
			}
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

	public class HFAttackCommand : IHFCommand
	{
		private HFUnit m_unit;

		public HFAttackCommand()
		{ }

		public bool Start(HFUnit unit)
		{
			m_unit = unit;

			return (m_unit != null && m_unit.FindNewTarget());
		}

		public void Perform()
		{
			m_unit.TryAttack();
		}

		public void Abort()
		{
			End();
		}

		public void End()
		{ }
	}

	#endregion
}
