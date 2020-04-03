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
        HFEventManager.SubscribeTo<HFUnit, int>(HFEventID.OnUnitUpgraded, OnUnitupgrade);
    }

    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom<HFUnit, int>(HFEventID.OnUnitUpgraded, OnUnitupgrade);
    }

    private void Start()
    {
        m_cam = Camera.main;

        // Set the specialize buttons positions.
        // Set the function to perform
        for (int i = 0; i < m_specializedButtons.Length; i++)
        {
            float angle = 360 / m_specializedButtons.Length * i;
            m_specializedButtons[i].transform.position = DrawIconsInCircle(transform.position, m_radius, angle);
            //m_specializedButtons[i].onClick.AddListener(/*Add funtion*/);
        }

        // Set the upgrades buttons positions.
        // Set the function to perform
        for (int i = 0; i < m_upgradeButtons.Length; i++)
        {
            float angle = 360 / m_upgradeButtons.Length * i;
            m_upgradeButtons[i].transform.position = DrawIconsInCircle(transform.position, m_radius, angle);
            m_upgradeButtons[i].onClick.AddListener(OnClickupgrade);
        }

        // Turn off all buttons.
        EnableButtons(false, m_specializedButtons);
        EnableButtons(false, m_upgradeButtons);
    }

    private void OnDestroy()
    {
        foreach (var button in m_specializedButtons)
        {
            button.onClick.RemoveAllListeners();
        }

        foreach (var button in m_upgradeButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    private void Update()
    {
        if (m_unit != null)
        {
            transform.position = RectTransformUtility.WorldToScreenPoint(m_cam, m_unit.transform.position);
        }
    }

    #endregion

    #region Set Up Methods (when it's called from the event trigger)

    /// <summary>
    /// It's called from <see cref="HFUnitView"/> 
    /// when the event <see cref="HFEventID.OnUnitSelected"/>
    /// it's triggered.
    /// </summary>
    public void SetUnitInfo(HFUnit unit)
    {
        // Get the unit reference.
        SetUnit(unit);

        switch (m_mode)
        {
            case PopUpMode.Specialize:
                EnableButtons(true, m_specializedButtons);
                EnableButtons(false, m_upgradeButtons);
                // If it's not specialized,
                // then show the specialization buttons.
                break;

            case PopUpMode.Upgrade:
                EnableButtons(true, m_upgradeButtons);
                EnableButtons(false, m_specializedButtons);
                // If it's specialized and can be upgraded,
                // then show the upgrade button.
                ShowUpgrade(unit);
                break;
        }

        gameObject.SetActive(true);
    }

    private void SetUnit(HFUnit unit)
    {
        m_unit = unit;
    }

    private void ShowUpgrade(HFUnit unit)
    {
        // ------------------------------------------
        // If the unit selected can be upgraded then
        // show some feedback.
        // ------------------------------------------

        if (unit.CanUpgrade())
        {
            foreach (var button in m_upgradeButtons)
            {
                button.image.color = Color.white;
            }
        }

        // ------------------------------------------
        // If the unit selected can't be upgraded then
        // show some feedback.
        // ------------------------------------------

        else
        {
            foreach (var button in m_upgradeButtons)
            {
                button.image.color = Color.grey;
            }
        }
    }

    #endregion

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

    private void EnableButtons(bool enable, params Button[] buttons)
    {
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(enable);
        }
    }

    private void OnClickupgrade()
    {
        if (m_unit.CanUpgrade())
        {
            m_unit.Upgrade();
        }
    }

    #endregion

    #region Events

    private void OnUnitupgrade(HFUnit unit, int team)
    {
        if (unit != null && team == 0)
        {
            Debug.Log("Reset unit info...");

            SetUnitInfo(unit);
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
