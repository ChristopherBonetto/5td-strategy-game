using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using Types;
using BehaviorDesigner.Runtime.Tasks;
using System.Xml.Schema;

public class Unit : MonoBehaviour, ITakeDamage
{
    private NavMeshAgent m_unitAgent;
    public NavMeshAgent UnitAgent { get => m_unitAgent; }

    private BehaviorTree m_unitTree;
    public BehaviorTree UnitTree { get => m_unitTree; }

    [SerializeField] private Troop m_troopRef;
    public Troop TroopRef { get => m_troopRef; }

    [Header("Ranged Field")]
    [SerializeField] private Transform m_BulletSpawnPoint;
    public Transform BulletSpawnPoint => m_BulletSpawnPoint;

    [SerializeField] private HFPoolID m_BulletID;
    public HFPoolID BulletID => m_BulletID;

    private int m_unitHp;
    public int UnitHp { get => m_unitHp; }
    private bool m_canShowHealthBar = true;

    private IAttackTypes m_unitAttackType;

    [Header("Regen")]
    [SerializeField]
    private float m_regenAmountPerFrame;
    [SerializeField]
    private float m_waitTimeToStartRegen = 10;
    private float m_lastTimeGetHit;

    private Unit m_focusUnit;
    public Unit FocusUnit
    {
        get
        {
            return m_focusUnit;
        }
        set
        {
            m_focusUnit = value;
        }
    }

    private BuildingBehaviour m_focusBuilding;
    public BuildingBehaviour FocusBuilding
    {
        get
        {
            return m_focusBuilding;
        }
        set
        {
            m_focusBuilding = value;
        }
    }

    private GameObject m_visualObj;
    public GameObject VisualObj
    {
        get { return m_visualObj; }
        set
        {
            m_visualObj = value;

            if(m_visualObj != null)
            {
                m_visualScript = m_visualObj.GetComponent<UnitVisual>();
                m_visualScript.unit = this;
            }
            else
            {
                m_visualScript = null;
            }
        }
    }

    private UnitVisual m_visualScript;
    public UnitVisual visualScript
    {
        get => m_visualScript;
    }

    private Vector3 m_unitFormationPos = Vector3.zero;
    public Vector3 UnitFormationPos
    {
        get
        {
            return m_unitFormationPos + TroopRef.Agent.transform.position;
        }
        set
        {
            m_unitFormationPos = value;
        }
    }

    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {
        if (m_troopRef.EntityPlayerType == PlayerType.Player && !m_troopRef.IsBusy && Time.time > m_lastTimeGetHit + m_waitTimeToStartRegen)
        {
            if (m_unitHp < m_troopRef.GetStats().MaxHp)
            {
                m_unitHp += (int)Mathf.Max(1, m_regenAmountPerFrame);
            }
        }
    }

    #region Initialize

    private void Initialize()
    {
        m_unitAgent = gameObject.GetComponent<NavMeshAgent>();

        m_unitTree = gameObject.GetComponent<BehaviorTree>();

        m_unitAttackType = new AttackBehaviors();

        StopTree(true);
    }

    #endregion

    #region Assignments

    public void AssignFocusToUnit(Unit inUnit)
    {
        FocusBuilding = null;

        AssignValuesToTree();
        FocusUnit = inUnit;
    }

    public void AssignFocusToUnit(BuildingBehaviour building)
    {
        FocusUnit = null;

        AssignValuesToTree();
        FocusBuilding = building;
    }

    public void AssignValuesToTree()
    {
        UnitTree.SetVariableValue("UnitRef", this);
        UnitTree.SetVariableValue("TroopRef", m_troopRef);
        UnitTree.SetVariableValue("MovSpeed", m_troopRef.GetStats().UnitSpeed);
        UnitTree.SetVariableValue("AttackSpeed", m_troopRef.GetStats().AttackSpeed);
        UnitTree.SetVariableValue("AttackRange", (float)m_troopRef.GetStats().AttackRange);
    }

    #endregion

    #region Behavior Tree

    public void StopTree(bool inValue)
    {
        m_unitTree.enabled = !inValue;
    }

    #endregion

    #region Attack

    public void UnitAttack()
    {
        if (FocusUnit)
        {
            if (m_troopRef.GetStats().AttackType == AttackType.MELEE)
            {

                m_unitAttackType.SingleMeleeAttack(m_focusUnit, TroopRef.GetStats().Damage);
            }
            else if (m_troopRef.GetStats().AttackType == AttackType.RANGED)
            {
                m_BulletSpawnPoint.forward = (FocusUnit.transform.position - transform.position).normalized;
                HF.HFBullet bullet = HFPoolManager.Instance.GetPooledObject(BulletID.ID).GetComponent<HF.HFBullet>();
                bullet.transform.rotation = BulletSpawnPoint.rotation;
                bullet.transform.position = BulletSpawnPoint.position;
                bullet.SetAllyLayer(LayerMask.LayerToName(gameObject.layer));
                bullet.SetParameters(new HF.HFBulletParameters(null, 0, 0, 50));
                bullet.gameObject.SetActive(true);

                m_unitAttackType.SingleMeleeAttack(m_focusUnit, TroopRef.GetStats().Damage);
            }
        }
        else if (FocusBuilding)
        {
            if (m_troopRef.GetStats().AttackType == AttackType.MELEE)
            {
                m_unitAttackType.SingleMeleeAttack(FocusBuilding, TroopRef.GetStats().Damage);
            }
            else if (m_troopRef.GetStats().AttackType == AttackType.RANGED)
            {
                m_BulletSpawnPoint.forward = (FocusUnit.transform.position - transform.position).normalized;
                HF.HFBullet bullet = HFPoolManager.Instance.GetPooledObject(BulletID.ID).GetComponent<HF.HFBullet>();
                bullet.transform.rotation = BulletSpawnPoint.rotation;
                bullet.transform.position = BulletSpawnPoint.position;
                bullet.SetAllyLayer(LayerMask.LayerToName(gameObject.layer));
                bullet.SetParameters(new HF.HFBulletParameters(null, 0, 0, 10));
                bullet.gameObject.SetActive(true);

                m_unitAttackType.SingleMeleeAttack(FocusBuilding, TroopRef.GetStats().Damage);
            }
        }


        //Tanto dovrà essere cambiato siccome il danno verrà messo all'animazione.
        if(m_visualScript != null)
        {
            m_visualScript.TriggerAttack("isAttacking01");
        }
        
        TroopRef.AttachAndPlaySound(TroopRef.GetStats().AttackSound);
    }

    public void CheckAnotherTarget()
    {
        if (m_troopRef.m_currentBattle != null)
        {
            m_troopRef.m_currentBattle.TakeOtherTarget(this);
        }
    }

    #endregion

    #region Health

    public void RefreshHp()
    {
        m_unitHp = m_troopRef.GetStats().MaxHp;
        m_visualScript.RefreshHealthbarSize(TroopRef.GetStats().MaxHp);
    }

    public bool TakeDamage(int Damage)
    {
        if (m_troopRef == null || m_troopRef.GetStats() == null) return true;

        Damage = Mathf.Clamp(Damage, 1, UnitHp + m_troopRef.GetStats().Armor);

        if (UnitHp <= Damage)
        {
            Death();
            return true;
        }
        else
        {
            m_unitHp -= Damage;

            TroopRef.AttachAndPlaySound(TroopRef.GetStats().TakeDamageSound);

            if (m_visualScript != null)
            {
                float HPperc = ((float)m_unitHp / (float)TroopRef.GetStats().MaxHp);
                m_visualScript.SetHealthbar(HPperc);

                m_visualScript.PlayParticle(m_visualScript.TakeDamageEffect);

                if (m_canShowHealthBar && InputReaderManager.Instance.CurrentEntity != TroopRef)
                {
                    StartCoroutine(ShowHealthBar(1f));
                }
            }

            return false;
        }
    }

    public void Death()
    {
        StopAllCoroutines();
        m_unitHp = 0;

        if (m_visualScript != null)
        {
            m_visualScript.EnableCorpses();
        }
        
        if(FocusUnit != null || FocusBuilding != null)
        {
            AssignFocusToUnit((Unit)null);
        }

        if(m_troopRef.EntityPlayerType == PlayerType.AI)
        {
            GameObject gem = ObjectPooler.Instance.GetPooledObject("Gem");
            gem.transform.position = this.transform.position;
            gem.SetActive(true);
        }

        TroopRef.AttachAndPlaySound(TroopRef.GetStats().DeathSound);

        m_troopRef.TroopTakeDamage(this);
    }



    #endregion

    #region Visual Controls

    IEnumerator ShowHealthBar(float inDestinationTime)
    {
        m_canShowHealthBar = false;
        UpdateUnitVisualState(true);
        yield return new WaitForSeconds(inDestinationTime);
        UpdateUnitVisualState(false);
        m_canShowHealthBar = true;
    }

    public void UpdateUnitVisualState(bool state)
    {
        if (m_visualScript == null) { return; }

        if (state == true)
        {
            //Lerp BG and FG alpha of healthbar and Selectioncircle from 0 to 1 with a preferred ease, 
            m_visualScript.SetHealthBarAlpha(1f);
            m_visualScript.SelectionCircle.SetActive(true);

        }
        else
        {
            //disable
            m_visualScript.SetHealthBarAlpha(0f);
            m_visualScript.SelectionCircle.SetActive(false);
        }
    }

    public void ResetUnitRotation()
    {
        if (m_visualScript == null) return;

        this.gameObject.transform.forward = TroopRef.transform.forward;
        //m_unitAgent.transform.rotation = TroopRef.transform.rotation;
        //VisualObj.transform.rotation = TroopRef.transform.rotation;
    }
    #endregion
}
