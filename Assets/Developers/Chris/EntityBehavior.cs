using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using UnityEngine.AI;

public class EntityBehavior : MonoBehaviour, ITakeCommand, ITakeDamage
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

    public PlayerType m_entityPlayerType { get; protected set; }

    protected float m_timer = 0f;

    private EntityBehavior m_focusEntity = null;
    public EntityBehavior FocusEntity
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


    public virtual void Start()
    {
        m_commands = new List<Command>();
    }

    //Maybe a new command.
    public virtual void RefreshHp()
    {
        CurrentHp = EntityStats.MaxHp;
    }

    public virtual void AssignPlayer(PlayerType inPlayerType)
    {
        m_entityPlayerType = inPlayerType;

        if (m_entityPlayerType == PlayerType.Player)
        {
            gameObject.layer = GameController.Instance.m_playerLayer;
        }
        else if (m_entityPlayerType == PlayerType.AI)
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

    public virtual void Select()
    {
        Debug.Log("selected " + m_entityPlayerType + " " + gameObject.name);

        if(m_entityPlayerType == PlayerType.Player)
        {
            InputReaderManager.Instance.CurrentEntity = this;
        }
    }

    public virtual void Interact(EntityBehavior inEntity)
    {
        if(inEntity == null)
        {
            Debug.Log("ciao miao");
        }
        FocusEntity = inEntity;
        Debug.Log(gameObject.name + " want to interact with " + FocusEntity.name);
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
                Death();
                return true;
            }
            else
            {
                CurrentHp -= Damage;
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
        this.gameObject.SetActive(false);
    }

    #endregion
}
