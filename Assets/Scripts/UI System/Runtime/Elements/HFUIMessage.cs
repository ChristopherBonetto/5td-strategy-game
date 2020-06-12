using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HFUIMessage : MonoBehaviour
{
    public Text TextComponent;
    public float FadeDuration = 2;

    public void SetMessage(string message)
    {
        TextComponent.text = message;
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        TextComponent.DOFade(1, FadeDuration).
            OnComplete(() => TextComponent.DOFade(0, FadeDuration)).
            OnComplete(() => gameObject.SetActive(false));
    }

    private void OnDisable()
    {
        TextComponent.text = "";
    }
}
