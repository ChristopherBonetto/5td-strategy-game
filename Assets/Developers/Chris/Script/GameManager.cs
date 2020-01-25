using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public CastleActions PlayerCastle; //{ get; private set; }
    

    private void Awake()
    {
        Instance = this;
    }
    

    public void SetObjective(CastleActions inCastle)
    {
        PlayerCastle = inCastle;
    }
    


    
}
