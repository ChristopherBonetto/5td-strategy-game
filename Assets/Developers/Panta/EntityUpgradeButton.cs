using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HF.Refactoring;

public class EntityUpgradeButton : MonoBehaviour
{
    [Header("Don't need reference from inspector")]
    public EntityBehavior Entity;

    [Header("Set up")]
    public Vector3 Offset;
    public Button UpgradeButton;
    public Image ButtonImage;
    

    private void Update()
    {
        if (Entity != null)
        {
            transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, Entity.transform.position + Offset);
        }
    }

    public void SetUpgradeButton(EntityBehavior entity)
    {
        //gameObject.SetActive(true);

        //Entity = entity;

        //if (Entity.GetStats().CanUpgrade) // && has enough money
        //{
        //    ButtonImage.color = Color.white;
        //    UpgradeButton.onClick.AddListener(() => entity.GetStats().Upgrade());
        //}
        //else //!canUpgrade || not enough money
        //{
        //    ButtonImage.color = Color.grey;
        //    //UpgradeButton.onClick.AddListener(/*trigger event => can't be upgraded*/);
        //}
    }
}
