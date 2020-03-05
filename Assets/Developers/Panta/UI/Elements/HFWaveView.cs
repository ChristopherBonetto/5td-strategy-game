using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HF.WaveSystem;

public class HFWaveView : MonoBehaviour
{
    private HFInGameWindow m_inGameWindow;
    // var time elapsed;

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
    }

    /// <summary>
    /// Invoked through event
    /// </summary>
    /// <param name="isWin"></param>
    public void OnEndLevel(HFLevelInfoSO level, bool isWin)
    {
        m_inGameWindow.ButtonReturnToLevelSelection.gameObject.SetActive(true);
    }

    // Time elapsed Method() 
    // {
    // }

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

        for (int i = 0; i < wave.MinorWavesCollection.Count; i++)
        {
            if (wave.MinorWavesCollection[i].MinorWaveType != MinorWaveType.Wait)
            {
                AddNewIcon(wave.MinorWavesCollection[i].UnitStatsData);
            }
        }
    }

    private void AddNewIcon(HFBaseStats stats)
    {
        // Pool it from the pool system.
        Image icon = Instantiate(m_inGameWindow.EnemyIconPrefab, m_inGameWindow.EnemyIconsGridParent.transform);
        icon.sprite = stats.Icon;

        m_inGameWindow.EnemiesTroopIcons.Add(icon);
    }
    #endregion
}
