using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;

public class EntitySO : ScriptableObject
{
    [Header("Name")]
    public string Name;
    public GameObject Prefab;
    
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

    [Header("Roles")]
    public EntitySO[] Roles;

    [Header("Utility")]
    public QuantityOfResources Cost;

}
