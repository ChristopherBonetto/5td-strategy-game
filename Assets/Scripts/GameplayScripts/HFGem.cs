using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Types;

public class HFGem : MonoBehaviour, IHFTutorial
{
    [SerializeField] private int m_amount;
    public GameEventData Event;
    public GameEventData InitEvent;

    public TutorialID TutorialID { get; set; } = TutorialID.Reposition_Turret;

    private void Awake()
    {
        InitEvent.AddListener(this);
        Event.AddListener(this);
    }

    private void OnDestroy()
    {
        InitEvent.RemoveListener(this);
        Event.RemoveListener(this);
    }

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

    public void Reset()
    {

    }

    public void OnGlobalInitialization()
    {
        this.gameObject.SetActive(false);
    }

    public void OnStepInitialization()
    {
        this.gameObject.SetActive(true);
    }

    public void OnStepCompleted()
    {
    }
}
