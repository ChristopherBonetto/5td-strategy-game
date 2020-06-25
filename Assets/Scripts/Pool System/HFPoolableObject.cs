using UnityEngine;

public class HFPoolableObject : MonoBehaviour
{
	public HFPoolID uniqueID;

	private void OnEnable()
	{
		HFEventManager.SubscribeTo<GameStates>(HFEventID.OnGameStateChanged, TurnOffGameObject);
	}

	private void OnDisable()
	{
		HFEventManager.UnsubscribeFrom<GameStates>(HFEventID.OnGameStateChanged, TurnOffGameObject);
	}

	private void TurnOffGameObject(GameStates states)
	{
		switch (states)
		{
			case GameStates.EndLevel:
				gameObject.SetActive(false);
				break;
			case GameStates.InitializeLevel:
				gameObject.SetActive(false);
				break;
		}
	}
}

