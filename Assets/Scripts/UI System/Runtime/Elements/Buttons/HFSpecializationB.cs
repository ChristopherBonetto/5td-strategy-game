using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.EventSystems;
using HF.Refactoring;

public class HFSpecializationB : HFButton, IHFTutorial
{
    public GameEventData Event;
    public bool RightTutorialButton = false;

    [SerializeField]
    private Button m_ButtonComponent;

    [SerializeField]
    private EventTrigger EventTriggerComponent;

    [SerializeField]
    private Image m_Icon;

    [SerializeField]
    private Text m_Cost;
    private int m_CostAmount;

    [SerializeField]
    private Image m_CostIcon;

    [Header("Fade variables")]

    [SerializeField]
    private Image m_FadingImage;

    [SerializeField]
    private float m_fadingAmount;

    [SerializeField]
    private float m_fadingDuration;

    [SerializeField]
    private Text TooltipText;

    public Button ButtonComponent => m_ButtonComponent;
    public Image Icon => m_Icon;
    public Text Cost => m_Cost;
    public Image CostIcon => m_CostIcon;
    public Image FadingImage => m_FadingImage;

    public TutorialID TutorialID { get; set; } = TutorialID.Specialize_Unit;

    private void Awake()
    {
        if (Event != null)
            Event.AddListener(this);
    }

    private void OnDestroy()
    {
        if (Event != null)
            Event.RemoveListener(this);
    }

    protected override void OnEnable()
    {
        HFEventManager.SubscribeTo<int>(HFEventID.OnRewardGained, OnGainReward);
        HFEventManager.SubscribeTo<int>(HFEventID.OnPurchrased, OnPurchrased);
    }

    protected override void OnDisable()
    {
        HFEventManager.UnsubscribeFrom<int>(HFEventID.OnRewardGained, OnGainReward);
        HFEventManager.UnsubscribeFrom<int>(HFEventID.OnPurchrased, OnPurchrased);

        OnCursorExit();
    }

    public void AddListener(Action callback)
    {
        ButtonComponent.onClick.AddListener(() => 
        {
            Debug.Log("Executing Specialization");
            // If m_CostAmount <= manager.instance.money
                callback?.Invoke();
                OnCursorExit();
                // manager.instance.Purchrase(blablabla) --> Trigger event OnPurchrased();
        });
    }

    public void SetToolTipMessage(string message)
    {
        TooltipText.text = message;
    }

    public void RemoveAllListeners()
    {
        ButtonComponent.onClick.RemoveAllListeners();
    }

    public void EnableButton(bool value)
    {
        RemoveAllListeners();

        gameObject.SetActive(value);
    }

    /// <summary>
    /// Set up the button elements.
    /// </summary>
    public void SetUpButton(Sprite sprite, int costAmount)
    {
        SetIcon(sprite);
        SetCost(costAmount);
    }

    /// <summary>
    /// Set the cost text
    /// </summary>
    public void SetCost(int costAmount) 
    {
        m_CostAmount = costAmount;
        Cost.text = costAmount.ToString();
    }

    /// <summary>
    /// Set the icon of the specialization
    /// </summary>
    public void SetIcon(Sprite icon)
    {
        Icon.sprite = icon;
    }

    /// <summary>
    /// Fade in all cost info elements.
    /// </summary>
    public void OnCursorEnter()
    {
        FadingImage.DOFade(m_fadingAmount, m_fadingDuration);
        Cost.DOFade(1, m_fadingDuration);
        CostIcon.DOFade(1, m_fadingDuration);
    }

    /// <summary>
    /// fade out all cost info elements.
    /// </summary>
    public void OnCursorExit()
    {
        FadingImage.DOFade(0, m_fadingDuration);
        Cost.DOFade(0, m_fadingDuration);
        CostIcon.DOFade(0, m_fadingDuration);
    }


    /// <summary>
    /// Event triggered when player gain money
    /// </summary>
    public void OnGainReward(int amount)
    {
        if (amount >= m_CostAmount)
        {
            Cost.color = Color.white;
        }

        Cost.color = Color.red;
    }

    /// <summary>
    /// Event triggered ehrn player gain money
    /// </summary>
    public void OnPurchrased(int amount)
    {
        if (amount >= m_CostAmount)
        {
            Cost.color = Color.white;
        }

        Cost.color = Color.red;
    }

    public void Reset()
    {
        ButtonComponent.enabled = true;
        transform.localScale = Vector3.one;
    }

    public void OnGlobalInitialization()
    {
    }

    public void OnStepInitialization()
    {
        if (RightTutorialButton)
        {
            ButtonComponent.enabled = true;
        }
    }

    public void OnStepCompleted()
    {
        ButtonComponent.enabled = true;
    }
}
