using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using BehaviorDesigner.Runtime;

public class CastleStarter : BuildingBehaviour
{
    [SerializeField] private Transform[] m_towerSpawnPoints;

    [SerializeField] private Transform m_unitSpawnPoint;

    protected override void OnEnable()
    {
        base.OnEnable();
        HFEventManager.SubscribeTo<EntityBehavior>(HFEventID.OnEntityDeath, CheckEntityDead);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        HFEventManager.UnsubscribeFrom<EntityBehavior>(HFEventID.OnEntityDeath, CheckEntityDead);
    }

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

    public void CheckEntityDead(EntityBehavior inEntity)
    {
        if(inEntity.EntityPlayerType == PlayerType.Player && inEntity is Troop)
        {
            StartCoroutine(Respawn(inEntity as Troop));
        }
    }

    IEnumerator Respawn(Troop troop)
    {
        yield return new WaitForSeconds(troop.GetStats().RespawnTime);
        GameController.Instance.CreateNewTroop(UnitType.STANDARD_ALLY, PlayerType.Player, m_unitSpawnPoint.position.SnapLocation());
    }
}
