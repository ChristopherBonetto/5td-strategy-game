using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Types;

public class HFGem : MonoBehaviour, IHFTutorial
{
    [SerializeField] 
    private int m_amount = 5;

    [SerializeField, Tooltip("The scaling factor is a logaritmic function that consider this base as offset")]
    private float m_scalingFactorLogBase = 4;

    public GameEventData Event;
    public GameEventData InitEvent;

    [FMODUnity.EventRef]
    public string GemPickUpSoundPath;
    private HFCustomEvent GemPickUpSoundEvent;

    private HFIEvent3D m_3DSoundInterface;

    public TutorialID TutorialID { get; set; } = TutorialID.Reposition_Turret;

    private void Awake()
    {
        InitEvent.AddListener(this);
        Event.AddListener(this);

        m_3DSoundInterface = new HFIAttachPlay3D();
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
    }

    IEnumerator PickUpGem()
    {

        HFCustomEvent tempEvent = HFSoundManager.Instance.GetFreeEventFromDictionaryKey(GemPickUpSoundPath);

        if(tempEvent != null)
        {
            m_3DSoundInterface.AttachAndPlay(tempEvent, this.gameObject);
        }
        
        yield return new WaitForSeconds(.5f);
        GameController.Instance.AddResources(m_amount);
        this.gameObject.SetActive(false);
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

    public void SetAmount(int amount) 
    {
        m_amount = amount;

        // link logaritmic function visualization: https://www.desmos.com/calculator/auubsajefh 
        float scalingFactor = Mathf.Log(Mathf.Max(m_scalingFactorLogBase, m_amount), m_scalingFactorLogBase);
        transform.localScale = Vector3.one * scalingFactor;
    }
}
