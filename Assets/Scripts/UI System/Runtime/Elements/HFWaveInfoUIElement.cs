using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HFWaveInfoUIElement : MonoBehaviour
{
    [SerializeField]
    private Text m_waveInfo;

    private bool m_enableTimer;

    [SerializeField]
    private Text m_timeElapsedText;

    // the time elapsed without consider the pause
    //between waves.
    private float m_timeElapsed;

    private void OnEnable()
    {
        HFEventManager.SubscribeTo(HFEventID.OnWaveCleared, OnWaveEnd);
        HFEventManager.SubscribeTo(HFEventID.OnWaveBeginned, OnNewWaveBegin);
        HFEventManager.SubscribeTo<int, int>(HFEventID.OnWaveIndexUpdate, OnWaveIndexUpdate);
    }

    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom(HFEventID.OnWaveCleared, OnWaveEnd);
        HFEventManager.UnsubscribeFrom(HFEventID.OnWaveBeginned, OnNewWaveBegin);
        HFEventManager.UnsubscribeFrom<int, int>(HFEventID.OnWaveIndexUpdate, OnWaveIndexUpdate);
    }

    private void Update()
    {
        if (m_enableTimer)
            ExecuteTimer();
    }

    private void ExecuteTimer()
    {
        m_timeElapsed += Time.deltaTime;
        int minutes = (int)(m_timeElapsed) / 60;
        int seconds = (int)(m_timeElapsed) % 60;
        m_timeElapsedText.text = $"{minutes} : {seconds}";
    }

    private void OnWaveIndexUpdate(int currentWave, int totalWave)
    {
        Debug.Log("Updating UI wave inde...");
        m_waveInfo.text = $"Wave: {currentWave} / {totalWave}";
    }

    private void OnEnemyKilled()
    {
        // Feature...
        // Support enemy count
    }

    #region Events

    private void OnNewWaveBegin()
    {
        m_enableTimer = true;
    }

    private void OnWaveEnd()
    {
        m_enableTimer = false;
    }

    #endregion
}
