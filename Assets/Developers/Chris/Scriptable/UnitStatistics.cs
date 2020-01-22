using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NewUnit", fileName = "UnitInfo")]
public class UnitStatistics : ScriptableObject
{
    public new string UnitName = "";

    public Units UnitType;
    public UnitQualities m_UnitQuality { get; private set; }

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
    public Sprite UnitSprite;
    

    private void Awake()
    {
        SetQualityBasedOnType(UnitType);
    }

    public void SetQualityBasedOnType(Units type)
    {
        switch (type)
        {
            case Units.Soldier:
                m_UnitQuality = UnitQualities.Infantry | UnitQualities.Melee;
                break;
            case Units.Lancer:
                m_UnitQuality = UnitQualities.Infantry | UnitQualities.Melee;
                break;
            case Units.ArcherMedium:
                m_UnitQuality = UnitQualities.Infantry | UnitQualities.Ranged;
                break;
            case Units.ArcherLong:
                m_UnitQuality = UnitQualities.Infantry | UnitQualities.Ranged;
                break;
            case Units.Knight:
                m_UnitQuality = UnitQualities.Cavalry | UnitQualities.Melee;
                break;
            case Units.BowKnight:
                m_UnitQuality = UnitQualities.Cavalry | UnitQualities.Ranged;
                break;
        }
    }
}
