using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NewEntity", fileName = "Entity")]
public class EntityStatistics : ScriptableObject
{
    public new string EntityName = "";

    public EntityType EntityType;
    public EntityQualities m_EntityQuality { get; private set; }

    [Space]
    public float TimeOfUpgrade;

    [Space]
    public int HealthMax = 1;
    public int Attack = 1;
    public int Defence = 1;
    public float TimeToAttack = 1;
    public float MovementSpeed = 1;
    public int ViewRadius = 1;
    [Space]
    public Sprite EntitySprite;
    

    private void Awake()
    {
        SetQualityBasedOnType(EntityType);
    }

    public void SetQualityBasedOnType(EntityType type)
    {
        switch (type)
        {
            case EntityType.Soldier:
                m_EntityQuality = EntityQualities.Infantry | EntityQualities.Melee;
                break;
            case EntityType.Lancer:
                m_EntityQuality = EntityQualities.Infantry | EntityQualities.Melee;
                break;
            case EntityType.ArcherMedium:
                m_EntityQuality = EntityQualities.Infantry | EntityQualities.Ranged;
                break;
            case EntityType.ArcherLong:
                m_EntityQuality = EntityQualities.Infantry | EntityQualities.Ranged;
                break;
            case EntityType.Knight:
                m_EntityQuality = EntityQualities.Cavalry | EntityQualities.Melee;
                break;
            case EntityType.BowKnight:
                m_EntityQuality = EntityQualities.Cavalry | EntityQualities.Ranged;
                break;
        }
    }
}
