using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Actions
{
    Idle,
    Attack,
    Collect,
    Move
}

public class PlayerMover : MonoBehaviour
{
    public UnitActions m_currentPlayerUnit;
    public UnitActions CurrentPlayerUnit
    {
        get
        {
            return m_currentPlayerUnit;
        }
        set
        {
            m_currentPlayerUnit = value;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CurrentPlayerUnit = SelectPlayerEntity();
        }

        if(CurrentPlayerUnit != null)
        {
            if (Input.GetMouseButtonDown(1))
            {
                UnitComandBasedOnClicckedObject();
            }
        }
    }

    private UnitActions SelectPlayerEntity()
    {
        RaycastHit HitInfo;
        Ray Ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(Ray, out HitInfo, Mathf.Infinity))
        {
            if(HitInfo.transform != null)
            {
                UnitActions playerEntity = HitInfo.transform.GetComponent<UnitActions>();

                if(playerEntity != null && HitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    return playerEntity;
                }
                else
                {
                    return null;
                }
                
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }

    private void UnitComandBasedOnClicckedObject()
    {
        RaycastHit HitInfo;
        Ray Ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(Ray, out HitInfo, Mathf.Infinity))
        {
            IDamageable CanBeAttacked = HitInfo.transform.GetComponent<IDamageable>();

            if (HitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy") && CanBeAttacked != null)
            {
                Debug.Log("Attack");
                GiveToEachUnitTipeAFocusObject(Actions.Attack, HitInfo.transform.gameObject, HitInfo.transform.position);
            }
            else
            {
                Debug.Log("Move");
                GiveToEachUnitTipeAFocusObject(Actions.Move, null, HitInfo.point);
            }
        }

    }

    private void GiveToEachUnitTipeAFocusObject(Actions ActionType, GameObject FocussedGameobject, Vector3 ObjectPosition)
    {
        switch (ActionType)
        {
            case Actions.Attack:
                if (CurrentPlayerUnit.UnitAgent != null)
                {
                    CurrentPlayerUnit.FocusObject = FocussedGameobject;
                    CurrentPlayerUnit.ChangeUnitState(Actions.Attack);
                }
                break;

            case Actions.Collect:
                break;

            case Actions.Move:
                if (CurrentPlayerUnit.UnitAgent != null)
                {
                    CurrentPlayerUnit.FocusObject = null;
                    CurrentPlayerUnit.UnitAgent.SetDestination(ObjectPosition);
                    CurrentPlayerUnit.ChangeUnitState(Actions.Move);
                }
                break;

            default:
                break;
        }
    }
}
