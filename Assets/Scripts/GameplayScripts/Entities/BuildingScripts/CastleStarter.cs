using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Types;
using BehaviorDesigner.Runtime;
using HF.Refactoring;

public class CastleStarter : BuildingBehaviour
{
    #region Variables

    #region Spawn variables

    [SerializeField] private Transform[] m_towerSpawnPoints;

    [SerializeField] private Transform m_unitSpawnPoint;

    public bool m_canSpawn = true;

    [SerializeField, Tooltip("Spawn point distance from castle")]
    private float m_spawnDistance = 6;

    #endregion

    public Transform[] m_enemyEngagePoints;

    #region Healthbar Variables
    public static List<UnitVisual> Active = new List<UnitVisual>();
    public GameObject Healthbar;
    private Slider HealthbarSlider;
    private RectTransform HealthbarRect;
    private CanvasGroup HealthbarCanvas;

    public float HealthPercentage
    {
        get { return healthPercentage; }
    }

    public float HealthBarWidth
    {
        get { return Length; }
    }

    public float HealthBarHPAlpha
    {
        get { return HPOpacity; }
    }
    public float HealthBarBGAlpha
    {
        get { return BGOpacity; }
    }

    public float HealthBarYOffset
    {
        get { return VerticalOffset; }
    }

    public Color HealthBarColor
    {
        get { return color; }
    }

    [Range(0f, 1f)]
    [SerializeField]
    public float HPOpacity = 1f;
    [Range(0f, 1f)]
    [SerializeField]
    public float BGOpacity = 1f;
    [SerializeField]
    private float VerticalOffset = 2.25f;
    [SerializeField]
    float Length;
    [SerializeField]
    bool ScaleWithMAXHP;
    [SerializeField]
    Color color = Color.green;
    float healthPercentage;

    [SerializeField]
    public GameObject SelectionCircle;

    #endregion

    #endregion

    #region Behaviour Cycle

    public override void Awake()
    {
        base.Awake();

        HealthbarSlider = Healthbar.GetComponent<Slider>();
        HealthbarRect = Healthbar.GetComponent<RectTransform>();
        HealthbarCanvas = Healthbar.GetComponent<CanvasGroup>();
        SetHealthBarAlpha(1);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        HFEventManager.SubscribeTo<EntityBehavior>(HFEventID.OnEntityDeath, CheckEntityDead);
        HFEventManager.SubscribeTo<bool>(HFEventID.OnPauseMode, CheckFreeze);

        SetHealthbar(1f); //Reset Healthbar value to its maximum
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

        RefreshHealthbarSize(m_buildingStats.MaxHp);
    }

    #endregion

    public override void Click()
    {
        base.Click();
        HFEventManager.TriggerEvent(HFEventID.OnTutorialQuestCompleted, TutorialID.Select_Castle);
    }

    #region Create Entities Methods
    public void SpawnTroop()
    {
        // I saw you use a random point on game controller but if it goes on failure you don't recall the method 
        // in recursive way.
        // We need to find a way to make sure the success

        bool success = false;

        for (int i = 0; i < 8; i++)
        {
            Vector3 pos = GetPoint(transform.position, m_spawnDistance, i);

            if (!Physics.CheckSphere(pos, 1, LayerMask.GetMask("Player")))
            {
                Debug.Log("Trying to spaen a unit form the castle");
                Troop troop = GameController.Instance.CreateNewTroop(UnitType.STANDARD_ALLY, PlayerType.Player, pos, false);

                if(troop != null && m_isFreezed)
                {
                    troop.FreezeMode(true);
                }

                success = true;
                break;
            }
        }

        if (!success)
        {
            HFUIManager.Instance.Getwindow<HFUIHUD>(HFUIWindowID.HUD).SetMessage("Threse aren't empty slot around the castle, try to move away any entity from it");
        }
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

    public void CheckEntityDead(EntityBehavior inEntity)
    {
        if (inEntity.EntityPlayerType == PlayerType.Player && inEntity is Troop)
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
                for (int i = 0; i < 8; i++)
                {
                    Vector3 pos = GetPoint(transform.position, m_spawnDistance, i);

                    if (!Physics.CheckSphere(pos, 1, LayerMask.GetMask("Player")))
                    {
                        Debug.Log("Trying to spaen a unit form the castle");
                        GameController.Instance.CreateNewTroop(stats.UnitType, PlayerType.Player, pos, false);

                        if (troop != null && m_isFreezed)
                        {
                            troop.FreezeMode(true);
                        }
                        break;
                    }
                }
                go = false;
            }
            yield return new WaitForEndOfFrame();
        }

    }
    #endregion

    public void CheckFreeze(bool inValue)
    {
        m_canSpawn = !inValue;
    }

    #region Hp methods
    public override bool TakeDamage(int Damage)
    {
        if (HealthbarSlider != null && HealthbarSlider.isActiveAndEnabled)
        {
            SetHealthbar((float)m_currentHp / (float)m_buildingStats.MaxHp);
        }
        return base.TakeDamage(Damage);
    }

    public override void Death()
    {
        StopAllCoroutines();

        BehaviorManager.instance.enabled = false;
        HFScenesManager.Instance.EndCurrentLevel(false);
    }

    #region HealthBar methods

    public void SetHealthbar(float NormalizedPercentage) //Changes the fill of the healthbar based on a provided normalized value;
    {
        //healthPercentage = percentage;
        HealthbarSlider.value = NormalizedPercentage;
    }
    public void RefreshHealthbarSize(int inValue) //Changes the WIDTH of the healthbar if the auto scaling is enabled.
    {
        if (ScaleWithMAXHP)
        {
            float factor = 5 * Mathf.Pow(inValue + 1, 2) / Mathf.Pow(inValue + 2, 2);
            //Length = inValue * 2;
            HealthbarRect.sizeDelta = new Vector2(inValue * factor, HealthbarRect.sizeDelta.y);

        }
    }

    public void SetHealthBarAlpha(float inValue)// Controls the alpha of the healthbar based on a normalized value;
    {
        //HPOpacity = inValue;
        //BGOpacity = inValue;
        HealthbarCanvas.alpha = inValue;
    }

    #endregion
#endregion
}
