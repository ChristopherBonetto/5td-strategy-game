using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using UnityEngine.AI;

public class EntityBehavior : MonoBehaviour, ISelectionable
{
    protected List<Command> m_commands;
    protected int m_currentCommandIndex;

    public PlayerType m_entityPlayerType { get; protected set; }

    protected float m_timer = 0f;
    protected bool m_canAttack = true;


    public virtual void Start()
    {
        m_commands = new List<Command>();
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

    public virtual void Clicked()
    {
        Debug.Log("selected " + gameObject.name);
    }
}
