using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Types
{
    public enum AttackType
    {
        MELEE,
        RANGED
    }

    public enum UnitType
    {
        STANDARD_ALLY = 0,
        STANDARDENEMY = 7,

        // Upgrade

        // Lifter
        LIFTER_LVL1 = 8,
        LIFTER_LVL2 = 9,
        LIFTER_LVL3 = 10,

        // Defender
        DEFENDER_LVL1 = 11,
        DEFENDER_LVL2 = 12,
        DEFENDER_LVL3 = 13,

        // Runner
        RUNNER_LVL1 = 14,
        RUNNER_LVL2 = 15,
        RUNNER_LVL3 = 16,

        // Warrior
        WARRIOR_LVL1 = 17,
        WARRIOR_LVL2 = 18,
        WARRIOR_LVL3 = 19,

        // Archer
        ARCHER_LVL1 = 20,
        ARCHER_LVL2 = 21,
        ARCHER_LVL3 = 22,

        //Tank
        TANK_LVL1 = 23,
        TANK_LVL2 = 24,
        TANK_LVL3 = 25,

        //Spearmean
        SPEARMAN_LVL1 = 26,
        SPEARMAN_LVL2 = 27,
        SPEARMAN_LVL3 = 28,

        //Standard enemy
        STANDARDENEMY_LVL1 = 29,
        STANDARDENEMY_LVL2 = 30,
        STANDARDENEMY_LVL3 = 31,
    }

    public enum BuildingType
    {
        CASTLE,
        TOWER,
        BALLISTA_LVL1,
        BALLISTA_LVL2,
        BALLISTA_LVL3,
        MORTAR_LVL1,
        MORTAR_LVL2,
        MORTAR_LVL3,
        CRYSTAL_LVL1,
        CRYSTAL_LVL2,
        CRYSTAL_LVL3,
    }

    [System.Flags]
    public enum Qualities
    {
        None = 0,
        Melee = 1 << 0,
        Ranged = 1 << 1,
        Infantry = 1 << 2,
        Cavalry = 1 << 3,
    }

    public enum PlayerType
    {
        Player,
        AI
    }

}
