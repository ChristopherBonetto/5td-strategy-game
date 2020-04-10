using System.Collections.Generic;
using UnityEngine;

namespace HF
{
	public class HFController : MonoBehaviour
	{
		#region Variables

		private HFUnit m_currentSelection;

		[SerializeField]
		private HFUnit m_unitPrefab = null;

		[SerializeField]
		private HFUnit[] m_possessedUnitsOnStart = new HFUnit[0];

		private List<HFUnit> m_possessedUnits = new List<HFUnit>();

		public int Team = 0;

		public Material BaseMaterial;

		[SerializeField]
		private HFSpawnPoint m_respawnPoint = null;

        private List<HFBaseStats> m_respawnUnits = new List<HFBaseStats>();
        private float m_respawnTimer;

		#endregion

		#region Core loop

		void OnEnable()
		{
			HFEventManager.SubscribeTo(HFEventID.OnLevelReady, ReceiveLevelReady);
			HFEventManager.SubscribeTo<GameStates, GameStates>(HFEventID.OnBeforeChangeState, HandlePreGameStateChange);
			HFEventManager.SubscribeTo<GameStates>(HFEventID.OnGameStateChanged, HandleGameStateChange);

            if (Team == HFGameParameters.PlayerTeam)
			{
                HFEventManager.SubscribeTo<HF.HFUnit>(HFEventID.OnUnitDeath, ReceiveUnitDeath);
			}
		}

		void OnDisable()
		{
			HFEventManager.UnsubscribeFrom(HFEventID.OnLevelReady, ReceiveLevelReady);
			HFEventManager.UnsubscribeFrom<GameStates, GameStates>(HFEventID.OnBeforeChangeState, HandlePreGameStateChange);
			HFEventManager.UnsubscribeFrom<GameStates>(HFEventID.OnGameStateChanged, HandleGameStateChange);

            if(Team == HFGameParameters.PlayerTeam)
			{
                HFEventManager.UnsubscribeFrom<HF.HFUnit>(HFEventID.OnUnitDeath, ReceiveUnitDeath);
			}
        }

		protected virtual void Update()
		{
			TrySelect();
			TryInteract();
			TryCarry();
            TryRespawn();
		}

		private void ReceiveLevelReady()
		{
			for (int i = 0; i < m_possessedUnitsOnStart.Length; i++)
			{
				if (m_possessedUnitsOnStart[i])
				{
					m_possessedUnitsOnStart[i].Possess(this);
					m_possessedUnits.Add(m_possessedUnitsOnStart[i]);
				}
			}
		}

		private void HandlePreGameStateChange(GameStates oldState, GameStates newState)
		{
		}

		private void HandleGameStateChange(GameStates newState)
		{
			if (newState == GameStates.EndLevel)
			{
				for (int i = 0; i < m_possessedUnits.Count; i++)
				{
					if (m_possessedUnits[i])
					{
						m_possessedUnits[i].UnPossess();
					}
				}
			}
		}

		#endregion

		#region Selection

		/// <summary>
		/// Update unit selection on mouse click
		/// </summary>
		private void TrySelect()
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit testHit))
				{
					HFUnit hitUnit = testHit.collider.gameObject.GetComponentInParent<HFUnit>();
					if (hitUnit && hitUnit.ControllerType == InputType.Player)
					{
						if (m_currentSelection)
						{
							m_currentSelection.Unselect();
						}
						hitUnit.Select();
						m_currentSelection = hitUnit;
					}
					else
					{
						if (m_currentSelection)
						{
							m_currentSelection.Unselect();
						}
						m_currentSelection = null;
					}
				}
			}
		}

		#endregion

		#region Interaction

		/// <summary>
		/// Try interact with unit
		/// </summary>
		private void TryInteract()
		{
			if (m_currentSelection && Input.GetMouseButtonDown(1))
			{
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit testHit))
				{
					HFUnit hitUnit = testHit.collider.gameObject.GetComponentInParent<HFUnit>();
					if (hitUnit)
					{
						m_currentSelection.SetCommand(new HFInteractCommand(hitUnit));
					}
					else
					{
						m_currentSelection.SetCommand(new HFMoveCommand(testHit.point));
					}
				}
			}
		}

		private void TryCarry()
		{
			if (m_currentSelection && Input.GetKeyDown(KeyCode.C))
			{
				m_currentSelection.CarryAction();
			}
		}

		#endregion

		#region Spawn

		public virtual HFUnit SpawnUnit(HFBaseStats stats, HFSpawnPoint spawnPoint)
		{
			HFUnit newUnit = Instantiate(m_unitPrefab, spawnPoint.SpawnPosition, spawnPoint.SpawnRotation);
			newUnit.Specialize(stats);
			newUnit.Possess(this);
			m_possessedUnits.Add(newUnit);

			return newUnit;
		}

        private void ReceiveUnitDeath(HFUnit inUnit)
		{
			if (inUnit && inUnit.ControllerType == HF.InputType.Player && inUnit.UnitType == HFUnitType.Unit)
			{
				m_respawnUnits.Add(inUnit.BaseStats);
			}
        }

        private void TryRespawn()
        {
            if(m_respawnUnits.Count > 0 && RespawnTimer(m_respawnUnits[0].GetFloat(HFStatistics.UnitRespawnDelay)))
            {
                SpawnUnit(m_respawnUnits[0], m_respawnPoint);
                m_respawnUnits.RemoveAt(0);
                m_respawnTimer = 0f;
            }
        }

        private bool RespawnTimer(float inDestinationTime)
        {
            m_respawnTimer += Time.deltaTime;

			return (m_respawnTimer >= inDestinationTime);
        }

		#endregion
	}
}
