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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                SelectDeselectOneObject();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                UnitAction();
            }
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
            if(HitInfo.transform.gameObject.layer == CivilizationScriptBehaviour.Instance.GetGameObjectLayer(CivilizationScriptBehaviour.Instance.CivilizationSO.CivilizationLayer))
            {
                ISelectionable Selectionable = HitInfo.transform.GetComponent<ISelectionable>() as ISelectionable;

                if(Selectionable != null)
                {
                    Debug.Log(HitInfo.transform.gameObject);
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
                GiveToEachUnitTipeAFocusObject(Actions.Move, null, HitInfo2.point);
            }

            else if (tempLayer != CivilizationScriptBehaviour.Instance.m_ShiftedCivilizationLayer)
            {
                IDamageable CanBeAttacked = HitInfo2.collider.GetComponent<IDamageable>();

                if (HitInfo2.transform.gameObject.GetComponent<UnitsScriptBehaviour>() != null && CanBeAttacked != null)
                {
                    Debug.Log("ATTACK");
                    GiveToEachUnitTipeAFocusObject(Actions.Attack, HitInfo2.transform.gameObject, HitInfo2.transform.position);
                }
            }

            else if(tempLayer == CivilizationScriptBehaviour.Instance.m_ShiftedCivilizationLayer)
            {
                //ha lo stesso layer. quindi posso interagire;
            }
        }
    }

    private void GiveToEachUnitTipeAFocusObject(Actions ActionType, GameObject FocussedGameobject, Vector3 ObjectPosition)
    {
        UnitsScriptBehaviour tempRef = CurrentSelectedObject.GetComponent<UnitsScriptBehaviour>();

        if(tempRef == null)
        {
            return;
        }

        switch (ActionType)
        {
            case Actions.Attack:
                tempRef.FocusObject = FocussedGameobject;
                tempRef.ChangeUnitState(Actions.Attack);
                break;

            case Actions.Collect:
                
                break;

            case Actions.Move:
                tempRef.FocusObject = null;
                tempRef.SetDestination(ObjectPosition);
                tempRef.ChangeUnitState(Actions.Move);
                break;

            default:
                break;
        }
    }

}


