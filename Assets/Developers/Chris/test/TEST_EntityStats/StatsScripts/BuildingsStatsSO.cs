using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;

[CreateAssetMenuAttribute(fileName = "New building data", menuName = "EntityData/Building")]
public class BuildingsStatsSO : EntityStatsSO
{
    public Sprite Icon;

    public BuildingType BuildingType;

    public int Weight;
}
