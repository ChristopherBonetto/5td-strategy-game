using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private CivilizationAction PlayerCivilization;
    [SerializeField] private CivilizationAction EnemyCivilization;


    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        InstantiateEntities();
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }


    public void InstantiateEntities()
    {
        for(int i = 0; i < PlayerCivilization.CivilizationSpawnPoint.Length; i++)
        {
            PlayerCivilization.InstantiateEntityFromType(EntityType.Soldier, PlayerCivilization.CivilizationSpawnPoint[i]);
        }

        for (int i = 0; i < PlayerCivilization.CivilizationSpawnPoint.Length; i++)
        {
            EnemyCivilization.InstantiateEntityFromType(EntityType.Soldier, EnemyCivilization.CivilizationSpawnPoint[i]);
        }
    }
}
