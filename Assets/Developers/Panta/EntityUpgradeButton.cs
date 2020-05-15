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


    private void OnEnable()
    {
        HFEventManager.SubscribeTo<EntityBehavior, int>(HFEventID.OnUnitUpgraded, OnUnitupgraded);
    }

    private void OnDisable()
    {

        HFEventManager.UnsubscribeFrom<EntityBehavior, int>(HFEventID.OnUnitUpgraded, OnUnitupgraded);
    }

    private void Update()
    {
        if (Entity != null)
        {
            transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, Entity.transform.position + Offset);
        }
    }

    public void OnUnitupgraded(EntityBehavior entity, int team)
    {
        gameObject.SetActive(false);
        SetUpgradeButton(Entity);
    }

    public void SetUpgradeButton(EntityBehavior entity)
    {
        gameObject.SetActive(true);

        Entity = entity;

        if (entity is Troop)
        {
            Troop troop = entity.GetComponent<Troop>();

            if (troop.GetStats().CanUpgrade) // && has enough money
            {
                ButtonImage.color = Color.white;
                UpgradeButton.onClick.AddListener(() => troop.GetStats().Upgrade());
            }
            else //!canUpgrade || not enough money
            {
                ButtonImage.color = Color.grey;
                //UpgradeButton.onClick.AddListener(/*trigger event => can't be upgraded*/);
            }
        }
        else if (entity is BuildingBehaviour)
        {
            BuildingBehaviour building = entity.GetComponent<BuildingBehaviour>();

            if (building.GetStats().CanUpgrade) // && has enough money
            {
                ButtonImage.color = Color.white;
                UpgradeButton.onClick.AddListener(() => building.GetStats().Upgrade());
            }
            else //!canUpgrade || not enough money
            {
                ButtonImage.color = Color.grey;
                //UpgradeButton.onClick.AddListener(/*trigger event => can't be upgraded*/);
            }
        }
    }
}
