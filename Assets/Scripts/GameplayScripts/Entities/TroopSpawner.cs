using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;

public class TroopSpawner : MonoBehaviour
{
    [SerializeField] private UnitType unitType;
    [SerializeField] private PlayerType playerType;

    // Start is called before the first frame update
    void Start()
    {
        GameController.Instance.CreateNewTroop(unitType, playerType, transform.position, true);
    }
}
