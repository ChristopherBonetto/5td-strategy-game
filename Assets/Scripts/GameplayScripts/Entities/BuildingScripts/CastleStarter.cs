using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using BehaviorDesigner.Runtime;

public class CastleStarter : BuildingBehaviour
{
    [SerializeField] private Transform[] m_towerSpawnPoints;

    [SerializeField] private Transform m_unitSpawnPoint;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        AssignPlayer(PlayerType.Player);
        AssignStats(GameController.Instance.Collection.BuildingsDictionary[BuildingType.CASTLE].BuildingStatsCopy);

        // Set this castel as reference in the bh tree.
        GlobalVariables.Instance.SetVariableValue("Castle", this.gameObject);

        if (m_towerSpawnPoints == null) return;
        foreach (Transform t in m_towerSpawnPoints)
        {
            GameController.Instance.CreateNewBuilding(BuildingType.TOWER, PlayerType.Player, t.transform.position.SnapLocation());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (m_unitSpawnPoint == null) return;

            GameController.Instance.CreateNewTroop(UnitType.STANDARD_ALLY, PlayerType.Player, m_unitSpawnPoint.position.SnapLocation());
        }
    }
}
