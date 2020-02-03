using UnityEngine;

public enum InputType
{
	None = 0,
	Player = 1,
	AI = 2
}

public enum RewardCondition
{
	NoReward = 0,
	Kill = 1,
	Survive = 2
}

public class HFUnit : MonoBehaviour
{
	#region Variables

	private HFController m_controller;

	private InputType m_controllerType;

	public InputType ControllerType
	{
		get => m_controllerType;
		private set { m_controllerType = value; }
	}

	private int m_team;

	public int Team => m_team;

	[SerializeField]
	private HFBaseStats m_stats = null;

	#endregion

	#region Core Loop

	void Awake()
	{
		HFHelpers.NullCheck(gameObject, m_stats, "base stats");
	}

	#endregion

	#region Public Interface

	public void Possess(HFController controller)
	{
		if (!controller)
		{
			ControllerType = InputType.None;
		}
		else if (controller is HFAIController)
		{
			ControllerType = InputType.AI;
		}
		else
		{
			ControllerType = InputType.Player;
		}

		m_controller = controller;
	}

	public void UnPossess()
	{
		ControllerType = InputType.None;
		m_controller = null;
	}

	#endregion
}
