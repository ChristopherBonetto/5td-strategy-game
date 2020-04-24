using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HF;
using UnityEngine.UI;


public enum PopUpMode
{
    Specialize,
    Upgrade,
}

public class HFUnitUIPopUpUpgrade : HFPoolableObject
{
    private Camera m_cam;
    private HFUnit m_unit;
    private PopUpMode m_mode = PopUpMode.Upgrade;

    [SerializeField]
    private float m_radius;

    [SerializeField]
    private Button[] m_specializedButtons;

    [SerializeField]
    private Button[] m_upgradeButtons;


    #region MonoBehaviour

    private void OnEnable()
    {
        HFEventManager.SubscribeTo<HFUnit, int>(HFEventID.OnUnitSpecialized, OnUnitSpecialization);
        HFEventManager.SubscribeTo<HFUnit, int>(HFEventID.OnUnitUpgraded, OnUnitupgrade);
        HFEventManager.SubscribeTo<HFUnit>(HFEventID.OnUnitDeath, OnUnitDeath);

        m_cam = Camera.main;
    }

    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom<HFUnit, int>(HFEventID.OnUnitSpecialized, OnUnitSpecialization);
        HFEventManager.UnsubscribeFrom<HFUnit, int>(HFEventID.OnUnitUpgraded, OnUnitupgrade);
        HFEventManager.UnsubscribeFrom<HFUnit>(HFEventID.OnUnitDeath, OnUnitDeath);

        EnableButtons(false, m_specializedButtons);
        EnableButtons(false, m_upgradeButtons);
        this.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (m_unit != null)
        {
            transform.position = RectTransformUtility.WorldToScreenPoint(m_cam, m_unit.transform.position);
        }
    }

    #endregion


    public void ShowSpecializations(HFUnit unit)
    {
        m_unit = unit;

        for (int i = 0; i < unit.Specializations.Length; i++)
        {
            float angle = 360 / m_unit.Specializations.Length * i;
            m_specializedButtons[i].transform.position = DrawIconsInCircle(transform.position, m_radius, angle);

            var index = i;

            // m_specializationButtons[i].Icon = unit.Specializations[i].Icon;
            m_specializedButtons[index].onClick.RemoveAllListeners();
            m_specializedButtons[index].onClick.AddListener(() => unit.Specialize(unit.Specializations[index]));

            EnableButtons(true, m_specializedButtons[i]);
        }
    }

    public void ShowUpgrade(HFUnit unit)
    {
        m_unit = unit;

        CheckCanUpgrade(unit);

        for (int i = 0; i < m_upgradeButtons.Length; i++)
        {
            float angle = 360 / m_upgradeButtons.Length * i;
            m_upgradeButtons[i].transform.position = DrawIconsInCircle(transform.position, m_radius, angle);

            var index = i;

            // m_specializationButtons[i].Icon = unit.Specializations[i].Icon;
            m_upgradeButtons[index].onClick.RemoveAllListeners();
            m_upgradeButtons[index].onClick.AddListener(unit.Upgrade);

            EnableButtons(true, m_upgradeButtons[i]);
        }
    }

    private void CheckCanUpgrade(HFUnit unit)
    {
        for (int i = 0; i < m_upgradeButtons.Length; i++)
        {
            if (!unit.CanUpgrade())
            {
                m_upgradeButtons[i].image.color = Color.grey;
            }
            else
            {
                m_upgradeButtons[i].image.color = Color.white;
            }
        }
    }

    #region Utils

    private Vector3 DrawIconsInCircle(Vector3 center, float radius, float angle)
    {
        float ang = angle;
        Vector3 pos;

        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = 0;

        return pos;
    }

    private void EnableButtons(bool enabled, params Button[] buttons)
    {
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(enabled);
        }
    }

    #endregion

    #region Events

    /// <summary>
    /// Update the pop up with new info.
    /// </summary>
    private void OnUnitupgrade(HFUnit unit, int team)
    {
        if (unit != null && team == 0)
        {
            EnableButtons(false, m_upgradeButtons);
            ShowUpgrade(unit);
        }
    }

    /// <summary>
    /// Turn off the pop up when unit death
    /// </summary>
    private void OnUnitDeath(HFUnit unit)
    {
        if (unit != null && unit.Team == 0 && unit == m_unit)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnUnitSpecialization(HFUnit unit, int team)
    {
        if (unit != null && team == 0)
        {
            EnableButtons(false, m_specializedButtons);
            ShowUpgrade(unit);
        }
    }

    #endregion

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, m_radius);

        Gizmos.color = Color.cyan;
        for (int i = 0; i < m_specializedButtons.Length; i++)
        {
            float angle = 360 / m_specializedButtons.Length * i;
            Gizmos.DrawWireSphere(DrawIconsInCircle(transform.position, m_radius, angle), m_specializedButtons[i].targetGraphic.rectTransform.rect.width - 2.5f);
        }
    }
#endif
}
