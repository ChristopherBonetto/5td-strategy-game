using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HF;

public class HFUIUnitInfo : MonoBehaviour
{
    [SerializeField]
    private int m_team = 0;

    [SerializeField]
    private Dictionary<Sprite, Image> m_unitsIcons = new Dictionary<Sprite, Image>();

    [SerializeField]
    private HFPoolID m_unitIconID;

    private void OnEnable()
    {
        HFEventManager.SubscribeTo<int, List<HFBaseStats>>(HFEventID.OnUnitsPossessed, SetUnitsIcon);
    }

    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom<int, List<HFBaseStats>>(HFEventID.OnUnitsPossessed, SetUnitsIcon);
    }

    private void SetUnitsIcon(int team, List<HFBaseStats> units)
    {
        if (m_team == team)
        {
            foreach (Image icon in m_unitsIcons.Values)
            {
                icon.gameObject.SetActive(false);
            }

            m_unitsIcons.Clear();

            foreach (var unit in units)
            {
                if (unit.UnitType == HFUnitType.Unit)
                {
                    if (!m_unitsIcons.ContainsKey(unit.Icon))
                    {
                        Image image = HFPoolManager.Instance.GetPooledObject(m_unitIconID.ID).GetComponent<Image>();
                        image.sprite = unit.Icon;
                        image.transform.SetParent(transform);
                        image.transform.localScale = Vector3.one;
                        image.gameObject.SetActive(true);
                        m_unitsIcons.Add(unit.Icon, image);
                    }
                }
            }
        }
    }
}
