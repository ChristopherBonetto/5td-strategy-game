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
        ClickEntity();
    }


    private void ClickEntity()
    {
        RaycastHit HitInfo;
        Ray Ray = Camera.main.ScreenPointToRay(mousePositon);

        if (Physics.Raycast(Ray, out HitInfo, Mathf.Infinity))
        {
            IClickable canBeSelected = HitInfo.transform.GetComponentInParent<IClickable>();

            if(canBeSelected != null)
            {
                canBeSelected.Click();
            }

            // else Trigger the event "OnUnitSelected(null)"
        }
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
            EntityBehavior entity = HitInfo.transform.GetComponentInParent<EntityBehavior>();
            LayerMask tempLayer = HitInfo.transform.gameObject.layer;

            if(entity != null)
            {
                //if(entity is UnitBehavior)
                //{
                //    UnitBehavior unit = entity as UnitBehavior;
                //    entity = unit.TroopRef;
                //}

                var command = new GoToInteract(CurrentEntity, entity);
                Debug.Log(CurrentEntity + " " + entity);
                CurrentEntity.ExecuteCommand(command);
            }
            else
            {
                if (tempLayer == LayerMask.NameToLayer("Terrain"))
                {
                    Debug.Log("Move");
                    CurrentEntity.FocusEntity = null;
                    var command = new MoveWithAgent(CurrentEntity, HitInfo.point);
                    CurrentEntity.ExecuteCommand(command);
                }
            }
        }
    }
}


