using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HF.WaveSystem;

public class HFWaveView : MonoBehaviour
{
    public bool TimerActivated;

    private HFInGameWindow m_inGameWindow;
    public float TimeElapsed { get; private set; }

    private void OnEnable()
    {
        HFEventManager.SubscribeTo<HFLevelInfoSO, bool>(HFEventID.OnEndLevel, OnEndLevel);
    }

    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom<HFLevelInfoSO, bool>(HFEventID.OnEndLevel, OnEndLevel);
    }

    private void Start()
    {
        m_inGameWindow = HFUIManager.Instance.UIControls[UIControlID.InGameWindow] as HFInGameWindow;
        m_inGameWindow.TimeElapsed.text = "";
    }

    private void Update()
    {
        if (TimerActivated)
        {
            TimeElapsed += Time.deltaTime * Time.timeScale;
            m_inGameWindow.TimeElapsed.text = $"{(int)(TimeElapsed / 60)} : {(int)(TimeElapsed % 60)}";
        }
    }

    /// <summary>
    /// Invoked through event
    /// </summary>
    /// <param name="isWin"></param>
    public void OnEndLevel(HFLevelInfoSO level, bool isWin)
    {
        m_inGameWindow.ButtonReturnToLevelSelection.gameObject.SetActive(true);
    }

    public void UpdateEnemiesInfo(params HFBaseStats[] inUnits)
    {
        // update the grid in UI that contains those infos.
    }

    public void UpdateWaveInfo(int inCurrentWave, int inTotalWaves)
    {
        m_inGameWindow.waveProgressionInfo.text = $"Wave: {inCurrentWave}/{inTotalWaves}";
    }

    public void EnableButtonToCallnextWave(bool value)
    {
        m_inGameWindow.ButtonCallNextWave.gameObject.SetActive(value);
    }

    #region Enemy icons setting
    public void SetEnemiesInfo(HFWave wave)
    {
        foreach (var item in m_inGameWindow.EnemiesTroopIcons)
        {
            // Push it in the pool.
            Destroy(item);
        }

        m_inGameWindow.EnemiesTroopIcons.Clear();
        m_inGameWindow.EnemiesTroopsStats.Clear();

        // Go through all minorwave
        for (int i = 0; i < wave.MinorWavesCollection.Count; i++)
        {
            if (wave.MinorWavesCollection[i].MinorWaveType != MinorWaveType.Wait)
            {
                // Check if the unit stas picked 
                // is already inside the list.

                if (m_inGameWindow.EnemiesTroopsStats.Count <= 0)
                {
                    AddNewIcon(wave.MinorWavesCollection[i].UnitStatsData);
                }
                else
                {
                    for (int j = 0; j < m_inGameWindow.EnemiesTroopsStats.Count; j++)
                    {
                        if (wave.MinorWavesCollection[i].UnitStatsData == m_inGameWindow.EnemiesTroopsStats[j]) continue;
                        else AddNewIcon(wave.MinorWavesCollection[i].UnitStatsData);
                    }
                }
            }
        }
    }

    private void AddNewIcon(HFBaseStats stats)
    {
        // Pool it from the pool system.
        Image icon = Instantiate(m_inGameWindow.EnemyIconPrefab, m_inGameWindow.EnemyIconsGridParent.transform);
        icon.sprite = stats.Icon;

        m_inGameWindow.EnemiesTroopIcons.Add(icon);
        m_inGameWindow.EnemiesTroopsStats.Add(stats);
    }
    #endregion
}
