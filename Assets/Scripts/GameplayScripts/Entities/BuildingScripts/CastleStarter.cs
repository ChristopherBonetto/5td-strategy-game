using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using BehaviorDesigner.Runtime;

public class CastleStarter : BuildingBehaviour
{
    [SerializeField]
    private Transform[] m_spawnPoints;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        AssignPlayer(PlayerType.Player);
        AssignStats(GameController.Instance.Collection.BuildingsDictionary[BuildingType.CASTLE].BuildingStatsCopy);

        // Set this castel as reference in the bh tree.
        GlobalVariables.Instance.SetVariableValue("Castle", this.gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (m_spawnPoints == null) return;

            foreach (Transform t in m_spawnPoints)
            {
                GameController.Instance.CreateNewTroop(UnitType.STANDARD_ALLY, PlayerType.Player, t.position.SnapLocation());
            }
            GameController.Instance.CreateNewBuilding(BuildingType.TOWER, PlayerType.Player, (new Vector3(transform.position.x + 5, transform.position.y, transform.position.z + 5).SnapLocation()));
        }
    }
}
