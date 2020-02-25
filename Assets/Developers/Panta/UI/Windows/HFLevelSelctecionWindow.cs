using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HFLevelSelctecionWindow : HFUIControl
{
    public override UIControlID Name => UIControlID.LevelSelection;

    /// <summary>
    /// it'sused as parent of buttons.
    /// </summary>
    public VerticalLayoutGroup ButtonsGrid;

    /// <summary>
    /// Text of the level 
    /// Change when the mouse overlay the buttons
    /// </summary>
    public HFLevelDescrition LevelDescription;

    [Header("Buttons Field")]

    /// <summary>
    /// Selection level button prefab.
    /// </summary>
    public HFButtonLevelSelection ButtonPrefab;

    /// <summary>
    /// Constant word present in every button. 
    /// It will be followed by the index of the level.
    /// </summary>
    public string PrefixButtonText;


    protected override void Start()
    {
        base.Start();
        SpawnLevelButtons();
    }

    private void SpawnLevelButtons()
    {
        List<HFLevelInfoSO> levels = HFLevelManager.Instance.LevelContainer.Levels;

        SetDefaultLevelDescription(levels[0]);

        for (int i = 0; i < levels.Count; i++)
        {
            HFButtonLevelSelection button = Instantiate(ButtonPrefab, ButtonsGrid.transform) as HFButtonLevelSelection;
            button.Level = levels[i];
            button.ButtonText.text = $"{PrefixButtonText}: {i}";
        }
    }

    public void OnClickBackToMainMenu() // => Wait war room details.
    {
        HFUIManager.Instance.ShowAndHide(UIControlID.MainMenu, this);
    }

    private void SetDefaultLevelDescription(HFLevelInfoSO info)
    {
        LevelDescription.LevelName.text = info.LevelName;
        LevelDescription.LevelDescrition.text = info.m_levelDescription;
    }
}

