using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using UnityEngine.AI;

public class EntityBehavior : MonoBehaviour, ITakeCommand, ITakeDamage, IAttack
{
    private EntityStatsSO m_entityStats;
    public virtual EntityStatsSO EntityStats
    {
        get
        {
            return m_entityStats;
        }
        set
        {
            m_entityStats = value;
        }
    }

    protected List<Command> m_commands;
    protected int m_currentCommandIndex;

    private PlayerType m_entityPlayerType;
    public PlayerType EntityPlayerType
    {
        get
        {
            return m_entityPlayerType;
        }
        protected set
        {
            m_entityPlayerType = value;
        }
    }

    protected float m_timer = 0f;

    protected EntityBehavior m_focusEntity = null;
    public virtual EntityBehavior FocusEntity
    {
        get
        {
            return m_focusEntity;
        }
        set
        {
            m_focusEntity = value;
        }
    }

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

    protected bool m_inCombat = false;
    public bool inCombat { get => m_inCombat; }

    //public TroopState CurrentTroopState = TroopState.FreeMove;

    public virtual void Start()
    {
        m_commands = new List<Command>();

        m_attackType = new AttackBehaviors();
    }

    public virtual void UnlockEntity()
    {
        ChangeInCombat(false);
    }

    public virtual void ChangeInCombat(bool inValue)
    {
        m_inCombat = inValue;
    }


    //Maybe a new command.
    public virtual void RefreshHp()
    {
        CurrentHp = EntityStats.MaxHp;
    }

    public virtual void AssignPlayer(PlayerType inPlayerType)
    {
        EntityPlayerType = inPlayerType;

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
        gameObject.name = inStats.Name + "Troops";
        EntityStats = inStats;
    }

    public virtual bool Timer(float destinationTime)
    {
        m_timer += Time.deltaTime;

        if (m_timer >= destinationTime)
        {
            m_timer = 0f;
            return true;
        }
        else
        {
            return false;
        }
    }


    #region Command

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
            // Trigger the event "OnUnitSelected(this)"
        }
    }

    public virtual void AssignFocusEntity(EntityBehavior inEntity)
    {
        FocusEntity = inEntity;
        Debug.Log(gameObject.name + " want to interact with " + inEntity.name);
    }

    #endregion

    #region Take Damage Inteface

    public virtual bool TakeDamage(int Damage)
    {
        if(EntityStats.CanTakeDamage)
        {
            Damage = Mathf.Clamp(Damage, 1, EntityStats.MaxHp + EntityStats.Armor);

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
        FocusEntity = null;
        this.gameObject.SetActive(false);

        //TO DO: se è il castello finisce il match.
        // TO DO: trigger an event that set null all attackers with this enitity as focus. @Panta
    }

    #endregion

    public virtual void Attack()
    {
    }
}
