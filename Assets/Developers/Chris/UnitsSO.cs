using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;

[CreateAssetMenuAttribute(fileName = "New unit data", menuName = "EntityData/Unit")]
public class UnitsSO : EntitySO
{
    public UnitType UnitType;

    public AttackType AttackType;

    public float UnitSpeed;

    public int CarryCapacity;

    public float RespawnTime;


}
