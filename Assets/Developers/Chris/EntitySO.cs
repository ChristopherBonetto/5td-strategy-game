using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;

public class EntitySO : ScriptableObject
{
    [Header("Name")]
    public string Name;

    
    [Header("Defensive Stats")]
    public int maxHp = 0;
    public int MaxHp
    {
        get
        {
            return maxHp;
        }
        set
        {
            maxHp = value;

            if(maxHp == 0)
            {
                canTakeDamage = false;
            }
            else if(maxHp > 0)
            {
                canTakeDamage = true;
            }
        }
    }

    public int Armor;
    protected bool canTakeDamage = false;


    
    [Header("Offensive Stats")]
    public int EngageRange;
    public int AttackRange;
    public float AttackSpeed;

    public int damage = 0;
    public int Damage
    {
        get
        {
            return damage;
        }
        set
        {
            damage = value;

            if(damage == 0)
            {
                canAttack = false;
            }
            else if(damage > 0)
            {
                canAttack = true;
            }
        }
    }

    protected bool canAttack = false;

    [Space]
    public GameObject Projectile = null;



    [Header("Visual")]
    public Mesh Mesh;

    [Header("Roles")]
    public EntitySO[] Roles;

    [Header("Utility")]
    public int Cost;



    public bool CanTakeDamage()
    {
        return canTakeDamage;
    }

    public bool CanAttack()
    {
        return canAttack;
    }
}
