using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using HF.Refactoring;
using UnityEngine.UI;

public class Entity_TestScript : MonoBehaviour
{
    [SerializeField] private GameObject caste;
    [SerializeField] private Transform castleSpawnPoint;
    private CastleStarter m_instantiatedCastle = null;

    [SerializeField] private Transform enemySpawnPoint;

    [SerializeField] private UnitType enemyUnitType;


    [SerializeField] private Button StartGameButton;
    [SerializeField] private Button SpawnEnemyButton;
    [SerializeField] private Button AddGemsButton;

    private void Awake()
    {
        StartGameButton.onClick.AddListener(StartGame);
        SpawnEnemyButton.onClick.AddListener(SpawnEnemy);
        AddGemsButton.onClick.AddListener(AddGems);

        EnableCheatButtons(false);
    }

    public void EnableCheatButtons(bool inValue)
    {
        StartGameButton.gameObject.SetActive(!inValue);
        SpawnEnemyButton.gameObject.SetActive(inValue);
        AddGemsButton.gameObject.SetActive(inValue);
    }


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
            Troop enemy = GameController.Instance.CreateNewTroop(enemyUnitType, PlayerType.AI, enemySpawnPoint.position, true);
            enemy.AssignTargetCastle(m_instantiatedCastle, m_instantiatedCastle.m_enemyEngagePoints[0].position);
        }
    }


    public void AddGems()
    {
        GameController.Instance.AddResources(50);
    }
}
