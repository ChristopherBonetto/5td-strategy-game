using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "NewUnit", fileName = "Unit")]
public class UnitStatistics : ScriptableObject
{
    public new string UnitName = "";
    public Units UnitType;
    public UnitQualities m_UnitQuality { get; private set; }

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
    public GameObject UnitPrefab;


    private void Awake()
    {
        SetQualityBasedOnType(UnitType);
    }

    public void SetQualityBasedOnType(Units type)
    {
        switch (type)
        {
            case Units.Farmer:
                m_UnitQuality = UnitQualities.Infantry | UnitQualities.Melee;
                break;
            case Units.Defender:
                m_UnitQuality = UnitQualities.Infantry | UnitQualities.Melee;
                break;
            case Units.Lifter:
                m_UnitQuality = UnitQualities.Infantry | UnitQualities.Melee;
                break;
            case Units.Runner:
                m_UnitQuality = UnitQualities.Infantry | UnitQualities.Ranged;
                break;
        }
    }
    }

