using UnityEngine;

public class HFController : MonoBehaviour
{
	#region Variables
	
	private HFUnit m_currentSelection;

	[SerializeField]
	private HFUnit[] m_possessedUnitsOnStart = new HFUnit[0];

	public int Team = 0;

	#endregion

	#region Core loop

	void Start()
	{
		for (int i = 0; i < m_possessedUnitsOnStart.Length; i++)
		{
			m_possessedUnitsOnStart[i].Possess(this);
		}
	}

	void Update()
	{
		TrySelect();
		TryInteract();
	}

	#endregion

	#region Selection

	/// <summary>
	/// Update unit selection on mouse click
	/// </summary>
	private void TrySelect()
	{
		if (Input.GetMouseButtonDown(0) && !HFUIManager.Instance.IsMouseOverUI())
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit testHit))
			{
				HFUnit hitUnit = testHit.collider.gameObject.GetComponent<HFUnit>();
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
	/// Try unit
	/// </summary>
	private void TryInteract()
	{
		if (m_currentSelection && Input.GetMouseButtonDown(1) && !HFUIManager.Instance.IsMouseOverUI())
		{
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit testHit))
			{
				HFUnit hitUnit = testHit.collider.gameObject.GetComponent<HFUnit>();
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

	#endregion
	
}
