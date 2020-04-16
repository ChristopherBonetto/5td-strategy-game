using UnityEngine;
using UnityEngine.AI;

public abstract class Command
{
    protected EntityBehavior m_entity;

    public Command(EntityBehavior inEntity)
    {
        m_entity = inEntity;
    }

    public abstract void Execute();
    public abstract void Undo();
}

public class TeleportCommand : Command
{
    private Vector3 m_direction;

    public TeleportCommand(EntityBehavior inEntity, Vector3 inDirection) : base(inEntity)
    {
        m_direction = inDirection;
    }

    public override void Execute()
    {
        m_entity.transform.position += m_direction * 0.1f;
    }

    public override void Undo()
    {
        m_entity.transform.position += m_direction * 0.1f;
    }
}

public class MoveToAgent : Command
{
    private Vector3 m_destination;
    private Vector3 m_originalPosition;

    public MoveToAgent(EntityBehavior inEntity, Vector3 inDestination) : base(inEntity)
    {
        m_destination = inDestination;
    }

    public override void Execute()
    {
        m_originalPosition = m_entity.transform.position;

        m_entity.AddRemoveAgent(true);
        m_entity.Agent.destination = m_destination;
    }

    public override void Undo()
    {
        m_entity.Agent.destination = m_originalPosition;
    }
}
