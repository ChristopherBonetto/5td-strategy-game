using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFWave
{
    /// </summary>
    /// collection of object to instantiate.
    /// it will be filled sorting some pool of troops.
    /// </summary>
    public Queue<GameObject> TroopsToSpawn;

    private int m_RemainingTroopsCount;
    public int RemainingTroopsCount
    {
        get { return m_RemainingTroopsCount; }
        set { m_RemainingTroopsCount = value; } // Trigger an event?
    }

    /// <summary>
    /// call next wave when previous troop spawn?
    /// call next wave when previous wave are death?
    /// </summary>
    public SpawnType SpawnType;

    /// <summary>
    /// how much time takes to call next wave?
    /// </summary>
    public float CallTime;


    public HFWave(float _callTime, SpawnType _type, params GameObject[] _objectsToSpawn)
    {
        CallTime = _callTime;
        SpawnType = _type;

        // Create and fill the list.
        TroopsToSpawn = new Queue<GameObject>();
        foreach (var item in _objectsToSpawn)
            TroopsToSpawn.Enqueue(item);

        RemainingTroopsCount = TroopsToSpawn.Count;
    }


    /// <summary>
    /// Get the next troop. F.I.F.O. system.
    /// </summary>
    /// <returns></returns>
    public GameObject GetNextTroopToSpawn()
    {
        if (TroopsToSpawn.Count > 0)
            return TroopsToSpawn.Dequeue();
        else
        {
            Debug.LogWarning("The wave is empty");
            return null;
        }
    }
}
