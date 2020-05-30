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
        public Image GemsIcon;
        public Text GemsText;

        public Image VictoryBanner;

        public Sequence m_enableSequence;
        Sequence m_disableSequence;


        private void Awake()
        {
            m_enableSequence = DOTween.Sequence();
            m_disableSequence = DOTween.Sequence();

            m_enableSequence
                .AppendCallback(() => SetActiveElements(true, FadeBackground.gameObject, VictoryBanner.gameObject, GemsIcon.gameObject, GemsText.gameObject))
                .Append(FadeBackground.DOColor(Color.black, 1f))
                .Append(VictoryBanner.transform.DOScale(1f, 1f))
                .Append(GemsIcon.transform.DOScale(1f, 1f))
                .Append(GemsText.transform.DOScale(1f, 1f))
                .AppendCallback(() => SetActiveElements(false, FadeBackground.gameObject, VictoryBanner.gameObject, GemsIcon.gameObject, GemsText.gameObject))
                .AppendCallback(() => HFUIManager.Instance.Getwindow<HFUILoadingScreen>(HFUIWindowID.LOADING_SCREEN).LoadLevel(1))
                .Pause();
            m_enableSequence.SetAutoKill(false);

            m_disableSequence
                .Append(VictoryBanner.transform.DOScale(0f, .2f)).SetDelay(.3f)
                .Append(FadeBackground.DOColor(new Color(0, 0, 0, 0), .3f)).SetDelay(1f)
                .Append(GemsIcon.transform.DOScale(0f, .2f)).SetDelay(1f)
                .Append(GemsText.transform.DOScale(0f, .2f)).SetDelay(1f)
                .AppendCallback(() => SetActiveElements(false, VictoryBanner.gameObject, GemsIcon.gameObject, GemsText.gameObject))
                .Pause();
            m_disableSequence.SetAutoKill(false);
        }


        private void OnEnable()
        {
            InitializeElements();
        }

        private void InitializeElements()
        {
            FadeBackground.gameObject.SetActive(false);
            FadeBackground.color = Color.clear;

            GemsIcon.gameObject.SetActive(false);
            GemsIcon.transform.localScale = Vector3.zero;
            
            GemsText.gameObject.SetActive(false);
            GemsText.transform.localScale = Vector3.zero;

            VictoryBanner.gameObject.SetActive(false);
            VictoryBanner.transform.localScale = Vector3.zero;
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
