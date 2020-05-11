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

	public class HFUnit : MonoBehaviour, IHFDamageable, IHFTargetable
	{
		public bool Updaiting { get; set; } = true;

		#region Variables

		/*** Components */

		private Transform m_transform = null;

		private Renderer[] m_renderers = new Renderer[0];

		public NavMeshAgent m_navAgent { get; private set; } = null;

		private NavMeshObstacle m_navObstacle = null;

		private Animator m_anim = null;

		private List<Collider> m_colliders = new List<Collider>();
		public List<Collider> Colliders => m_colliders;

		/*** Statistics */

		[Space]
		[Header("Statistics")]

		[SerializeField]
		private HFBaseStats m_initialStats = null;
		public HFBaseStats InitialStats => m_initialStats; // #TEMP

		[SerializeField]
		private HFBaseStats[] m_Specializations = null;
		public HFBaseStats[] Specializations => m_Specializations; // #TEMP
		public bool CanBeSpecialize => m_Specializations != null && m_Specializations.Length > 0;

		private HFBaseStats m_baseStats = null;
        public HFBaseStats BaseStats => m_baseStats; // #TEMP

		private Dictionary<HFStatistics, float> m_stats;
		private Dictionary<HFStatistics, string> m_stringStats;

		private List<HFStatUpgrade> m_upgrades = new List<HFStatUpgrade>();
		private List<IHFStatModifier> m_mods;

		public HFUnitType UnitType => m_baseStats.UnitType;

		public int CurrentLevel { get; protected set; }

		/*** Input */

		private HFController m_controller = null;

		public InputType ControllerType { get; protected set; }

		/*** Commands */

		[Space]
		[Header("Commands")]

		[SerializeField]
		private float m_refreshDelay = 0.25f;

		private float m_lastRefresh;

		private bool m_shouldSkipRefresh;

		private Queue<IHFCommand> m_pendingCommands = new Queue<IHFCommand>();

		private IHFCommand m_startCommand;

		private IHFCommand m_currentCommand;

		private bool m_isCommandComplete;

		/*** Navigation */

		private bool m_isMoving;

		private bool m_isDirectMoving;

		private Vector3 m_currentDestination;

		private Vector3 m_lastPosition;

		/*** Soldiers */

		private List<HFSoldier> m_soldiers = new List<HFSoldier>();

		private int m_activeSoldiersCount;

		private float m_respawnTimer;

		private bool m_needsRespawn;

		/*** Carry */

		[SerializeField]
		private float m_carriedScale = 0.2f;

		private HFUnit m_carriedTower;

		private bool m_isCarried;

		/*** Attack */

		[Space]
		[Header("Attack")]

		[SerializeField]
		private HFBullet m_bulletPrefab = null;

		private LayerMask m_unitLayer = 1 << 0;

		private HFUnit m_targetEnemy;

		private float m_lastAttackTime;

		/*** Selection */

		[Space]
		[Header("Selection")]

		[SerializeField]
		private Material m_selectedMaterial = null;

		private Material m_unselectedMaterial = null;

		private bool m_isSelected;

		/*** IHFTargetable interface */

		public Vector3 Position => m_transform.position.SnapLocation();

		/*** IHFDamageable interface */

		/// <summary>
		/// Max health
		/// </summary>
		public float MaxHealth => m_stats[HFStatistics.MaxHealth] * m_baseStats.SoldiersPerUnit;

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

		/// <summary>
		/// id it unit specialized?
		/// </summary>
		public bool IsSpecialized { get; protected set; }

        private HFIEvent3D m_interfaceSound3D;

        #endregion

        #region Core loop

        void Awake()
		{
			m_transform = transform;
			m_renderers = GetComponentsInChildren<Renderer>();
			m_navAgent = GetComponent<NavMeshAgent>();
			m_navObstacle = GetComponent<NavMeshObstacle>();
			m_anim = GetComponent<Animator>();


			HFHelpers.NullCheck(gameObject, m_initialStats, "initial stats");
			HFHelpers.NullCheck(gameObject, m_renderers, "renderers");
			HFHelpers.NullCheck(gameObject, m_navAgent, "navigation agent");
			HFHelpers.NullCheck(gameObject, m_navObstacle, "navigation obstacle");
			// #TEMP
			//HFHelpers.NullCheck(gameObject, m_anim, "animator");


			m_stats = new Dictionary<HFStatistics, float>();
			m_stringStats = new Dictionary<HFStatistics, string>();
			m_mods = new List<IHFStatModifier>();

			Initialization();

			UpdateStats();

			ResetHealth(true, true);

			UnPossess();

			m_lastAttackTime = -m_stats[HFStatistics.AttackRate];
			m_lastRefresh = -m_refreshDelay;
		}

		void OnEnable()
		{
			HFEventManager.SubscribeTo<GameStates, GameStates>(HFEventID.OnBeforeChangeState, HandlePreGameStateChange);
			HFEventManager.SubscribeTo<GameStates>(HFEventID.OnGameStateChanged, HandleGameStateChange);
		}

		void OnDisable()
		{
			HFEventManager.UnsubscribeFrom<GameStates, GameStates>(HFEventID.OnBeforeChangeState, HandlePreGameStateChange);
			HFEventManager.UnsubscribeFrom<GameStates>(HFEventID.OnGameStateChanged, HandleGameStateChange);
		}

        void Start()
        {
            m_interfaceSound3D = new HFIAttachPlay3D();
        }

        void Update()
		{
			if (Updaiting)
			{
				if (IsKilled || !m_controller)
				{
					return;
				}

				RefreshCommands();

				if (m_isCommandComplete)
				{
					TryStartAction();
				}
				else if (m_currentCommand != null)
				{
					m_currentCommand.Perform(this);
				}

				if (m_needsRespawn && Team == HFGameParameters.PlayerTeam) //&& !isClashing
				{
					m_respawnTimer += Time.deltaTime;
					if (m_respawnTimer >= m_stats[HFStatistics.SoldierRespawnDelay])
					{
						RespawnSoldier();
					}
				}
			}
		}

		private void HandlePreGameStateChange(GameStates oldState, GameStates newState)
		{
			if (newState == GameStates.EndLevel
				&& m_baseStats.RewardCondition == HFRewardCondition.Survive
				&& !IsKilled
				&& ControllerType == InputType.Player)
				{
					GainReward(Mathf.RoundToInt(m_stats[HFStatistics.RewardValue] * CurrentHealth / MaxHealth));
				}
		}

		private void HandleGameStateChange(GameStates newState)
		{
			if (newState == GameStates.InitializeLevel)
			{
				ResetStats();
			}
		}

		#endregion

		#region Statistics

		public void Initialization()
		{
			m_baseStats = m_initialStats;
			IsSpecialized = false;
		}

		public void ResetStats()
		{
			Specialize(m_initialStats);
			IsSpecialized = false;
		}

		public void Specialize(HFBaseStats newStats)
		{
			if (newStats == null)
			{
				Debug.LogWarning("Missing new base stats for specialization on unit " + gameObject.name);
				return;
			}

			// Set base stats
			m_baseStats = newStats;
			CurrentLevel = 0;
			m_upgrades.Clear();

			LoadVisuals();

			Upgrade();

			ResetHealth(true, false);

			HFEventManager.TriggerEvent(HFEventID.OnUnitSpecialized, this, Team);

			IsSpecialized = true;
		}

		public void Upgrade()
		{
			if (CanUpgrade())
			{
				CurrentLevel++;

				// Upgrade stats
				foreach (HFStatUpgrade upgrade in m_baseStats.Levels[CurrentLevel - 1].List)
				{
					m_upgrades.Add(upgrade);
				}
				UpdateStats();

				UpdateVisuals();

				HFEventManager.TriggerEvent(HFEventID.OnUnitUpgraded, this, Team);

				if (Team == HFGameParameters.PlayerTeam)
					HFEventManager.TriggerEvent<TutorialID>(HFEventID.OnTutorialQuestCompleted, TutorialID.Upgrade_Unit);
			}
		}

		private void LoadVisuals()
		{
			HFUnitVisuals visualsPrefab = m_baseStats.Visuals;

			if (!visualsPrefab)
			{
				Debug.LogWarning("No base visuals for stats " + m_baseStats.name);
				return;
			}

			// #TEMP Disable placeholder visuals
			Transform mesh = m_transform.Find("PlaceHolder");
			if (mesh != null)
			{
				DestroyImmediate(mesh.gameObject);
			}

			SpawnUnits(visualsPrefab);
		}

		private void UpdateVisuals()
		{
			foreach (HFSoldier soldier in m_soldiers)
			{
				soldier.SetVisualsLevel(CurrentLevel);

				// Update colliders for targets
				m_colliders.Add(soldier.Target);

				// Update renderers for selection
				m_renderers = GetComponentsInChildren<Renderer>();
			}
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

			m_stringStats.Clear();
			foreach (HFStatistics stat in allStats)
			{
				m_stringStats.Add(stat, ParseStat(stat));
			}

			// IHFDamageable update
			ResetHealth(false, false);

			// Navigation update
			float agentSize = HFGameParameters.TileSize;
			m_navAgent.enabled = (m_stats[HFStatistics.Speed] > 0f);
			if (m_navAgent.enabled)
			{
				m_navAgent.speed = m_stats[HFStatistics.Speed];
				m_navAgent.radius = agentSize;
			}
			m_navObstacle.enabled = !m_navAgent.enabled;
			if (m_navObstacle.enabled)
			{
				m_navObstacle.size = new Vector3(agentSize, agentSize, agentSize);
				m_navObstacle.center = 0.5f * new Vector3(0f, m_navObstacle.size.y, 0f);
			}
		}

		private void UpdateModifiers()
		{
			m_mods.Clear();
			for (int i = 0; i < m_upgrades.Count; i++)
			{
				if (m_upgrades[i])
				{
					m_mods.Add(m_upgrades[i] as IHFStatModifier);
				}
			}
		}

		/// <summary>
		/// Calculate modified value for a given float statistic
		/// </summary>
		/// <param name="stat">Statistic name</param>
		/// <returns>Statistic value</returns>
		private float CalculateStat(HFStatistics stat)
		{
			return (m_baseStats.GetFloat(stat) + GetAddModifiers(stat)) * (1f + GetPctModifiers(stat));
		}

		/// <summary>
		/// Parse modified value for a given string statistic
		/// </summary>
		/// <param name="stat">Statistic name</param>
		/// <returns>Statistic value</returns>
		private string ParseStat(HFStatistics stat)
		{
			return GetStringModifiers(stat);
		}

		/// <summary>
		/// Calculate all additive modifiers for a given statistic
		/// </summary>
		/// <param name="stat">Statistic name</param>
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
		/// <param name="stat">Statistic name</param>
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
		/// Parse all string modifiers for a given statistic
		/// </summary>
		/// <param name="stat">Statistic name</param>
		/// <returns>Final value</returns>
		private string GetStringModifiers(HFStatistics stat)
		{
			string value = m_baseStats.GetString(stat);
			foreach (IHFStatModifier mod in m_mods)
			{
				foreach (string newValue in mod.GetStringModifiers(stat))
				{
					value = newValue;
				}
			}
			return value;
		}

		/// <summary>
		/// Assign reward
		/// </summary>
		private void GainReward(int value)
		{
			if (value > 0)
			{
				HFEventManager.TriggerEvent(HFEventID.GainReward, value, this);
			}
		}

		public float GetStat(HFStatistics stat)
		{
			return (m_stats.ContainsKey(stat) ? m_stats[stat] : 0f);
		}

		public string GetStringStat(HFStatistics stat)
		{
			return (m_stringStats.ContainsKey(stat) ? m_stringStats[stat] : "");
		}

		public int GetMaxLevel()
		{
			return m_baseStats.Levels.Length;
		}

		public bool CanUpgrade()
		{
			return (CurrentLevel < m_baseStats.Levels.Length);
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

			if (CurrentLevel == 0)
			{
				Debug.LogError(gameObject.name + " has no initialized stats. InitializeLevel has not been called.");
				return;
			}

			ControllerType = (controller is HFAIController ? InputType.AI : InputType.Player);
			Team = controller.Team;
			m_unselectedMaterial = controller.BaseMaterial;
			m_controller = controller;

			// #TEMP Assign material
			Unselect();
		}

		public void UnPossess()
		{
			ControllerType = InputType.None;
			m_controller = null;
			Team = HFGameParameters.NoTeam;
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
				else
				{
					nextCommand.Abort(this);
				}
				nextCommand = GetNextCommand();
			}
			if (nextCommand == null)
			{
				m_isCommandComplete = false;
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

				//m_currentCommand?.Abort(this);
			}

			//m_currentCommand?.End(this);
			m_currentCommand = null;
		}

		public void SetStartCommand(IHFCommand startCommand)
		{
			m_startCommand = startCommand;
			SetCommand(startCommand);
			m_shouldSkipRefresh = true;
		}

		private void RefreshCommands(bool bImmediate = false)
		{
			if (m_shouldSkipRefresh)
			{
				m_shouldSkipRefresh = false;
				return;
			}

			if (Time.time >= m_lastRefresh + m_refreshDelay || bImmediate)
			{
				m_lastRefresh = Time.time;
				
				if (FindNewTarget())
				{
					SetCommand(new HFAttackCommand());
					if (ControllerType == InputType.AI && m_isMoving)
					{
						PauseMovement();
					}
				}
				else
				{
					ActionComplete(true);
				}
			}
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
			BreakMovement(true);

			ActionComplete(true);
		}

		private void PauseMovement()
		{
			AddCommand(new HFMoveCommand(m_currentDestination, (m_isDirectMoving ? MovementType.Direct : MovementType.Agent)));

			BreakMovement(false);
		}

		private void BreakMovement(bool bStop)
		{
			m_isMoving = !bStop;
			m_isDirectMoving = false;

			if (m_navAgent.enabled)
			{
				m_navAgent.isStopped = true;
			}

			UpdateAnimState();
			UpdateAnimMoveSpeed();
		}

		public bool SetMove(Vector3 destination, MovementType moveType)
		{
			if (m_stats[HFStatistics.Speed] <= 0f)
			{
				return false;
			}
			
			m_isMoving = true;

			m_lastPosition = m_transform.position;
			destination = destination.SnapLocation();
			m_currentDestination = destination;

			if (moveType == MovementType.Agent)
			{
				m_isDirectMoving = false;
				m_navAgent.isStopped = false;
				m_navAgent.Warp(m_transform.position);
				m_navAgent.ResetPath();
				m_navAgent.updatePosition = true;
				m_navAgent.SetDestination(destination);

				if (Team == HFGameParameters.PlayerTeam)
					HFEventManager.TriggerEvent<TutorialID>(HFEventID.OnTutorialQuestCompleted, TutorialID.Move_Unit);
			}
			else // Direct or Teleport
			{
				m_isDirectMoving = true;
				LookAt(destination);

				// Teleport is a direct move but will reach destination on next frame and anim speed will be zero
				if (moveType == MovementType.Teleport)
				{
					m_transform.position = destination;
					m_lastPosition = destination;
					m_navAgent.Warp(destination);
				}
			}

			UpdateAnimState();

			return true;
		}

		private bool DirectMove()
		{
			m_transform.position = Vector3.Lerp(m_transform.position, m_currentDestination, Time.deltaTime * m_stats[HFStatistics.Speed]);

			return (Vector3.SqrMagnitude(m_currentDestination - m_transform.position) < 0.005f);
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

		#region Soldiers

		private void SpawnUnits(HFUnitVisuals baseVisuals)
		{
			GameObject m_soldierPrefab = baseVisuals.gameObject;
			HFSoldier newSoldier;

			void SpawnSoldier(Vector3 localPosition, float phase)
			{
				newSoldier = Instantiate(m_soldierPrefab, m_transform).AddComponent<HFSoldier>();
				newSoldier.transform.localPosition = localPosition;
				newSoldier.Init(this, phase);
				m_soldiers.Add(newSoldier);
				m_activeSoldiersCount++;

				Debug.Log(newSoldier);
				Debug.Log(newSoldier.transform.position);
			}

			int soldierTotal = m_baseStats.SoldiersPerUnit;

			if (soldierTotal == 1)
			{
				SpawnSoldier(Vector3.zero, 0f);
				return;
			}

			// If there are multiple soldiers, they are spawned in two rows

			float tileSize = HFGameParameters.TileSize;
			int soldiersPerRow = Mathf.RoundToInt(soldierTotal / 2);
			float soldierSpacerOnRow = tileSize / (float)soldiersPerRow;
			float soldierSpacerOnColumn = tileSize / 2f;
			float phaseSpacer = 1f / soldierTotal;

			float startX = (-tileSize + soldierSpacerOnRow) / 2f;
			float startZ = -tileSize * 0.25f;
			Vector3 localSpawnPosition = new Vector3(startX, 0f, startZ);

			for (int i = 0; i < 2; i++)
			{
				localSpawnPosition.x = startX;
				localSpawnPosition.z += soldierSpacerOnColumn * i;
				for (int j = 0; j < soldiersPerRow; j++)
				{
					// Spawn if less than limit and not already active in that position
					if (m_soldiers.Count < soldierTotal && j + (i * soldiersPerRow) >= m_activeSoldiersCount)
					{
						localSpawnPosition.x += soldierSpacerOnRow * j;
						SpawnSoldier(localSpawnPosition, j + (i * soldiersPerRow) * phaseSpacer);
					}
				}
			}
		}

		private void DisableSoldier()
		{
			for (int i = 0; i < m_soldiers.Count; i++)
			{
				if (m_soldiers[i].gameObject.activeSelf)
				{
					m_soldiers[i].gameObject.SetActive(false);
					m_activeSoldiersCount--;
					break;
				}
			}

			m_respawnTimer = 0f;
			m_needsRespawn = true;
		}

		private void RespawnSoldier()
		{
			for (int i = 0; i < m_soldiers.Count; i++)
			{
				if (!m_soldiers[i].gameObject.activeSelf)
				{
					m_soldiers[i].gameObject.SetActive(true);
					m_activeSoldiersCount++;
					break;
				}
			}

			bool bContinueRespawn = false;
			for (int i = 0; i < m_soldiers.Count; i++)
			{
				if (!m_soldiers[i].gameObject.activeSelf)
				{
					bContinueRespawn = true;
					break;
				}
			}

			m_respawnTimer = 0f;
			m_needsRespawn = bContinueRespawn;
		}

		#endregion

		#region Carry

		private bool CanCarry()
		{
			return (m_carriedTower == null
				&& UnitType == HFUnitType.Unit
				&& m_stats[HFStatistics.CarryCapacity] > 0f
				&& !IsKilled
			);
		}

		public bool CanBeCarried()
		{
			return (!m_isCarried
				&& UnitType == HFUnitType.Turret
				&& m_stats[HFStatistics.Weight] > 0f
				&& !IsKilled
			);
		}

		public void SetCarried(bool bInIsCarried)
		{
			Unselect();

			m_isCarried = bInIsCarried;

			// Set activation

			enabled = !bInIsCarried;

			foreach (HFSoldier soldier in m_soldiers)
			{
				soldier.Target.enabled = !bInIsCarried;
			}

			m_navObstacle.enabled = !bInIsCarried;

			// Set transform

			float scaleFactor = (bInIsCarried ? m_carriedScale : 1f);
			Vector3 newScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
			m_transform.localScale = newScale;

			Vector3 newPosition = m_transform.position;
			newPosition.y += 2f * (bInIsCarried ? 1f : -1f);
			m_transform.position = (newPosition);
			m_transform.position.SnapLocation();

			m_transform.rotation = (bInIsCarried ? m_transform.rotation : Quaternion.identity);
		}

		public void CarryAction()
		{
			if (!m_carriedTower)
			{
				if (!CanCarry())
				{
					return;
				}

				Collider[] colliders = Physics.OverlapSphere(m_transform.position, HFGameParameters.TileSize, m_unitLayer);
				if (colliders.Length > 0)
				{
					float targetDistance = Mathf.Infinity;
					HFUnit targetTurret = null;

					for (int i = 0; i < colliders.Length; i++)
					{
						Collider testCollider = colliders[i];

						// Ignore if alreaady have a closer target
						float testDistance = Vector3.Magnitude(testCollider.gameObject.transform.position - m_soldiers[0].BulletSpawn.position);
						if (testDistance >= targetDistance)
						{
							continue;
						}

						// Ignore self
						if (m_colliders.Contains(testCollider))
						{
							continue;
						}

						// Check interaction conditions
						HFUnit testTurret = testCollider.gameObject.GetComponentInParent<HFUnit>();
						float currentCarry = m_stats[HFStatistics.CarryCapacity] * m_activeSoldiersCount;
						if (!testTurret
							|| !testTurret.CanBeCarried()
							|| testTurret.Team != Team
							|| currentCarry < testTurret.GetStat(HFStatistics.Weight))
						{
							continue;
						}

						targetTurret = testTurret;
						targetDistance = testDistance;
					}

					TryCarry(targetTurret);
				}
			}
			else
			{
				TryDrop();
			}
		}

		public bool TryCarry(HFUnit tower)
		{
			bool bCanCarry = (tower != null); //!IsClashing

			if (bCanCarry)
			{
				m_carriedTower = tower;
				m_carriedTower.SetCarried(true);
				m_carriedTower.transform.parent = m_transform;
				HFEventManager.TriggerEvent<TutorialID>(HFEventID.OnTutorialQuestCompleted, TutorialID.Carry_Turret);
			}

			return bCanCarry;
		}

		public bool TryDrop()
		{
			bool bCanDrop = (m_carriedTower != null); //!targetTile.HasTower

			if (bCanDrop)
			{
				m_carriedTower.transform.parent = null;
				m_carriedTower.SetCarried(false);
				m_carriedTower = null;
				HFEventManager.TriggerEvent<TutorialID>(HFEventID.OnTutorialQuestCompleted, TutorialID.Reposition_Turret);
			}

			return bCanDrop;
		}

		#endregion

		#region Attack

		public bool FindNewTarget()
		{
			if (m_soldiers.Count > 0)
			{

				m_targetEnemy = null;

				// Fail if carrying a tower
				if (m_carriedTower)
				{
					return false;
				}

				float unitDamage = m_stats[HFStatistics.UnitDamage];
				float buildingDamage = m_stats[HFStatistics.BuildingDamage];

				// Fail if can't deal any damage
				if (unitDamage == 0f && buildingDamage == 0f)
				{
					return false;
				}

				// #TEMP Acquisition range could be larger than attack range
				Collider[] colliders = Physics.OverlapSphere(m_transform.position, (m_stats[HFStatistics.AttackRange] + 0f) * HFGameParameters.TileSize, m_unitLayer);

				if (colliders.Length > 0)
				{
					float targetDistance = Mathf.Infinity;

					for (int i = 0; i < colliders.Length; i++)
					{
						Collider testCollider = colliders[i];

						// Ignore if alreaady have a closer target
						float testDistance = Vector3.Magnitude(testCollider.gameObject.transform.position - m_soldiers[0].BulletSpawn.position);
						if (testDistance >= targetDistance)
						{
							continue;
						}

						// Ignore self
						if (m_colliders.Contains(testCollider))
						{
							continue;
						}

						// Ignore useless attacks and disable friendly fire
						HFUnit testEnemy = testCollider.gameObject.GetComponentInParent<HFUnit>();
						if (!testEnemy
							|| testEnemy.IsKilled
							|| testEnemy.Team == Team
							|| !testEnemy.CanSufferDamage
							|| (testEnemy.UnitType == HFUnitType.Unit && unitDamage == 0f)
							|| (testEnemy.UnitType == HFUnitType.Castle && buildingDamage == 0f))
						{
							continue;
						}

						m_targetEnemy = testEnemy;
						targetDistance = testDistance;
					}
				}

				UpdateAnimState();
			}

			return (m_targetEnemy != null);
		}

		public void AttackAction()
		{
			if (m_targetEnemy
				&& IsInRange()
				&& m_targetEnemy.enabled
				&& !m_targetEnemy.IsKilled
				&& !m_carriedTower)
			{
				Vector3 direction = (m_targetEnemy.transform.position - m_transform.position);
				Transform rotatingMesh = (m_soldiers[0].LoadedVisuals.UsesPivot ? m_soldiers[0].LoadedVisuals.Pivot : m_transform);

				// Look at target
				Quaternion lookDirection = Quaternion.LookRotation(direction);
				rotatingMesh.rotation = Quaternion.Lerp(rotatingMesh.rotation, lookDirection, Time.deltaTime * 3f);
				rotatingMesh.rotation = Quaternion.Euler(0f, rotatingMesh.rotation.eulerAngles.y, 0f);

				// Wait for cooldown
				float rateOfFire = m_stats[HFStatistics.AttackRate];
				bool bCanShootAgain = rateOfFire > 0f && (Time.time > m_lastAttackTime + 1f / rateOfFire);

				if (bCanShootAgain)
				{
					float attackDistance = m_stats[HFStatistics.AttackRange];

					if (attackDistance <= 3f)
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
						float targetAngle = Vector3.Angle(rotatingMesh.forward, direction);
						float targetDistance = direction.magnitude;

						if (Mathf.Abs(targetAngle) < shootAngle && targetDistance < attackDistance * HFGameParameters.TileSize)
						{
							RangedAttack(m_targetEnemy);
							m_lastAttackTime = Time.time;
						}
					}
				}
			}
			else
			{
				// Lost target
				m_targetEnemy = null;
				UpdateAnimState();

				if (m_currentCommand is HFAttackCommand)
				{
					m_currentCommand.End(this);
				}

				ActionComplete(true);
			}
		}

		private void MeleeAttack(HFUnit target)
		{
			DamageInfo damageInfo = new DamageInfo(
				this,
				m_stats[HFStatistics.UnitDamage],
				m_stats[HFStatistics.BuildingDamage]
			);
            PlayUnitSound(m_baseStats.AttackSound);
			target.TakeDamage(damageInfo);
		}

		private void RangedAttack(HFUnit target)
		{
			float speedModifier = Vector3.Magnitude(target.transform.position - m_transform.position) / m_stats[HFStatistics.AttackRange];
			foreach (HFSoldier soldier in m_soldiers)
			{
				if (soldier.gameObject.activeSelf)
				{
					HFBullet bullet = Instantiate(m_bulletPrefab, soldier.BulletSpawn.position, soldier.BulletSpawn.rotation);
					HFBulletParameters bulletParams = new HFBulletParameters(
						this,
						m_stats[HFStatistics.UnitDamage],
						m_stats[HFStatistics.BuildingDamage],
						m_stats[HFStatistics.BulletSpeed] * speedModifier
					);
					bullet.SetParameters(bulletParams);
					bullet.SetTarget(target);
				}
			}
		}

		private bool IsInRange()
		{
			// #TEMP Acquisition range could be larger than attack range
			return (Vector3.Magnitude(m_targetEnemy.transform.position - m_transform.position) <= (m_stats[HFStatistics.AttackRange] + 0f) * HFGameParameters.TileSize);
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

		#region Selection

		public void Select()
		{
			if (Team == HFGameParameters.PlayerTeam)
				HFEventManager.TriggerEvent<TutorialID>(HFEventID.OnTutorialQuestCompleted, TutorialID.Select_Unit);

			HFEventManager.TriggerEvent(HFEventID.OnUnitSelected, this, Team);
			m_isSelected = true;
			if (m_selectedMaterial)
			{
				foreach (Renderer renderer in m_renderers)
				{
					renderer.material = m_selectedMaterial;
				}
			}
		}

		public void Unselect()
		{
			HFEventManager.TriggerEvent(HFEventID.OnUnitSelected, null as HFUnit, Team);
			m_isSelected = false;
			if (m_unselectedMaterial)
			{
				foreach (Renderer renderer in m_renderers)
				{
					renderer.material = m_unselectedMaterial;
				}
			}
		}

		#endregion

		#region IHFTargetable interface

		public bool TryInteraction(HFUnit otherUnit)
		{
			//Debug.Log(gameObject.name + " interacts with " + otherUnit.gameObject.name);

			if (otherUnit != this && otherUnit.Team != Team)
			{
				// #TEMP
				//MeleeAttack(otherUnit);
				AddCommand(otherUnit.GetDefaultInteraction());
			}

			ActionComplete(true);
			return true;
		}

		public IHFCommand GetDefaultInteraction()
		{
			//Debug.Log(gameObject.name + " returns interaction at " + Position.ToString());
			return new HFMoveCommand(Position);
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
				return 0f;
			}

			// Ignore friendly fire
			if (info.Instigator.Team == Team)
			{
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
                PlayUnitSound(m_baseStats.HittedSound);
			}

			float actualDamage = previous - CurrentHealth;

			// Compare current health with single unit's health
			int soldiers = m_activeSoldiersCount - Mathf.CeilToInt(CurrentHealth / m_stats[HFStatistics.MaxHealth]);
			for (int i = 0; i < soldiers; i++)
			{
				DisableSoldier();
			}
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
				CurrentHealth = Mathf.Min(previous + info.Amount, MaxHealth);
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
			OnDestinationReached();
			ActionComplete(false);
            PlayUnitSound(m_baseStats.DeathSound);

// 			m_navAgent.enabled = false;
// 			if (UnitType != HFUnitType.Unit)
// 			{
// 				m_navObstacle.enabled = true;
// 			}

			HFEventManager.TriggerEvent(HFEventID.OnUnitDeath, this);

			if (m_baseStats.RewardCondition == HFRewardCondition.Kill && ControllerType != InputType.Player)
			{
				GainReward(Mathf.RoundToInt(m_stats[HFStatistics.RewardValue]));
			}

			gameObject.SetActive(false);
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
				CurrentHealth = (bCurrentToMax ? MaxHealth : Mathf.Min(CurrentHealth, MaxHealth));
				CanSufferDamage = (MaxHealth > 0f);
			}
		}

        #endregion

        #region Sounds

        void PlayUnitSound(string inEvent)
        {
            HFCustomEvent tempEvent;
            tempEvent = HFSoundManager.Instance.GetFreeEventFromDictionaryKey(inEvent);
            m_interfaceSound3D.AttachAndPlay(this.gameObject, tempEvent);
        }

		#endregion

#if UNITY_EDITOR
		private void OnDrawGizmosSelected()
		{
			UnityEditor.Handles.DrawSolidDisc(transform.position, Vector3.up, m_stats[HFStatistics.AttackRange]);
		}
#endif
	}



	#region Commands inl

	public class HFMoveCommand : IHFCommand
	{
		private readonly Vector3 m_destination;
		private readonly MovementType m_moveType;

		public HFMoveCommand(Vector3 destination, MovementType moveType = MovementType.Agent)
		{
			m_destination = destination;
			m_moveType = moveType;
		}

		public bool Start(HFUnit unit)
		{
			return (unit != null && unit.SetMove(m_destination, m_moveType));
		}

		public void Perform(HFUnit unit)
		{
			if (!unit)
			{
				Abort(unit);
				return;
			}

			if (unit.WaitForDestination())
			{
				End(unit);
			}
		}

		public void Abort(HFUnit unit)
		{
			End(unit);
		}

		public void End(HFUnit unit)
		{ }
	}

	public class HFInteractCommand : IHFCommand
	{
		private readonly HFUnit m_otherUnit;

		public HFInteractCommand(HFUnit otherUnit)
		{
			m_otherUnit = otherUnit;
		}

		public bool Start(HFUnit unit)
		{
			return (unit != null && m_otherUnit != null);
		}

		public void Perform(HFUnit unit)
		{
			if (!unit)
			{
				Abort(unit);
				return;
			}

			if (unit.TryInteraction(m_otherUnit))
			{
				End(unit);
			}
		}

		public void Abort(HFUnit unit)
		{
			End(unit);
		}

		public void End(HFUnit unit)
		{ }
	}

	public class HFAttackCommand : IHFCommand
	{
		private HFUnit m_unit;

		public HFAttackCommand()
		{ }

		public bool Start(HFUnit unit)
		{
			return (unit != null && unit.FindNewTarget());
		}

		public void Perform(HFUnit unit)
		{
			if (!unit)
			{
				Abort(unit);
				return;
			}

			unit.AttackAction();
		}

		public void Abort(HFUnit unit)
		{
			End(unit);
		}

		public void End(HFUnit unit)
		{
			unit.ActionComplete(true);
		}
	}

    #endregion
}
