using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Good North/Entity/Upgrade")]
public class EntityStatsMultiplierSO : ScriptableObject
{
    [Header("Defensive Stats")]
    public float MaxHpMultiplier;
    public float ArmorMultuplier;

    [Header("Offensive Stats")]
    public float EngageRangeMultiplier;
    public float AttackRangeMultiplier;
    public float AttackSpeedMultiplier;

    public float DamageMultiplier;

    [Header("Utility")]
    public QuantityOfResources Cost;
}
