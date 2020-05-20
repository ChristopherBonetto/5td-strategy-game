using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCalculator : ITakeDamage
{
    private EntityBehavior entity;

    public DamageCalculator(EntityBehavior inEntity)
    {
        entity = inEntity;
    }

    public void Death()
    {
        if(InputReaderManager.Instance.CurrentEntity == entity)
        {
            InputReaderManager.Instance.CurrentEntity = null;
        }
        entity.gameObject.SetActive(false);
    }

    public bool TakeDamage(int inDamage)
    {
        //inDamage = Mathf.Clamp(inDamage, 0, entity.EntityStats.MaxHp + entity.EntityStats.Armor);

        //if (m_UnitCurrentHp <= inDamage)
        //{
        //    m_UnitCurrentHp -= inDamage;
        //    UIManager.Instance.DeactivateAllPanels();
        //    Death();
        //    return true;
        //}
        //else
        //{
        //    m_UnitCurrentHp -= inDamage;
        //    if (gameObject == MouseSelectionManager.Instance.CurrentSelectedObject)
        //    {
        //        ShowInfoPanels();
        //    }
        //    return false;
        //}
        return true;
    }
}
