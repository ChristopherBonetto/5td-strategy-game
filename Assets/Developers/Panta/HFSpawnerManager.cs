using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Choose "When" spawn something.
/// </summary>
public enum SpawnType
{
    PRE_DELAY = 0,
    POST_DELAY = 1,

    MAX_SPAWN_TYPE,
}

/// <summary>
/// Allow us to split variable based no faction.
/// </summary>
public enum Faction
{
    ENEMY = 0,
    ALLY = 1,
    NEUTRAL = 2,

    MAX_FACTION,
}

public class HFSpawnerManager : Singleton<HFSpawnerManager>
{
    [Header("Inspector Assign")]
    public List<HFSpawner> Points;  // @TEMP

    /// <summary>
    /// Store all enemy and ally spawn positions.
    /// </summary>
    public Dictionary<Faction, List<HFSpawner>> Spawners;
    /// <summary>
    /// Store the generated waves in a queue.
    /// </summary>
    public Dictionary<Faction, Queue<HFWave>> Waves;    // I assume that each wave must be wait the previous one to spawn.
    /// <summary>
    /// Take reference to the current wave.
    /// </summary>
    public Dictionary<Faction, HFWave> CurrentWave;


    #region METHODS

    private void Start()
    {
        InitSpawnerDictionary();
        InitWavesDictionary();

        // @TEMP
        AddSpawner(Faction.ENEMY, Points); 
        AddWave(Faction.ENEMY, new HFWave(2, SpawnType.POST_DELAY, Points[0].gameObject));
    }

    private void Update()
    {
        // @TEMP
        if (Input.GetKeyDown(KeyCode.Space))
            CallNextWave(Faction.ENEMY);
    }

    #region SPAWNER MANAGEMENT
    private void InitSpawnerDictionary()
    {
        // Create an instance of dictionary
        Spawners = new Dictionary<Faction, List<HFSpawner>>();

        for (int i = 0; i < (int)Faction.MAX_FACTION; i++)
        {
            // Create a new list instance for every key.
            Spawners[(Faction)i] = new List<HFSpawner>();
            Debug.Log((Faction)i);
        }
    }

    public void AddSpawner(Faction faction, List<HFSpawner> spawner) // change the list parameter with a single value.
    {
        for (int i = 0; i < spawner.Count; i++) // delete this row of code later...
            Spawners[faction].Add(spawner[i]);
    }

    public void RemoveSpawner(Faction faction, HFSpawner spawner)
    {
        Spawners[faction].Remove(spawner);
    }

    public void ClearDictionary()
    {
        for (int i = 0; i < (int)Faction.MAX_FACTION; i++)
        {
            // Clear the list for every key.
            Spawners[(Faction)i].Clear();
        }
    }
    #endregion

    #region WAVES MANAGEMENT
    private void InitWavesDictionary()
    {
        // Create an instance of dictionaries
        CurrentWave = new Dictionary<Faction, HFWave>();
        Waves = new Dictionary<Faction, Queue<HFWave>>();

        for (int i = 0; i < (int)Faction.MAX_FACTION; i++)
        {
            // Create a new list instance for every key.
            Waves[(Faction)i] = new Queue<HFWave>();
        }
    }

    public void AddWave(Faction faction, HFWave wave)
    {
        Waves[faction].Enqueue(wave);
    }

    public void CallNextWave(Faction faction)
    {
        CurrentWave[faction] = Waves[faction].Dequeue();

        // Init new timer.
        HFTimer timer = new HFTimer(CurrentWave[faction].CallTime);
        // when timer go to 0, invoke the method => Spawn New Wave.
        StartCoroutine(timer.DecreaseTime<Faction>(SpawnNextWave, faction));
    }

    public void SpawnNextWave(Faction faction)
    {
        if (CurrentWave[faction] != null)
        {
            // Get random spawn position.
            HFSpawner spawnPoint;
            int randomValue = Random.Range(0, Spawners[faction].Count);

            // Check variables.
            List<HFSpawner> checkedSpawner = new List<HFSpawner>();

            // if the this picked spawner is free...
            if (!Spawners[faction][randomValue].IsAlreadyEmployed)
            {
                // Assign the troop to the spawner.

                Debug.Log(randomValue);
                return;
            }

            // if this picked spawn is already employed...
            else if (Spawners[faction][randomValue].IsAlreadyEmployed)
            {
                // if it's not already checked before...
                if (!checkedSpawner.Contains(Spawners[faction][randomValue]))
                {
                    // Add this spawn to the check list
                    checkedSpawner.Add(Spawners[faction][randomValue]);
                    // if all spawn available are checked and no one is free
                    // then return.
                    if (checkedSpawner.Count == Spawners[faction].Count)
                    {
                        Debug.LogWarning("There aren't any available spawners");
                        return;
                    }
                }

                // search recursively.
                SpawnNextWave(faction);
            }
        }
    }
    #endregion

    #endregion
}
