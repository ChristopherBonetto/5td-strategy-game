using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Good North/Entity/Upgrade")]
public class EntityStatsMultiplierSO : ScriptableObject
{
    [Header("Defensive Stats")]
    public int MaxHpMultiplier;
    public int ArmorMultuplier;

    [Header("Offensive Stats")]
    public int EngageRangeMultiplier;
    public int AttackRangeMultiplier;
    public float AttackSpeedMultiplier;

    public int DamageMultiplier;

    [Header("Utility")]
    public QuantityOfResources Cost;
}
