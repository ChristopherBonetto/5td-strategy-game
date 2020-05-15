using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        Entity = entity;

        ButtonImage[0].sprite = GameController.Instance.Collection.UnitsDictionary[Types.UnitType.LIFTER].OriginalUnitStats.Icon;
        ButtonImage[1].sprite = GameController.Instance.Collection.UnitsDictionary[Types.UnitType.DEFENDER].OriginalUnitStats.Icon;
        ButtonImage[2].sprite = GameController.Instance.Collection.UnitsDictionary[Types.UnitType.RUNNER].OriginalUnitStats.Icon;

        // if has enough money
        ButtonImage[0].color = Color.white;
        ButtonImage[1].color = Color.white;
        ButtonImage[2].color = Color.white;

        SpecializeButton[0].onClick.AddListener(() => entity.Specialization(Types.UnitType.LIFTER));
        SpecializeButton[1].onClick.AddListener(() => entity.Specialization(Types.UnitType.DEFENDER));
        SpecializeButton[2].onClick.AddListener(() => entity.Specialization(Types.UnitType.RUNNER));

        // else
        //ButtonImage[0].color = Color.grey;
        //ButtonImage[1].color = Color.grey;
        //ButtonImage[2].color = Color.grey;
        //UpgradeButton.onClick.AddListener(/*trigger event => can't be upgraded*/);
    }
}
