using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "New Gm State", fileName = "Gm State")]
public class HFGMStateSO : ScriptableObject
{
    [SerializeField] private GameStates m_gameState;
    public GameStates GameState
    {
        get
        {
            return m_gameState;
        }
    }

    [SerializeField] List<GameStates> m_possibleSwtichStates;

    public List<GameStates> PossibleSwitchStates
    {
        get
        {
            return m_possibleSwtichStates;
        }
    }
}
