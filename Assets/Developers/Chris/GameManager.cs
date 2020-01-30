using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStates
{
    StartGame,
    LevelSelection,
    InitializeLevel,
    PlayLevel,
    Settings,
    EndLevel,
    Pause
}

public class GameManager : OtherSingleton<GameManager>
{
    public GameStates CurrentGameState = GameStates.StartGame;

    
}
