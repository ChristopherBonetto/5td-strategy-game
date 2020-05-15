using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using BehaviorDesigner.Runtime;


public class EntityStatsSO : ScriptableObject
{
    [Header("Name")]
    public string Name;

    public Sprite Icon;

    public GameObject BehaviorHandler;
    public GameObject VisualPrefab;
    
    [Header("Defensive Stats")]
    public int MaxHp;
    public int Armor;
    public bool CanTakeDamage = false;
    
    [Header("Offensive Stats")]
    public int EngageRange;
    public int AttackRange;
    public float AttackSpeed;

    public int Damage;
    public bool CanAttack = false;

    [Space]
    public GameObject Projectile = null;

    [Header("Visual")]
    public Mesh Mesh;

    [Header("Upgrade")]
    public EntityStatsMultiplierSO[] Upgrades;
    public int Level { get; set; } = 0;
    public bool CanUpgrade { get { return Level < Upgrades.Length; } }

    [Header("Utility")]
    public QuantityOfResources Cost;


    public void Upgrade()
    {
        if (CanUpgrade)
        {
            EntityStatsMultiplierSO upgrade = Upgrades[Level];
            MaxHp += Mathf.RoundToInt(MaxHp * upgrade.MaxHpMultiplier);
            Armor += Mathf.RoundToInt(Armor * upgrade.ArmorMultuplier);
            EngageRange += Mathf.RoundToInt(EngageRange * upgrade.EngageRangeMultiplier);
            AttackRange += Mathf.RoundToInt(AttackRange * upgrade.AttackRangeMultiplier);
            AttackSpeed += AttackSpeed * upgrade.AttackSpeedMultiplier;
            Damage += Mathf.RoundToInt(Damage * upgrade.DamageMultiplier);

            Level++;
        }
    }
}
