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

    private bool Pausing = false;

    private void OnEnable()
    {
        HFEventManager.SubscribeTo(HFEventID.OnWaveCleared, OnWaveCleared);
        HFEventManager.SubscribeTo(HFEventID.OnWaveBeginned, OnWaveBeginned);
        HFEventManager.SubscribeTo<int, int>(HFEventID.OnWaveIndexUpdate, OnWaveIndexUpdate);
        HFEventManager.SubscribeTo<bool>(HFEventID.OnPauseMode, OnPauseMode);
    }

    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom(HFEventID.OnWaveCleared, OnWaveCleared);
        HFEventManager.UnsubscribeFrom(HFEventID.OnWaveBeginned, OnWaveBeginned);
        HFEventManager.UnsubscribeFrom<int, int>(HFEventID.OnWaveIndexUpdate, OnWaveIndexUpdate);
        HFEventManager.UnsubscribeFrom<bool>(HFEventID.OnPauseMode, OnPauseMode);
    }

    private void Update()
    {
        if (m_enableTimer && Pausing)
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
        m_waveInfo.text = $"Wave: {currentWave} / {totalWave}";
    }

    private void OnEnemyKilled()
    {
        // Feature...
        // Support enemy count
    }

    #region Events

    private void OnWaveBeginned()
    {
        m_enableTimer = true;
    }

    private void OnWaveCleared()
    {
        m_enableTimer = false;
    }

    private void OnPauseMode(bool freeze)
    {
        Pausing = freeze;
    }

    #endregion
}
