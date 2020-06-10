using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitVisual : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_takeDamageEffect;
    public ParticleSystem TakeDamageEffect { get => m_takeDamageEffect; }

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
}
