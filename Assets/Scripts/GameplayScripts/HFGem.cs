using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Types;

public class HFGem : MonoBehaviour
{
    [SerializeField] private int m_amount;

    private void OnTriggerEnter(Collider other)
    {
        Unit unit = other.gameObject.GetComponent<Unit>();

        if (unit == null) return;

        if (unit.TroopRef.EntityPlayerType == PlayerType.Player)
        {
            GameController.Instance.AddResources(m_amount);
            this.gameObject.SetActive(false);
        }
    }
}
