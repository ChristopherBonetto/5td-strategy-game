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

public class EntityHandler : MonoBehaviour
{
    public PlayerAIBehaviour m_currentPlayerEntity;
    public PlayerAIBehaviour CurrentPlayerEntity
    {
        get
        {
            return m_currentPlayerEntity;
        }
        set
        {
            m_currentPlayerEntity = value;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CurrentPlayerEntity = SelectPlayerEntity();
        }

        if(CurrentPlayerEntity != null)
        {
            if (Input.GetMouseButtonDown(1))
            {
                UnitComandBasedOnClicckedObject();
            }
        }
    }

    private PlayerAIBehaviour SelectPlayerEntity()
    {
        RaycastHit HitInfo;
        Ray Ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(Ray, out HitInfo, Mathf.Infinity))
        {
            if(HitInfo.transform != null)
            {
                PlayerAIBehaviour playerEntity = HitInfo.transform.GetComponent<PlayerAIBehaviour>();

                if(playerEntity is PlayerAIBehaviour)
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
            Entity FocussedEntity = HitInfo.transform.GetComponent<Entity>();
            IDamageable CanBeAttacked = HitInfo.transform.GetComponent<IDamageable>();

            if (FocussedEntity is AIBehaviour && CanBeAttacked != null)
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
                if (CurrentPlayerEntity.EntityAgent != null)
                {
                    CurrentPlayerEntity.FocusObject = FocussedGameobject;
                    CurrentPlayerEntity.ChangeEntityState(Actions.Attack);
                }
                break;

            case Actions.Collect:
                break;

            case Actions.Move:
                if (CurrentPlayerEntity.EntityAgent != null)
                {
                    CurrentPlayerEntity.FocusObject = null;
                    CurrentPlayerEntity.EntityAgent.SetDestination(ObjectPosition);
                    CurrentPlayerEntity.ChangeEntityState(Actions.Move);
                }
                break;

            default:
                break;
        }
    }
}
