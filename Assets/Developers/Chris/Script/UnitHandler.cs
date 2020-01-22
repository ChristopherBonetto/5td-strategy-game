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

public class UnitHandler : MonoBehaviour
{
    private GameObject m_CurrentSelectedObject;
    public GameObject CurrentSelectedObject
    {
        get
        {
            return m_CurrentSelectedObject;
        }
        set
        {
            m_CurrentSelectedObject = value;
        }
    }

    public LayerMask PlayerLayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            CurrentSelectedObject = SelectObject();
        }

        if(CurrentSelectedObject != null)
        {
            if (Input.GetMouseButton(1))
            {
                UnitComandBasedOnClicckedObject();
            }
        }
    }

    private GameObject SelectObject()
    {
        RaycastHit HitInfo;
        Ray Ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(Ray, out HitInfo, Mathf.Infinity, PlayerLayer))
        {
            if(HitInfo.transform != null)
            {
                return HitInfo.transform.gameObject;
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

        if (Physics.Raycast(Ray, out HitInfo))
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

                UnitsAction Unit = CurrentSelectedObject.GetComponent<UnitsAction>();

                if(Unit != null)
                {
                    if(Unit.UnitAgent != null)
                    {
                        Unit.FocusObject = FocussedGameobject;
                        Unit.ChangeUnitState(Actions.Move);
                    }
                }
                
                break;

            case Actions.Collect:
                
                break;

            case Actions.Move:

                UnitsAction Unit2 = CurrentSelectedObject.GetComponent<UnitsAction>();

                if (Unit2 != null)
                {
                    if (Unit2.UnitAgent != null)
                    {
                        Unit2.FocusObject = null;
                        Unit2.UnitAgent.SetDestination(ObjectPosition);
                        Unit2.ChangeUnitState(Actions.Move);
                    }
                }
                break;

            default:
                break;
        }
    }
}
