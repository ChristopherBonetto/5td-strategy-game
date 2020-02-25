using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HF;

public class HFPlayerRewardUIElement : MonoBehaviour
{
    public Image RewardIcon;
    public Text RewardText;

    private void OnEnable()
    {
        HFEventManager.SubscribeTo<float, HFUnit>(HFEventID.GainReward, OnGainReward);
    }

    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom<float, HFUnit>(HFEventID.GainReward, OnGainReward);
    }

    private void Start()
    {
        ResetValue();
    }

    public void ResetValue()
    {
        RewardText.text = "0";
    }

    public void OnGainReward(float value, HFUnit unit)
    {
        RewardText.text += ((int)value).ToString();
    }
}
