using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using BehaviorDesigner.Runtime;

public class CastleStarter : BuildingBehaviour
{
    [SerializeField] private Transform[] m_towerSpawnPoints;

    [SerializeField] private Transform m_unitSpawnPoint;

    public bool m_canSpawn = true;

    protected override void OnEnable()
    {
        base.OnEnable();
        HFEventManager.SubscribeTo<EntityBehavior>(HFEventID.OnEntityDeath, CheckEntityDead);
        HFEventManager.SubscribeTo<bool>(HFEventID.OnPauseMode, CheckFreeze);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        HFEventManager.UnsubscribeFrom<EntityBehavior>(HFEventID.OnEntityDeath, CheckEntityDead);
        HFEventManager.UnsubscribeFrom<bool>(HFEventID.OnPauseMode, CheckFreeze);
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        AssignStats(GameController.Instance.Collection.BuildingsDictionary[BuildingType.CASTLE].BuildingStatsCopy);

        // Set this castel as reference in the bh tree.
        GlobalVariables.Instance.SetVariableValue("Castle", this.gameObject);

        if (m_towerSpawnPoints == null) return;
        foreach (Transform t in m_towerSpawnPoints)
        {
            GameController.Instance.CreateNewBuilding(BuildingType.TOWER, t.transform.position.SnapLocation());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (m_unitSpawnPoint == null) return;

            GameController.Instance.CreateNewTroop(UnitType.STANDARD_ALLY, PlayerType.Player, m_unitSpawnPoint.position.SnapLocation(), false);
        }
    }

    public void CheckFreeze(bool inValue)
    {
        m_canSpawn = !inValue;
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
        float timer = 0;

        UnitsStatsSO stats = troop.GetStats();

        bool go = true;

        while (go)
        {
            if (m_canSpawn)
            {
                timer += Time.deltaTime;
            }

            Debug.Log(timer + " " + stats.RespawnTime);

            if (timer >= stats.RespawnTime)
            {
                GameController.Instance.CreateNewTroop(stats.UnitType, PlayerType.Player, m_unitSpawnPoint.position.SnapLocation(), true);
                go = false;
            }
            yield return new WaitForEndOfFrame();
        }

    }
}
