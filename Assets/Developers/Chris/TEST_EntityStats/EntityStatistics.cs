using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "NewEntity", fileName = "Entity")]
public class EntityStatistics : ScriptableObject
{
    public new string EntityName = "";
    public EntityType EntityType;

    [Header("Unit Stats")]
    [Space]

    public int HealthMax = 1;
    public int Attack = 1;
    public int Defence = 1;
    public float AttackSpeed = 1;
    public float MovementSpeed = 1;
    public int Range = 1;
    public int CarryCapacity = 1;

    [Space,Space,Space]
    public GameObject EntityPrefab;
}

