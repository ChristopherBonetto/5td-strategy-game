using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HFWaveInfoUIElement : MonoBehaviour
{
    [Header("Wave Info Field")]

    /// <summary>
    /// Number of wave compleated.
    /// </summary>
    public Text waveProgressionInfo;

    /// <summary>
    /// real time passed.
    /// </summary>
    public Text TimePassed;

    public void UpdateWaveInfoDisplayed(int currentWave, int maxWave)
    {
        waveProgressionInfo.text = $"Wave: {currentWave + 1}/{maxWave}";
    }
}
