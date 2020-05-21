using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Types;
using HF.Refactoring;


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

    #region Behavior Cycle

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
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

    #endregion

    #region Select deselect entity

    private void SelectDeselectOneObject()
    {
        ClickEntity();
    }

    private void ClickEntity()
    {
        RaycastHit HitInfo;
        Ray Ray = Camera.main.ScreenPointToRay(mousePositon);
        if (Physics.Raycast(Ray, out HitInfo, Mathf.Infinity) && !HFUIManager.IsPointerOverUIElement())
        {
            Debug.Log(HitInfo.transform.name);
            IClickable canBeSelected = HitInfo.transform.GetComponentInParent<IClickable>();

            if(canBeSelected != null)
            {
                if ((EntityBehavior)canBeSelected != CurrentEntity)
                {
                    ClearSelection();
                    HFEventManager.TriggerEvent(HFEventID.OnUnitSelected, null as EntityBehavior, 0);
                }
                else
                {
                    HFEventManager.TriggerEvent(HFEventID.OnUnitSelected, (EntityBehavior)canBeSelected, 0);
                }

                canBeSelected.Click();
            }
            else
            {
                ClearSelection();
                HFEventManager.TriggerEvent(HFEventID.OnUnitSelected, null as EntityBehavior, 0);
            }
        }
    }

    public void ClearSelection()
    {
        CurrentEntity = null;
    }

    #endregion

    #region Command to current entity

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
                var command = new GoToInteract(CurrentEntity, entity);
                CurrentEntity.ExecuteCommand(command);
            }
            else
            {

                if (tempLayer == LayerMask.NameToLayer("Terrain"))
                {
                    TileHighlight tile = HitInfo.transform.GetComponentInChildren<TileHighlight>();
                    if (tile != null)
                    {
                        var command = new MoveWithAgent(CurrentEntity, tile.transform.position);
                        CurrentEntity.ExecuteCommand(command);
                        Debug.Log(CurrentEntity);
                    }
                    
                }
            }
        }
    }

    #endregion
}


