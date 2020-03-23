using UnityEngine;
using UnityEngine.UI;
using HF;

public class HFPlayerRewardUIElement : MonoBehaviour
{
    public Image RewardIcon;
    public Text RewardText;

	private int m_value;

    private void OnEnable()
    {
        HFEventManager.SubscribeTo<int, HFUnit>(HFEventID.GainReward, OnGainReward);
    }

    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom<int, HFUnit>(HFEventID.GainReward, OnGainReward);
    }

    private void Start()
    {
        ResetValue();
    }

    public void ResetValue()
    {
		m_value = 0;
        RewardText.text = "0";
    }

    public void OnGainReward(int value, HFUnit unit)
    {
		m_value += value;
        RewardText.text = m_value.ToString();
    }
}
