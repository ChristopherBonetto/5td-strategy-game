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
        get { return _healthPercentage; }
    }

    public float HealthBarWidth
    {
        get { return _Length; }
    }

    public float HealthBarHPAlpha
    {
        get { return _HPOpacity; }
    }
    public float HealthBarBGAlpha
    {
        get { return _BGOpacity; }
    }

    public float HealthBarYOffset
    {
        get { return VerticalOffset; }
    }

    public Color HealthBarColor
    {
        get { return _color; }
    }

    [Range(0f, 1f)]
    [SerializeField]
    float _HPOpacity = 1f;
    [Range(0f, 1f)]
    [SerializeField]
    float _BGOpacity = 1f;
    [SerializeField]
    private float VerticalOffset = 2.25f;
    [SerializeField]
    float _Length;
    [SerializeField]
    bool ScaleWithMAXHP;

    [SerializeField]
    Color _color = Color.green;

    float _healthPercentage;

    #endregion

    #region Particle Methods
    public void PlayParticle(ParticleSystem inPart)
    {
        inPart.gameObject.SetActive(true);
        inPart.Play();
    }
    public void StopParticle(ParticleSystem inPart)
    {
        inPart.Stop();
        inPart.gameObject.SetActive(false);
    }
    #endregion

    void OnEnable()
    {
        _healthPercentage = 1f; //Reset Healthbar value to its maximuml
        Active.Add(this);
    }
    void OnDisable()
    {
        Active.Remove(this);
    }
    public void SetHealthbar(float percentage)
    {
        _healthPercentage = percentage;
    }
    public void RefreshHealthbarSize()
    {
        if (ScaleWithMAXHP)
        {
            _Length = GetComponentInParent<Unit>().TroopRef.GetStats().MaxHp * 2;
        }
    }
}
