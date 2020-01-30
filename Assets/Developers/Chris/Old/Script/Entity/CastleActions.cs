using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastleActions : MonoBehaviour, IDamageable
{
    private int MaxHp;
    private int CurrentHp;


    private void Start()
    {
        CurrentHp = MaxHp;
    }
    public void Death()
    {
        Destroy(this.gameObject);
    }

    public bool TakeDamage(int Damage)
    {
        Damage = Mathf.Clamp(Damage, 0, CurrentHp);

        if (CurrentHp <= Damage)
        {
            CurrentHp -= Damage;
            Death();
            return true;
        }
        else
        {
            CurrentHp -= Damage;
            return false;
        }
    }

    public void RefreshHp(int inValue)
    {
        MaxHp = inValue;
        CurrentHp = MaxHp;
    }
}
