using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICarriable
{
    /// <summary>
    /// Called mostly from buildings to perform the troops carry action.
    /// </summary>
    void Carry(Vector3 inCarryPosition, out bool success);

    /// <summary>
    /// Called mostly from buildings to perform the troops drop action.
    /// </summary>
    void Drop(Vector3 inDropAction, out bool success);
}
