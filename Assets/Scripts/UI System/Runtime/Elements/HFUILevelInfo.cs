using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HFUILevelInfo : MonoBehaviour
{
    private Text m_text;

    private void Awake()
    {
        m_text = GetComponent<Text>();
    }

    private void OnEnable()
    {
        HFEventManager.SubscribeTo<GameStates>(HFEventID.OnGameStateChanged, OnChangeState);
    }

    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom<GameStates>(HFEventID.OnGameStateChanged, OnChangeState);
    }

    private void OnChangeState(GameStates state)
    {
        switch (state)
        {
            case GameStates.PlayingLevel:
                m_text.text = "Level " + (HFScenesManager.Instance.CurrentLevelSelected.LevelSceneIndex - 1).ToString();
                break;

            default:
                m_text.text = "";
                break;
        }
    }
}
