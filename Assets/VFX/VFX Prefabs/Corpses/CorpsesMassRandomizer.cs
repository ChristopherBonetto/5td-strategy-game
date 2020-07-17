using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpsesMassRandomizer : MonoBehaviour
{
    private Rigidbody m_rigidbody;
    private float mass;
    public float minMultiplier = .6f;
    public float maxMultiplier = 1.4f;

    [SerializeField]
    private bool m_UseTime = false;
    [SerializeField]
    private int m_totalWaveEndBeforeDisappear = 2;
    private int m_currentWaveEnd = 0;
    [SerializeField]
    private float m_totalTimeBeforeDisappear = 160;
    private float m_currentTimeElapsed;

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        mass = Random.Range(m_rigidbody.mass * minMultiplier, m_rigidbody.mass * maxMultiplier);
        m_rigidbody.mass = mass;
        m_rigidbody.AddForce(Random.Range(-4f * Random.Range(m_rigidbody.mass * minMultiplier, m_rigidbody.mass * maxMultiplier), 4f * Random.Range(m_rigidbody.mass * minMultiplier, m_rigidbody.mass * maxMultiplier)), 4f * Random.Range(m_rigidbody.mass * minMultiplier, m_rigidbody.mass * maxMultiplier), Random.Range(-4f * Random.Range(m_rigidbody.mass * minMultiplier, m_rigidbody.mass * maxMultiplier), 4f * Random.Range(m_rigidbody.mass * minMultiplier, m_rigidbody.mass * maxMultiplier)), ForceMode.Impulse);
    }

    private void OnEnable() 
    {
        // Reset values.
        m_currentTimeElapsed = 0;
        m_currentWaveEnd = 0;

        HFEventManager.SubscribeTo<GameStates>(HFEventID.OnGameStateChanged, OngGameStateChange);
        HFEventManager.SubscribeTo(HFEventID.OnWaveEnded, OnWaveEnd);
    }

    private void OnDisable() 
    {

        HFEventManager.UnsubscribeFrom<GameStates>(HFEventID.OnGameStateChanged, OngGameStateChange);
        HFEventManager.UnsubscribeFrom(HFEventID.OnWaveEnded, OnWaveEnd);
    }

    private void Update() 
    {
        if (m_UseTime) 
        {
            m_currentTimeElapsed += Time.deltaTime;
            if (m_currentTimeElapsed >= m_totalTimeBeforeDisappear) 
                gameObject.SetActive(false);
        }
    }

    private void OnWaveEnd() 
    {
        m_currentWaveEnd++;
        if (m_currentWaveEnd >= m_totalWaveEndBeforeDisappear)
            gameObject.SetActive(false);
    }

    private void OngGameStateChange(GameStates state) 
    {
        switch (state) 
        {
            case GameStates.InitializeLevel:
                gameObject.SetActive(false);
                break;
        }
    }
}
