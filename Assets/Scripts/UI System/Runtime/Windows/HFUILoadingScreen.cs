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

        public float MinLoadingTime = 3f;

        [SerializeField]
        private bool m_RandomTip = true;
        public bool RandomTip { get => m_RandomTip; set => m_RandomTip = value; }

        [SerializeField]
        private CanvasGroup m_canvasGroupComponentRef;
        [SerializeField]
        private Image m_imageComponentRef;
        [SerializeField]
        private Text m_textTipRef;
        [SerializeField]
        private Text m_Text;
        [SerializeField]
        private Sprite[] TipImages;
        [SerializeField, TextArea]
        private String[] TipTexts;


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

            GetTip(RandomTip);
            OnShow();
            StartCoroutine(FadeIn(levelIndex,  showLoadingText));
        }

        /// <summary>
        /// Fade in the background...
        /// once completed start a new coroutine that load the scene.
        /// </summary>
        IEnumerator FadeIn(int levelIndex, bool showLoadingText = true)
        {
            while(m_canvasGroupComponentRef.alpha < 1f)
            {
                m_canvasGroupComponentRef.DOFade(1.1f, 0.6f);
                yield return null;
            }

            float currentDelay = 0f;
            while (currentDelay < MinLoadingTime)
            {
                currentDelay += Time.deltaTime;
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

            while (m_canvasGroupComponentRef.alpha > 0f)
            {
                DOTweenModuleUI.DOFade(m_canvasGroupComponentRef, -0.1f, 0.6f);
                yield return null;
            }

            OnHide();
            yield return null;
        }

        private void GetTip(bool random = false)
        {
            // Select an image tip
            if (TipImages != null && TipImages.Length > 0)
            {
                if (random)
                    m_imageComponentRef.sprite = TipImages[UnityEngine.Random.Range(0, TipImages.Length)];
                else
                    m_imageComponentRef.sprite = TipImages[HFScenesManager.Instance.IndexCurrentScene % TipImages.Length];
            }
            else
                m_imageComponentRef.sprite = null;

            // Select a text tip
            if (TipTexts != null && TipTexts.Length > 0)
            {
                if (random)
                    m_textTipRef.text = TipTexts[UnityEngine.Random.Range(0, TipTexts.Length)];
                else
                    m_textTipRef.text = TipTexts[HFScenesManager.Instance.IndexCurrentScene % TipTexts.Length];
            }
            else
                m_textTipRef.text = "";
        }
    }
}
