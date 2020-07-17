using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using HF.Refactoring;
using UnityEngine.UI;
using System;

public class Entity_TestScript : MonoBehaviour
{
    [SerializeField] private GameObject caste;
    [SerializeField] private Transform castleSpawnPoint;
    private CastleStarter m_instantiatedCastle = null;

    [SerializeField] private Transform enemySpawnPoint;

    [SerializeField] private Button StartGameButton;
    [SerializeField] private Button SpawnEnemyButton;
    [SerializeField] private Button AddGemsButton;

    [SerializeField] private Dropdown EntitiesDropdown;
    List<string> entities = new List<string>();

    private void Awake()
    {
        StartGameButton.onClick.AddListener(StartGame);
        SpawnEnemyButton.onClick.AddListener(SpawnEnemy);
        AddGemsButton.onClick.AddListener(AddGems);

        InitializeGame();
    }

    public void InitializeGame()
    {
        EnableCheatButtons(false);

        TakeInfoDropDown();
    }

    public void EnableCheatButtons(bool inValue)
    {
        StartGameButton.gameObject.SetActive(!inValue);
        SpawnEnemyButton.gameObject.SetActive(inValue);
        AddGemsButton.gameObject.SetActive(inValue);
    }

    #region Buttons
    public void StartGame()
    {
        if (m_instantiatedCastle == null)
        {
            m_instantiatedCastle = Instantiate(caste).GetComponent<CastleStarter>();
            HFGameManager.Instance.ChangeGMState(GameStates.InitializeLevel);
            HFGameManager.Instance.ChangeGMState(GameStates.PlayingLevel);
            EnableCheatButtons(true);
        }
    }

    public void SpawnEnemy()
    {
        if (m_instantiatedCastle != null)
        {
            UnitType enemyUnitType = (UnitType)EntitiesDropdown.value == 0 ? (UnitType)EntitiesDropdown.value : (UnitType)(EntitiesDropdown.value + 6);
            Troop enemy = GameController.Instance.CreateNewTroop(enemyUnitType, PlayerType.AI, enemySpawnPoint.position, true);

            if(enemy!=null)
            enemy.AssignTargetCastle(m_instantiatedCastle, m_instantiatedCastle.m_enemyEngagePoints[0].position);
        }
    }


    public void AddGems()
    {
        GameController.Instance.AddResources(50);
    }
    #endregion

    #region Dropdown

    public void TakeInfoDropDown()
    {
        EntitiesDropdown.ClearOptions();
        string[] enumNames = Enum.GetNames(typeof(UnitType));
        List<string> names = new List<string>(enumNames);
        EntitiesDropdown.AddOptions(names);
    }

    #endregion
}
