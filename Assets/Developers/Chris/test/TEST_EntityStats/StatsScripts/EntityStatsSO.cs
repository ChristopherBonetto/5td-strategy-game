using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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

    [Header("Utility")]
    public QuantityOfResources Cost;
}
