using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITakeCommand : IClickable
{
    void AssignFocusEntity(EntityBehavior inEntity);
}
