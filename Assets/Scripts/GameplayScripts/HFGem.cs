using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Types;

public class HFGem : MonoBehaviour
{
    [SerializeField] private int m_amount;


    private void OnEnable()
    {
        HFEventManager.SubscribeTo<GameStates>(HFEventID.OnGameStateChanged, GameChangedState);

    }
    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom<GameStates>(HFEventID.OnGameStateChanged, GameChangedState);
    }

    public void GameChangedState(GameStates inState)
    {
        if(inState == GameStates.EndLevel || inState == GameStates.WarRoom)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Unit unit = other.gameObject.GetComponent<Unit>();

        if (unit == null) return;

        if (unit.TroopRef.EntityPlayerType == PlayerType.Player)
        {
            transform.DOMove(unit.transform.position, .5f);
            StartCoroutine(PickUpGem());
        }

        IEnumerator PickUpGem()
        {
            transform.DOScale(.5f, .5f);
            yield return new WaitForSeconds(.5f);
            GameController.Instance.AddResources(m_amount);
            transform.DOScale(.5f, .1f);
            this.gameObject.SetActive(false);
        }
    }
}
