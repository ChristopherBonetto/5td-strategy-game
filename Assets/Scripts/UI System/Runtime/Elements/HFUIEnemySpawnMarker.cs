using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HFUIEnemySpawnMarker : MonoBehaviour
{
    public Image ImageMarker;
    public Transform MarkDestination;
    private Tween scale;

    private void Awake()
    {
        scale = transform.DOScale(2f, .5f).SetLoops(-1, LoopType.Yoyo);
    }

    private void Update()
    {
        if (MarkDestination != null)
            SetEnemySpawnMarker();
    }

    private void OnEnable()
    {
        scale.Restart();
    }

    public void SetDestinationMarker(Transform transform)
    {
        MarkDestination = transform;
    }

    public void SetEnemySpawnMarker()
    {
        if (RectTransformUtility.WorldToScreenPoint(Camera.main, MarkDestination.position).x < 0 ||
            RectTransformUtility.WorldToScreenPoint(Camera.main, MarkDestination.position).x > Screen.width ||
            RectTransformUtility.WorldToScreenPoint(Camera.main, MarkDestination.position).y < 0 ||
            RectTransformUtility.WorldToScreenPoint(Camera.main, MarkDestination.position).y > Screen.height)
        {
            if (!ImageMarker.isActiveAndEnabled) ImageMarker.enabled = true;

            ImageMarker.rectTransform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, MarkDestination.position);
            Vector3 screenPosition = ImageMarker.rectTransform.position;
            screenPosition = new Vector3(
                Mathf.Clamp(screenPosition.x, 0 + ImageMarker.rectTransform.rect.size.x * .5f, Screen.width - ImageMarker.rectTransform.rect.size.x * .5f),
                Mathf.Clamp(screenPosition.y, 0 + ImageMarker.rectTransform.rect.size.y * .5f, Screen.height - ImageMarker.rectTransform.rect.size.y * .5f),
                0);
            ImageMarker.rectTransform.position = screenPosition;
        }
        else
        {
            if (ImageMarker.isActiveAndEnabled) ImageMarker.enabled = false;
        }
    }
}
