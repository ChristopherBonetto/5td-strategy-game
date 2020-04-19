using UnityEngine;
using UnityEngine.AI;

public interface ICanMove : ITakeCommand
{
    void TakeAgentComponent();
    void MoveFromTo(Vector3 endPosition);
    void Stop(bool inBool);
    bool IsMoving();
}
