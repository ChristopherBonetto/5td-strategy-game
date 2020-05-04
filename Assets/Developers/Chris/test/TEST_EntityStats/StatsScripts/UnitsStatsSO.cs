using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;

[CreateAssetMenuAttribute(fileName = "New unit data", menuName = "EntityData/Unit")]
public class UnitsStatsSO : EntityStatsSO
{
    public UnitType UnitType;

    public AttackType AttackType;

    public int TroopsQuantity = 1;

    public float UnitSpeed;

    public int CarryCapacity;

    public float RespawnTime;
}
