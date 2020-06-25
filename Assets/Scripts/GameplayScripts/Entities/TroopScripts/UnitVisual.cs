using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitVisual : MonoBehaviour
{
    #region Particle Variables
    [SerializeField] private ParticleSystem m_takeDamageEffect;
    public ParticleSystem TakeDamageEffect { get => m_takeDamageEffect; }
    #endregion

    #region Healthbar Variables
    public static List<UnitVisual> Active = new List<UnitVisual>();
    public GameObject Healthbar;
    private Slider HealthbarSlider;
    private RectTransform HealthbarRect;
    private CanvasGroup HealthbarCanvas;

    public float HealthPercentage
    {
        get { return healthPercentage; }
    }

    public float HealthBarWidth
    {
        get { return Length; }
    }

    public float HealthBarHPAlpha
    {
        get { return HPOpacity; }
    }
    public float HealthBarBGAlpha
    {
        get { return BGOpacity; }
    }

    public float HealthBarYOffset
    {
        get { return VerticalOffset; }
    }

    public Color HealthBarColor
    {
        get { return color; }
    }

    [Range(0f, 1f)]
    [SerializeField]
    public float HPOpacity = 1f;
    [Range(0f, 1f)]
    [SerializeField]
    public float BGOpacity = 1f;
    [SerializeField]
    private float VerticalOffset = 2.25f;
    [SerializeField]
    float Length;
    [SerializeField]
    bool ScaleWithMAXHP;
    [SerializeField]
    Color color = Color.green;
    float healthPercentage;

    [SerializeField]
    public GameObject SelectionCircle;

    #endregion

    #region Corpses Variables
    [SerializeField] private HFPoolID m_corpse;
    #endregion


    #region Animator Variables
    [SerializeField] private Animator m_animator;
    private int runLayer;
    private int topLayer;
    public Animator UnitAnimator { get => m_animator; }

    #endregion

    //public Unit unit;
    public Troop troop;
    private void Awake()
    {
        HealthbarSlider = Healthbar.GetComponent<Slider>();
        HealthbarRect = Healthbar.GetComponent<RectTransform>();
        HealthbarCanvas = Healthbar.GetComponent<CanvasGroup>();
        SetHealthBarAlpha(0);
        SelectionCircle.SetActive(false);

      if(m_animator!=null)
        {
            runLayer = m_animator.GetLayerIndex("Bottom Run");
            topLayer = m_animator.GetLayerIndex("Top");
        }
    }

    void OnEnable()
    {
        SetHealthbar(1f); //Reset Healthbar value to its maximum
        Active.Add(this);

        if (m_animator == null) return;
        m_animator.SetLayerWeight(2, 1f);
        m_animator.SetLayerWeight(topLayer, 0);
    }
    void OnDisable()
    {
        Active.Remove(this);
    }
    private void Update()
    {
        if (m_animator != null)
        {
            m_animator.SetLayerWeight(runLayer, troop.Agent.velocity.magnitude);
        }
    }

    #region HealthBar methods

    public void SetHealthbar(float NormalizedPercentage) //Changes the fill of the healthbar based on a provided normalized value;
    {
        //healthPercentage = percentage;
        HealthbarSlider.value = NormalizedPercentage;
    }
    public void RefreshHealthbarSize(float inValue) //Changes the WIDTH of the healthbar if the auto scaling is enabled.
    {
        if (ScaleWithMAXHP)
        {
            float factor = 25*Mathf.Log(inValue+(inValue/10)/10);
            Debug.Log(inValue);
            //Length = inValue * 2;
            HealthbarRect.sizeDelta = new Vector2(factor , HealthbarRect.sizeDelta.y);
            
        }
    }

    public void SetHealthBarAlpha(float inValue)// Controls the alpha of the healthbar based on a normalized value;
    {
        //HPOpacity = inValue;
        //BGOpacity = inValue;
        HealthbarCanvas.alpha = inValue;
    }

    #endregion

    #region Animator Methods
    public void TriggerAnimation(string inValue)
    {
        if (m_animator == null) return;

        m_animator.SetTrigger(inValue);
    }
    public void TriggerTopLayer(int weight)
    {
        if (m_animator == null) return;
        m_animator.SetLayerWeight(topLayer, weight);

    }
  

    #endregion

    #region Particle Methods
    public void PlayParticle(ParticleSystem inPart)
    {
        if(inPart != null)
        {
            inPart.gameObject.SetActive(true);
            inPart.Play();
        }
    }
    public void StopParticle(ParticleSystem inPart)
    {
        if (inPart != null)
        {
            inPart.Stop();
            inPart.gameObject.SetActive(false);
        }

    }
    #endregion

    #region CorpsesMethods
    public void EnableCorpses()
    {
       if(m_corpse!=null)
        {
            GameObject corpe = HFPoolManager.Instance.GetPooledObject(m_corpse.ID);

            if (corpe == null) return;

            corpe.transform.position = transform.position;
            corpe.transform.rotation = Quaternion.FromToRotation(transform.forward, transform.up);
            corpe.SetActive(true);
        }
    }
    #endregion

   
}
