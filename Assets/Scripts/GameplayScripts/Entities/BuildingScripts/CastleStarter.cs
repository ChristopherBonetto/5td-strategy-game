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

    [SerializeField]
    private float m_maxSpawnDistance = 6;
    private Collider m_collider;


    public override void Awake()
    {
        base.Awake();
        m_collider = GetComponent<Collider>();
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

        Vector3 SpawnPoint = FreePoint(transform.position, m_maxSpawnDistance);

        GameController.Instance.CreateNewTroop(UnitType.STANDARD_ALLY, PlayerType.Player, SpawnPoint.SnapLocation(), false);
    }

    private Vector3 FreePoint(Vector3 center, float maxRadius)
    {
        Vector3 pos = center;

        float ang = Random.value * 360;
        float radius = Random.Range(3, maxRadius);

        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
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
