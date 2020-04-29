using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBehaviors : IAttackTypes
{
    private float m_lastAttack;


    public bool CanAttack(float inAttackRate)
    {
        if (Time.time > inAttackRate + m_lastAttack)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AreaAttack(Transform spawnPointDetectionArea, float ViewRadius, float ViewAngle, Collider[] detectedCollider, int numberOfCollisions, LayerMask detectionMask, int damage)
    {
        numberOfCollisions = Physics.OverlapSphereNonAlloc(spawnPointDetectionArea.transform.position, ViewRadius, detectedCollider, detectionMask);

        for (int i = 0; i < numberOfCollisions; i++)
        {
            if (detectedCollider[i])
            {
                float angle = Vector3.Angle(spawnPointDetectionArea.forward, detectedCollider[i].transform.position - spawnPointDetectionArea.position);

                if (Mathf.Abs(angle) < ViewAngle / 2)
                {
                    ITakeDamage dmg = detectedCollider[i].GetComponent<ITakeDamage>();

                    if (dmg != null)
                    {
                        dmg.TakeDamage(damage);
                    }
                }
            }
        }
        m_lastAttack = Time.time;
    }

    

    public bool SingleAttack(GameObject inFocusObj, int inDamage)
    {
        if (!inFocusObj.gameObject.active)
        {
            return false;
        }

        ITakeDamage damageInterface = inFocusObj.GetComponent<ITakeDamage>();

        if(damageInterface != null)
        {
            if (damageInterface.TakeDamage(inDamage))
            {
                return true;
            }
        }
        m_lastAttack = Time.time;
        return false;
    }
}
