using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitVisual : MonoBehaviour
{
    #region Particle Variables
    [SerializeField] private ParticleSystem m_takeDamageEffect;
    public ParticleSystem TakeDamageEffect { get => m_takeDamageEffect; }
    #endregion

    #region Healthbar Variables
    public static List<UnitVisual> Active = new List<UnitVisual>();

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
    [SerializeField] private GameObject m_corpse;
    public GameObject GetCorpse { get => m_corpse; }
    #endregion


    #region Animator Variables
    [SerializeField] private Animator m_animator;
    private int runLayer;
    private int attackLayer;
    public Animator TriggerAnimations { get => m_animator; }

    #endregion

    public Unit unit;

    private void Awake()
    {
       SetHealthBarAlpha(0);
       SelectionCircle.SetActive(false);

      if(m_animator!=null)
        {
            runLayer = m_animator.GetLayerIndex("Bottom Run");
            attackLayer = m_animator.GetLayerIndex("Top");
            m_animator.SetLayerWeight(attackLayer, 0);
        }




    }

    void OnEnable()
    {
        healthPercentage = 1f; //Reset Healthbar value to its maximuml
        Active.Add(this);
    }
    void OnDisable()
    {
        Active.Remove(this);
    }
    private void Update()
    {

        if (m_animator != null)
        {
            m_animator.SetLayerWeight(runLayer, unit.UnitAgent.velocity.magnitude);
        }




    }

    public void SetHealthbar(float percentage)
    {
        healthPercentage = percentage;
    }
    public void RefreshHealthbarSize(int inValue)
    {
        if (ScaleWithMAXHP)
        {
            Length = inValue * 2;
        }
    }

    public void SetHealthBarAlpha(float inValue)
    {
        HPOpacity = inValue;
        BGOpacity = inValue;
    }

    #region Animator Methods
    public void TriggerAttack(Animator animation)
    {

        if (m_animator != null)
        {
            m_animator.SetLayerWeight(attackLayer, 1);
        }
    }
    public void StopAttackAnimation(Animator animation)
    {

        if (m_animator != null)
        {
            m_animator.SetLayerWeight(attackLayer, 0);
        }
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
    public void EnableCorpses(GameObject inGameObject)
    {
       if(m_corpse!=null)
        {
            Instantiate(inGameObject, transform.position, transform.rotation);
        }
    }
    #endregion

   
}
