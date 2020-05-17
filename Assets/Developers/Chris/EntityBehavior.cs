using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;

public class EntityBehavior : MonoBehaviour, ITakeCommand, ITakeDamage, IAttack
{
    protected EntityStatsSO m_entityStats;
    public virtual EntityStatsSO EntityStats
    {
        get { return m_entityStats; }
        set { m_entityStats = value; }
    }

    protected List<Command> m_commands;
    protected int m_currentCommandIndex;

    protected PlayerType m_entityPlayerType;
    public PlayerType EntityPlayerType { get => m_entityPlayerType; }

    protected EntityBehavior m_focusEntity = null;
    public EntityBehavior FocusEntity { get { return m_focusEntity; } set { m_focusEntity = value; } }

    protected int m_currentHp;
    public virtual int CurrentHp
    {
        get
        {
            return m_currentHp;
        }
        set
        {
            m_currentHp = value;
        }
    }

    protected IAttackTypes m_attackType;

    protected BehaviorTree m_behaviorTree;

    protected bool m_isBusy = false;
    public bool IsBusy
    {
        get { return m_isBusy; }
        set
        {
            m_isBusy = value;

            if(m_isBusy == true)
            {
                transform.tag = "IsBusy";
            }
            else
            {
                transform.tag = "NoBusy";
            }
        }
    }

    public int Level { get; set; } = 0;
    public bool CanUpgrade { get { return Level < EntityStats.Upgrades.Length; } }

    public void Upgrade()
    {
        if (CanUpgrade)
        {
            EntityStatsMultiplierSO upgrade = EntityStats.Upgrades[Level];
            EntityStats.MaxHp += Mathf.RoundToInt(EntityStats.MaxHp * upgrade.MaxHpMultiplier);
            EntityStats.Armor += Mathf.RoundToInt(EntityStats.Armor * upgrade.ArmorMultuplier);
            EntityStats.EngageRange += Mathf.RoundToInt(EntityStats.EngageRange * upgrade.EngageRangeMultiplier);
            EntityStats.AttackRange += Mathf.RoundToInt(EntityStats.AttackRange * upgrade.AttackRangeMultiplier);
            EntityStats.AttackSpeed += EntityStats.AttackSpeed * upgrade.AttackSpeedMultiplier;
            EntityStats.Damage += Mathf.RoundToInt(EntityStats.Damage * upgrade.DamageMultiplier);

            Level++;
            Debug.Log("Current level is: " + Level);
        }
    }

    public virtual void Awake()
    {
        m_behaviorTree = gameObject.GetComponent<BehaviorTree>();
        m_behaviorTree.PauseWhenDisabled = false;
        m_behaviorTree.enabled = false;
    }
    public virtual void Start()
    {
        m_commands = new List<Command>();

        m_attackType = new AttackBehaviors();

        IsBusy = false;
    }

    #region Generic Methods for entity

    public virtual void AssignPlayer(PlayerType inPlayerType)
    {
        m_entityPlayerType = inPlayerType;

        if (EntityPlayerType == PlayerType.Player)
        {
            gameObject.layer = GameController.Instance.m_playerLayer;
        }
        else if (EntityPlayerType == PlayerType.AI)
        {
            gameObject.layer = GameController.Instance.m_aiLayer;
        }
    }

    public virtual void AssignStats(EntityStatsSO inStats)
    {
        m_entityStats = inStats as EntityStatsSO;
        Debug.Log(m_entityStats);
    }

    public EntityStatsSO GetStats()
    {
        return m_entityStats;
    }

    public void StopTree(bool inValue)
    {
        m_behaviorTree.enabled = !inValue;
    }

    #endregion

    #region Command

    public virtual void AssignFocusEntity(EntityBehavior inEntity)
    {
        m_focusEntity = inEntity;
        Debug.Log(gameObject.name + " want to interact with " + inEntity.name);
    }

    //MAYBE INTERFACE.
    public virtual void ExecuteCommand(Command inCommand)
    {
        m_commands.Add(inCommand);
        inCommand.Execute();
        m_currentCommandIndex = m_commands.Count - 1;
    }

    public virtual void Undo()
    {
        if (m_currentCommandIndex < 0)
        {
            return;
        }
        m_commands[m_currentCommandIndex].Undo();
        m_commands.RemoveAt(m_currentCommandIndex);
        m_currentCommandIndex--;
    }

    public virtual void Redo()
    {
        m_commands[m_currentCommandIndex].Execute();
        m_currentCommandIndex++;
    }

    #endregion

    #region Click Interface

    public virtual void Click()
    {
        Debug.Log("selected " + EntityPlayerType + " " + gameObject.name);

        if(EntityPlayerType == PlayerType.Player)
        {
            InputReaderManager.Instance.CurrentEntity = this;
            HFEventManager.TriggerEvent(HFEventID.OnUnitSelected, this, 0);
        }
    }

    #endregion

    #region Hp Methods

    public virtual void RefreshHp()
    {
        CurrentHp = m_entityStats.MaxHp;
    }

    public virtual bool TakeDamage(int Damage)
    {
        if(m_entityStats.CanTakeDamage)
        {
            Damage = Mathf.Clamp(Damage, 1, m_entityStats.MaxHp + m_entityStats.Armor);

            if (CurrentHp <= Damage)
            {
                Debug.Log("Death");
                Death();
                return true;
            }
            else
            {
                CurrentHp -= Damage;
                Debug.Log(CurrentHp);
                if (InputReaderManager.Instance.CurrentEntity == this)
                {
                    InputReaderManager.Instance.CurrentEntity = null;
                }
                return false;
            }
        }
        return false;
    }

    public virtual void Death()
    {
        m_focusEntity = null;
        this.gameObject.SetActive(false);

        //TO DO: se è il castello finisce il match.
        // TO DO: trigger an event that set null all attackers with this enitity as focus. @Panta
    }

    #endregion

    #region Attack

    public virtual void Attack()
    {
    }

    #endregion

    #region Specialization
    public void Specialization(UnitType type)
    {
        gameObject.SetActive(false);
        GameController.Instance.CreateNewTroop(type, PlayerType.Player, this.transform.position);

        HFEventManager.TriggerEvent(HFEventID.OnUnitSpecialized, this, 0);
    }

    public void Specialization(BuildingType type)
    {
        gameObject.SetActive(false);
        GameController.Instance.CreateNewBuilding(type, PlayerType.Player, this.transform.position);

        HFEventManager.TriggerEvent(HFEventID.OnUnitSpecialized, this, 0);
    }
    #endregion
}
