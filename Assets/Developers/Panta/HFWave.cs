using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFWave
{
    // collection of object to instantiate.
    // it will be filled sorting some pool of troops.
    public List<GameObject> TroopsToSpawn;

    public int RemainingTroopsCount;

    // call next wave when previous troop spawn?
    // call next wave when previous wave are death?
    public SpawnType SpawnType;

    // how much time takes to call next wave?
    public float CallTime;


    public HFWave(float _callTime, SpawnType _type, params GameObject[] _objectsToSpawn)
    {
        CallTime = _callTime;
        SpawnType = _type;

        // Create and fill the list.
        TroopsToSpawn = new List<GameObject>();
        foreach (var item in _objectsToSpawn)
            TroopsToSpawn.Add(item);

        RemainingTroopsCount = TroopsToSpawn.Count;
    }
}
