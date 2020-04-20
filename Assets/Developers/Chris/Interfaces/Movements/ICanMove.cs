using UnityEngine;
using UnityEngine.AI;

public interface ICanMove : ITakeCommand
{
    void MoveFromTo(Vector3 endPosition);
}
