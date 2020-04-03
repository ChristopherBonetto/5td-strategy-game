using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class HFLoadingScreenWindow : HFUIControl
{
    public override UIControlID Name => UIControlID.LoadingScreen;

    [Header("Fade")]

    [SerializeField]
    private Image m_imageToFade;
    [SerializeField]
    private Text m_Text;

    /// <summary>
    /// The minimum time to show this screen.
    /// </summary>
    public const float MinTimeToShow = 1f;

    /// <summary>
    /// Take care bout time elapsed during loading screen
    /// </summary>
    private float m_timeElapsed;


    private void OnEnable()
    {
        // Listen to game manager state changes
        HFEventManager.SubscribeTo<GameStates>(HFEventID.OnGameStateChanged, OnGameStateChange);
    }

    private void OnDisable()
    {
        //Stop to listen to game manager state changes
        HFEventManager.UnsubscribeFrom<GameStates>(HFEventID.OnGameStateChanged, OnGameStateChange);
    }

    /// <summary>
    /// Call this to load level from war room
    /// </summary>
    public void LoadLevel(int levelIndex)
    {
        // Enable this gameObject when the method is called.
        // Start the load methods in cascade wave.
        // 1) Fade in.
        // 2) Load progress...
        // 3) Fade out.

        OnShow();
        StartCoroutine(FadeIn(levelIndex));
    }

    /// <summary>
    /// Fade in the background...
    /// once completed start a new coroutine that load the scene.
    /// </summary>
    IEnumerator FadeIn(int levelIndex)
    {
        HFUIManager.Instance.LastUIControlShown.OnHide();

        while(m_imageToFade.color.a < 0.9f)
        {
            DOTweenModuleUI.DOFade(m_imageToFade, 1, 0.4f);
            yield return null;
        }

        StartCoroutine(Load(levelIndex));
        yield return null;
    }

    /// <summary>
    /// Load the scene.
    /// </summary>
    IEnumerator Load(int levelIndex)
    {
        m_timeElapsed = 0;
        m_Text.gameObject.SetActive(true);
        HFScenesManager.Instance.LoadSceneFromIndex(levelIndex);

        while(m_timeElapsed < MinTimeToShow)
        {
            // here wait the bank loadeing.
            m_timeElapsed += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }

    /// <summary>
    /// Handle the fade out at the right moment.
    /// </summary>
    private void OnGameStateChange(GameStates inState)
    {
        // Handle all game state variables.
        switch (inState)
        {
            case GameStates.InitializeLevel:
                StartCoroutine(FadeOut());
                break;
        }
    }

    /// <summary>
    /// Fade out the background, 
    /// Called when OnGameStateChange is triggered.
    /// </summary>
    IEnumerator FadeOut()
    {
        m_Text.gameObject.SetActive(false);

        while (m_imageToFade.color.a > 0.1f)
        {
            DOTweenModuleUI.DOFade(m_imageToFade, 0, 0.4f);
            yield return null;
        }

        OnHide();
        yield return null;
    }
}
