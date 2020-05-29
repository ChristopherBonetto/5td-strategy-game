using UnityEngine;
using UnityEngine.UI;
using HF;

public class HFPlayerRewardUIElement : MonoBehaviour
{
    public Image RewardIcon;
    public Text RewardText;

    [SerializeField]
    private HFPulseScale m_scaleComponent;

    private void OnEnable()
    {
        ResetValue();
        HFEventManager.SubscribeTo<int,bool>(HFEventID.OnGemChanged, OnGainReward);
    }

    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom<int,bool>(HFEventID.OnGemChanged, OnGainReward);
    }

    public void ResetValue()
    {
        RewardText.text = "0";
    }

    public void OnGainReward(int value, bool inGained)
    {
        RewardText.text = value.ToString();
        if (inGained)
        {
            StartCoroutine(m_scaleComponent.Pulse());
        }
        
    }
}
