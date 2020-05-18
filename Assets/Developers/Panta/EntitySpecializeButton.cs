using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EntitySpecializeButton : MonoBehaviour
{
    [Header("Don't need reference from inspector")]
    public EntityBehavior Entity;

    [Header("Set up")]
    public Vector3 Offset;
    public Button[] SpecializeButton;
    public Image[] ButtonImage;


    private void OnEnable()
    {
        HFEventManager.SubscribeTo<EntityBehavior, int>(HFEventID.OnUnitSpecialized, OnUnitSpecialized);
    }

    private void OnDisable()
    {
        
        HFEventManager.UnsubscribeFrom<EntityBehavior, int>(HFEventID.OnUnitSpecialized, OnUnitSpecialized);
    }

    private void Update()
    {
        if (Entity != null)
        {
            transform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, Entity.transform.position + Offset);
        }
    }

    public void OnUnitSpecialized(EntityBehavior entity, int team)
    {
        gameObject.SetActive(false);
    }

    public void SetSpecializeButton(EntityBehavior entity)
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        transform.DOScale(1, 0.3f);

        SpecializeButton[0].onClick.RemoveAllListeners();
        SpecializeButton[1].onClick.RemoveAllListeners();
        SpecializeButton[2].onClick.RemoveAllListeners();

        Entity = entity;

        if (Entity is Troop)
        {
            ButtonImage[0].sprite = GameController.Instance.Collection.UnitsDictionary[Types.UnitType.LIFTER].OriginalUnitStats.Icon;
            ButtonImage[1].sprite = GameController.Instance.Collection.UnitsDictionary[Types.UnitType.DEFENDER].OriginalUnitStats.Icon;
            ButtonImage[2].sprite = GameController.Instance.Collection.UnitsDictionary[Types.UnitType.RUNNER].OriginalUnitStats.Icon;

            // if has enough money
            ButtonImage[0].color = Color.white;
            ButtonImage[1].color = Color.white;
            ButtonImage[2].color = Color.white;

            SpecializeButton[0].onClick.AddListener(() => { entity.Specialization(Types.UnitType.LIFTER); HFEventManager.TriggerEvent(HFEventID.OnUnitSpecialized, entity, 0); });
            SpecializeButton[1].onClick.AddListener(() => { entity.Specialization(Types.UnitType.DEFENDER); HFEventManager.TriggerEvent(HFEventID.OnUnitSpecialized, entity, 0); });
            SpecializeButton[2].onClick.AddListener(() => { entity.Specialization(Types.UnitType.RUNNER); HFEventManager.TriggerEvent(HFEventID.OnUnitSpecialized, entity, 0); });

            // else
            //ButtonImage[0].color = Color.grey;
            //ButtonImage[1].color = Color.grey;
            //ButtonImage[2].color = Color.grey;
            //UpgradeButton.onClick.AddListener(/*trigger event => can't be upgraded*/);
        }
        else if (Entity is BuildingBehaviour)
        {
            //ButtonImage[0].sprite = GameController.Instance.Collection.UnitsDictionary[Types.UnitType.LIFTER].OriginalUnitStats.Icon;
            //ButtonImage[1].sprite = GameController.Instance.Collection.UnitsDictionary[Types.UnitType.DEFENDER].OriginalUnitStats.Icon;
            //ButtonImage[2].sprite = GameController.Instance.Collection.UnitsDictionary[Types.UnitType.RUNNER].OriginalUnitStats.Icon;

            //// if has enough money
            //ButtonImage[0].color = Color.white;
            //ButtonImage[1].color = Color.white;
            //ButtonImage[2].color = Color.white;

            //SpecializeButton[0].onClick.AddListener(() => { entity.Specialization(Types.UnitType.LIFTER); HFEventManager.TriggerEvent(HFEventID.OnUnitSpecialized, entity, 0); });
            //SpecializeButton[1].onClick.AddListener(() => { entity.Specialization(Types.UnitType.DEFENDER); HFEventManager.TriggerEvent(HFEventID.OnUnitSpecialized, entity, 0); });
            //SpecializeButton[2].onClick.AddListener(() => { entity.Specialization(Types.UnitType.RUNNER); HFEventManager.TriggerEvent(HFEventID.OnUnitSpecialized, entity, 0); });

            // else
            //ButtonImage[0].color = Color.grey;
            //ButtonImage[1].color = Color.grey;
            //ButtonImage[2].color = Color.grey;
            //UpgradeButton.onClick.AddListener(/*trigger event => can't be upgraded*/);
        }
    }
}
