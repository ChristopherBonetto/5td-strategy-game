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

    private void Awake()
    {
       SetHealthBarAlpha(0);
       SelectionCircle.SetActive(false);
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
}
