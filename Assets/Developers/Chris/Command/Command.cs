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
    private Vector3 m_destination;
    private Vector3 m_originalPosition;

    public TeleportCommand(EntityBehavior inEntity, Vector3 inPos) : base(inEntity)
    {
        m_destination = inPos;
    }

    public override void Execute()
    {
        m_originalPosition = m_entity.transform.position;
        m_entity.transform.position = m_destination;
    }

    public override void Undo()
    {
        m_entity.transform.position = m_originalPosition;
    }
}

public class MoveWithAgent : Command
{
    private Vector3 m_destination;
    private Vector3 m_originalPosition;

    ICanMove canMove;

    public MoveWithAgent(EntityBehavior inEntity, Vector3 inDestination) : base(inEntity)
    {
        m_destination = inDestination;
        canMove = inEntity.GetComponent<ICanMove>() as ICanMove;
    }

    public override void Execute()
    {
        if(canMove != null)
        {
            m_originalPosition = m_entity.transform.position;

            canMove.MoveFromTo(m_destination);
        }
    }

    public override void Undo()
    {
        if(canMove != null)
        canMove.MoveFromTo(m_originalPosition);
    }
}

public class GoToInteract : Command
{
    private EntityBehavior FocusObj;
    private EntityBehavior PreviousFocusObj;

    public GoToInteract(EntityBehavior inEntity, EntityBehavior inFocus) : base(inEntity)
    {
        FocusObj = inFocus;
    }

    public override void Execute()
    {
        if(m_entity.FocusEntity != null)
        {
            PreviousFocusObj = m_entity.FocusEntity;
        }

        m_entity.Interact(FocusObj);
    }

    public override void Undo()
    {
        m_entity.Interact(PreviousFocusObj);
    }
}
