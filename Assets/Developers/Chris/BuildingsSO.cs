using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;

[CreateAssetMenuAttribute(fileName = "New building data", menuName = "EntityData/Building")]
public class BuildingsSO : EntitySO
{
    public BuildingType BuildingType;

    public int Weight;
}
