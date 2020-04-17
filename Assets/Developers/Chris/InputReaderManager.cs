using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Types;


public class InputReaderManager : Singleton<InputReaderManager>
{
    new public static InputReaderManager Instance
    {
        get
        {
            if (applicationIsQuitting)
                return null;

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (InputReaderManager)FindObjectOfType(typeof(InputReaderManager));


                    if (_instance == null)
                    {
                        GameObject outGO = Instantiate(Resources.Load<GameObject>("Managers/InputReader"));
                        _instance = outGO.GetComponent<InputReaderManager>();

                        DontDestroyOnLoad(_instance);
                    }
                    else
                        DontDestroyOnLoad(_instance);
                }

                return _instance;
            }
        }
    }

    private EntityBehavior m_currentEntity;
    public EntityBehavior CurrentEntity
    {
        get
        {
            return m_currentEntity;
        }
        set
        {
            m_currentEntity = value;
        }
    }
    

    [SerializeField] private LayerMask m_GroundMask;

    private Vector3 mousePositon { get => Input.mousePosition; }



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
        CurrentEntity = SelectEntity();
    }


    private EntityBehavior SelectEntity()
    {
        RaycastHit HitInfo;
        Ray Ray = Camera.main.ScreenPointToRay(mousePositon);

        if (Physics.Raycast(Ray, out HitInfo, Mathf.Infinity))
        {
            ISelectionable canBeSelected = HitInfo.transform.GetComponentInParent<ISelectionable>();

            if(canBeSelected != null)
            {
                canBeSelected.Clicked();

                if (HitInfo.transform.gameObject.layer == GameController.Instance.m_playerLayer)
                {
                    EntityBehavior entity = HitInfo.transform.GetComponentInParent<EntityBehavior>();
                    if (entity != null)
                    {
                        return entity;
                    }
                }
            }
        }
        return null;
    }


    public void ClearSelection()
    {
        CurrentEntity = null;
    }


    private void UnitAction()
    {
        if (CurrentEntity != null)
            CommandBasedOnClicckedObject();
    }


    private void CommandBasedOnClicckedObject()
    {
        RaycastHit HitInfo;
        Ray Ray = Camera.main.ScreenPointToRay(mousePositon);

        if (Physics.Raycast(Ray, out HitInfo, Mathf.Infinity))
        {
            LayerMask tempLayer = HitInfo.transform.gameObject.layer;

            if (tempLayer == LayerMask.NameToLayer("Terrain"))
            {
                Debug.Log("Move");
                var command = new MoveToAgent(CurrentEntity, HitInfo.point);
                CurrentEntity.ExecuteCommand(command);
            }

            else if (tempLayer == GameController.Instance.m_aiLayer)
            {
                IDamageable CanBeAttacked = HitInfo.collider.GetComponent<IDamageable>();

                if (HitInfo.transform.gameObject.GetComponent<EntityBehavior>() != null && CanBeAttacked != null)
                {
                    Debug.Log("ATTACK");
                    //GiveCommand(Actions.Attack, HitInfo2.transform.gameObject, HitInfo2.transform.position);
                }
            }

            else if (tempLayer == GameController.Instance.m_playerLayer)
            {
                //ha lo stesso layer. quindi posso interagire;
            }
        }
    }
}


