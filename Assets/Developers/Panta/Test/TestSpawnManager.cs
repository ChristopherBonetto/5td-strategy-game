using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSpawnManager : MonoBehaviour
{
    public HFSpawnerManager Manager;

    private void Start()
    {
        Manager.CallNextWave(Manager.SpawnEntityInRandomPosition, this.gameObject, Vector3.zero);
    }
}
