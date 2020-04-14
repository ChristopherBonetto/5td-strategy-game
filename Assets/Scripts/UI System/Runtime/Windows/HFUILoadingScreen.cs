using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

namespace HF.Refactoring
{

    public class HFUILoadingScreen : HFUIWindow
    {
        public override HFUIWindowID ID => HFUIWindowID.LOADING_SCREEN;

        [Header("Fade")]

        [SerializeField]
        private Image m_imageToFade;
        [SerializeField]
        private Text m_Text;


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
        public void LoadLevel(int levelIndex, bool showLoadingText = true)
        {
            // Enable this gameObject when the method is called.
            // Start the load methods in cascade wave.
            // 1) Fade in.
            // 2) Load progress...
            // 3) Fade out.

            OnShow();
            StartCoroutine(FadeIn(levelIndex,  showLoadingText));
        }

        /// <summary>
        /// Fade in the background...
        /// once completed start a new coroutine that load the scene.
        /// </summary>
        IEnumerator FadeIn(int levelIndex, bool showLoadingText = true)
        {
            while(m_imageToFade.color.a < 1f)
            {
                m_imageToFade.DOFade(1.1f, 0.6f);
                yield return null;
            }

            StartCoroutine(Load(levelIndex, showLoadingText));
            yield return null;
        }

        /// <summary>
        /// Load the scene.
        /// </summary>
        IEnumerator Load(int levelIndex, bool showLoadingText = true)
        {
            m_Text.gameObject.SetActive(showLoadingText);
            HFScenesManager.Instance.LoadSceneFromIndex(levelIndex);

            yield return null;
        }

        /// <summary>
        /// Handle the fade out at the right moment.
        /// </summary>
        private void OnGameStateChange(GameStates inState)
        {
                Debug.Log(inState);
            // Handle all game state variables.
            switch (inState)
            {

                case GameStates.WarRoom:
                    StartCoroutine(FadeOut());
                    break;
                case GameStates.InitializeLevel:
                    break;
                case GameStates.PlayingLevel:
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

            while (m_imageToFade.color.a > 0f)
            {
                DOTweenModuleUI.DOFade(m_imageToFade, -0.1f, 0.6f);
                yield return null;
            }

            OnHide();
            yield return null;
        }
    }
}
