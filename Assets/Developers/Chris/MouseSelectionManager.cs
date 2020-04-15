using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum Actions
    {
        Idle,
        Attack,
        Collect,
        Lift,
        Move,
    }

public class MouseSelectionManager : MonoBehaviour
{
    public static MouseSelectionManager Instance;

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

    [SerializeField] private LayerMask m_GroundMask;

    private Vector3 mousePositon { get => Input.mousePosition; }
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Select"))
        {
            SelectDeselectOneObject();
        }
        else if (Input.GetButtonDown("Command"))
        {
            UnitAction();
        }
    }

    

    private void SelectDeselectOneObject()
    {
        ClearSelection();
        CurrentSelectedObject = SelectObject();
    }


    private GameObject SelectObject()
    {
        RaycastHit HitInfo;
        Ray Ray = Camera.main.ScreenPointToRay(mousePositon);
        
        if (Physics.Raycast(Ray, out HitInfo, Mathf.Infinity))
        {
            if(HitInfo.transform.gameObject.layer == GameController.Instance.GetGameObjectLayer(GameController.Instance.Collection.PlayerLayer))
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
    

    public void ClearSelection()
    {
        CurrentSelectedObject = null;
    }


    private void UnitAction()
    {
        if(CurrentSelectedObject != null)
        UnitComandBasedOnClicckedObject();
    }

    
    private void UnitComandBasedOnClicckedObject()
    {
        RaycastHit HitInfo2;
        Ray Ray2 = Camera.main.ScreenPointToRay(mousePositon);

        if (Physics.Raycast(Ray2, out HitInfo2))
        {
            LayerMask tempLayer = HitInfo2.transform.gameObject.layer;

            if (tempLayer == LayerMask.NameToLayer("Terrain"))
            {
                Debug.Log("Move");
                GiveCommand(Actions.Move, null, HitInfo2.point);
            }

            else if (tempLayer == GameController.Instance.m_aiLayer)
            {
                IDamageable CanBeAttacked = HitInfo2.collider.GetComponent<IDamageable>();

                if (HitInfo2.transform.gameObject.GetComponent<UnitsScriptBehavior>() != null && CanBeAttacked != null)
                {
                    Debug.Log("ATTACK");
                    GiveCommand(Actions.Attack, HitInfo2.transform.gameObject, HitInfo2.transform.position);
                }
            }

            else if(tempLayer == GameController.Instance.m_playerLayer)
            {
                //ha lo stesso layer. quindi posso interagire;
            }
        }
    }

    private void GiveCommand(Actions ActionType, GameObject FocussedGameobject, Vector3 ObjectPosition)
    {
        UnitsScriptBehavior tempRef = CurrentSelectedObject.GetComponent<UnitsScriptBehavior>();

        if(tempRef == null)
        {
            return;
        }

        switch (ActionType)
        {
            case Actions.Attack:
                tempRef.AssignFocusObj(FocussedGameobject);
                tempRef.ChangeAction(Actions.Attack);
                break;

            case Actions.Collect:
                
                break;

            case Actions.Move:
                tempRef.AssignFocusObj(null);
                tempRef.SetDestination(ObjectPosition);
                tempRef.ChangeAction(Actions.Move);
                break;

            default:
                break;
        }
    }

}


