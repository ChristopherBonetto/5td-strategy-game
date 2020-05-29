using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;

[RequireComponent(typeof(BehaviorTree))]
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


    protected virtual void OnEnable()
    {
        HFEventManager.SubscribeTo<GameStates>(HFEventID.OnGameStateChanged, GameStateChanged);
    }

    protected virtual void OnDisable()
    {
        HFEventManager.UnsubscribeFrom<GameStates>(HFEventID.OnGameStateChanged, GameStateChanged);
    }

    protected virtual void GameStateChanged(GameStates inState)
    {
        if((inState == GameStates.EndLevel || inState == GameStates.WarRoom))
        {
            Debug.Log(gameObject.name);
            ResetEntity();
        }
    }

    protected virtual void ResetEntity()
    {
        this.gameObject.SetActive(false);
    }

    public virtual void Awake()
    {
        m_behaviorTree = gameObject.GetComponent<BehaviorTree>();
        StopTree(true);
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
        //Debug.Log("selected " + EntityPlayerType + " " + gameObject.name);

        if(EntityPlayerType == PlayerType.Player)
        {
            InputReaderManager.Instance.CurrentEntity = this;
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

        HFEventManager.TriggerEvent(HFEventID.OnUnitDeath, this);
    }

    #endregion

    #region Attack

    public virtual void Attack()
    {
    }

    #endregion


    #region Specialization
    public virtual void Specialization(UnitType type)
    {
        if (GameController.Instance.CreateNewTroop(type, PlayerType.Player, this.transform.position) != null)
        {
            HFEventManager.TriggerEvent(HFEventID.OnUnitSpecialized, this, 0);
        }
    }

    public virtual void Specialization(BuildingType type)
    {
        if (GameController.Instance.CreateNewBuilding(type, PlayerType.Player, this.transform.position) != null)
        {
            HFEventManager.TriggerEvent(HFEventID.OnUnitSpecialized, this, 0);
        }
    }
    #endregion
}
