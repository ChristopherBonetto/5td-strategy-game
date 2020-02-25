using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HF;
using HF.WaveSystem;

public class HFEnemyInfoUIElement : MonoBehaviour
{
    public HorizontalLayoutGroup IconsParentGrid;
    public Image Prefab;
    public List<Image> EnemiesTroopIcons;

    private void Start()
    {
        EnemiesTroopIcons = new List<Image>();
    }

    public void SetEnemiesInfo(HFWave wave)
    {
        foreach (var item in EnemiesTroopIcons)
        {
            Destroy(item);
        }

        EnemiesTroopIcons.Clear();

        for (int i = 0; i < wave.MinorWavesCollection.Count; i++)
        {
            Debug.Log(wave.MinorWavesCollection[i].MinorWaveType);
            if (wave.MinorWavesCollection[i].MinorWaveType == MinorWaveType.Single)
                AddNewIcon(wave.MinorWavesCollection[i].UnitStatsData);
        }
    }

    public void AddNewIcon(HFBaseStats stats)
    {
        Image icon = Instantiate(Prefab, IconsParentGrid.transform);
        icon.sprite = stats.Icon;

        EnemiesTroopIcons.Add(icon);
        Debug.Log(EnemiesTroopIcons.Count);
    }
}
