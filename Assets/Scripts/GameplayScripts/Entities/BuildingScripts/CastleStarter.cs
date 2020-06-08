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

    [SerializeField, Tooltip("Spawn point distance from castle")]
    private float m_spawnDistance = 6;


    public override void Awake()
    {
        base.Awake();
    }

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
        if (Input.GetKeyDown(KeyCode.X))
        {
            Death();
        }
    }

    public void SpawnTroop()
    {
        // I saw you use a random point on game controller but if it goes on failure you don't recall the method 
        // in recursive way.
        // We need to find a way to make sure the success

        for (int i = 0; i < 8; i++)
        {
            Vector3 pos = GetPoint(transform.position, m_spawnDistance, i);

            if (!Physics.CheckSphere(pos + Vector3.up * 2.3f, 1))
            {
                GameController.Instance.CreateNewTroop(UnitType.STANDARD_ALLY, PlayerType.Player, pos, false);
                break;
            }
        }

        // Debug : there aren0t any free slot.
    }

    private Vector3 GetPoint(Vector3 center, float maxRadius, int index)
    {
        Vector3 pos = center;

        float ang = 360 / 8 * index;

        pos.x = center.x + maxRadius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.z = center.z + maxRadius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        return pos;
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

            if (timer >= stats.RespawnTime)
            {
                GameController.Instance.CreateNewTroop(stats.UnitType, PlayerType.Player, m_unitSpawnPoint.position.SnapLocation(), true);
                go = false;
            }
            yield return new WaitForEndOfFrame();
        }

    }

    public override void Death()
    {
        HFScenesManager.Instance.EndCurrentLevel(false);
        base.Death();
    }
}
