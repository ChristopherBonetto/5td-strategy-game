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
        PEASANT,
        LIFTER,
        DEFENDER,
        RUNNER
    }

    public enum TowerType
    {
        BALLISTA,
        CANNON,
        ENERGY
    }

    public enum BuildingType
    {
        CASTLE,
        TOWER
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

    public enum ResourceType
    {
        Gold,
        Gems
    }

    public enum PlayerType
    {
        Player,
        AI
    }
}
