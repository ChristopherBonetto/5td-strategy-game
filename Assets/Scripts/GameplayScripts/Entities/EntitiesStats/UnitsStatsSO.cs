using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;

[CreateAssetMenuAttribute(fileName = "New unit data", menuName = "EntityData/Unit")]
public class UnitsStatsSO : EntityStatsSO
{
    public Sprite Icon;

    public UnitType UnitType;

    public AttackType AttackType;

    public int UnitQuantity = 1;

    public float UnitSpeed;

    public int CarryCapacity;

    public float RespawnTime;

    public int GemDropAmount = 8;

    [FMODUnity.EventRef]
    public string LiftSound, DropSound;
}
