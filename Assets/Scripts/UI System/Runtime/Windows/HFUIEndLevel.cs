using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace HF.Refactoring
{
    public class HFUIEndLevel : HFUIWindow
    {
        public override HFUIWindowID ID => HFUIWindowID.LEVEL_ENDING;

        public Image FadeBackground;

        public Image VictoryBanner;
        public Image DefeatBanner;
        public HFLoadSceneB RestartLevelButton;
        public HFLoadSceneB NextLevelButton;
        public HFLoadSceneB ReturnToMainMenuButton;
        public Color victoryBackgroundColor;
        public Color defeatBackgroundColor;
     


        public Sequence m_victorySequence;
        public Sequence m_defeatSequence;

        private void Awake()
        {
            m_victorySequence = DOTween.Sequence();
            m_defeatSequence = DOTween.Sequence();

            m_victorySequence
                .AppendCallback(() => SetActiveElements(true, FadeBackground.gameObject, VictoryBanner.gameObject))
                .Append(FadeBackground.DOColor(victoryBackgroundColor, 1f))
                .Append(VictoryBanner.transform.DOScale(1f, 1f))
                .AppendCallback(() => SetActiveElements(true, /*RestartLevelButton.gameObject, NextLevelButton.gameObject,*/ ReturnToMainMenuButton.gameObject))
                .AppendCallback(() =>
                {
                    if (HFGameManager.Instance.CurrentGameState != GameStates.EndLevel)
                        return;
                    HFUIManager.Instance.TryGetWindow(HFUIWindowID.LEVEL_ENDING);
                })
                .Pause();
            m_victorySequence.SetAutoKill(false);

            m_defeatSequence
                .AppendCallback(() => SetActiveElements(true, FadeBackground.gameObject, DefeatBanner.gameObject))
                .Append(FadeBackground.DOColor(defeatBackgroundColor, 1f))
                .Append(DefeatBanner.transform.DOScale(1f, 1f))
                .AppendCallback(() => SetActiveElements(true, /*RestartLevelButton.gameObject,*/ ReturnToMainMenuButton.gameObject))
                .AppendCallback(() =>
                {
                    if (HFGameManager.Instance.CurrentGameState != GameStates.EndLevel)
                        return;
                    HFUIManager.Instance.TryGetWindow(HFUIWindowID.LEVEL_ENDING);
                })
                .Pause();
            m_defeatSequence.SetAutoKill(false);
        }


        private void OnEnable()
        {
            InitializeElements();
        }

        private void InitializeElements()
        {
            FadeBackground.gameObject.SetActive(false);
            FadeBackground.color = Color.clear;

            RestartLevelButton.gameObject.SetActive(false);
            RestartLevelButton.Sceneindex = HFScenesManager.Instance.IndexCurrentScene;

            NextLevelButton.gameObject.SetActive(false);
            NextLevelButton.Sceneindex = HFScenesManager.Instance.IndexCurrentScene + 1;    // How can I control if is it null?

            ReturnToMainMenuButton.gameObject.SetActive(false);

            VictoryBanner.gameObject.SetActive(false);
            VictoryBanner.transform.localScale = Vector3.zero;

            DefeatBanner.gameObject.SetActive(false);
            DefeatBanner.transform.localScale = Vector3.zero;
        }

        private void SetActiveElements(bool active, params GameObject[] objects)
        {
            foreach (var item in objects)
            {
                item.SetActive(active);
            }
        }
    }
}
