using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Types;
using HF.Refactoring;
using System;

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

    private int? m_currentPressedNumber;

    #region Behavior Cycle

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        SelectDeselectOneObject();
        
        if (Input.GetButtonUp("Command"))
        {
            TroopAction();
        }
    }

    #endregion

    #region Select deselect entity

    private void SelectDeselectOneObject()
    {
        if (HFGameManager.Instance.CurrentGameState != GameStates.PlayingLevel) return;

        if (Input.GetButtonUp("Select"))
        {
            ClickEntity();
        }
        else if (Input.GetButtonUp("SwitchTroop"))
        {
            GameController.Instance.TakeEntityFromDictionary(typeof(Troop));
        }
        else
        {
            m_currentPressedNumber = ReturnKeyboardNumber();

            if (m_currentPressedNumber == null) return;

            GameController.Instance.TakeEntityFromDictionary(typeof(BuildingBehaviour), m_currentPressedNumber.Value);
        }
    }

    private int? ReturnKeyboardNumber()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            return 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            return 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            return 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            return 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            return 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            return 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            return 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            return 7;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            return 8;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            return 9;
        }
        else
        {
            return null;
        }
    }

    private void ClickEntity()
    {
        RaycastHit HitInfo;
        Ray Ray = Camera.main.ScreenPointToRay(mousePositon);
        if (Physics.Raycast(Ray, out HitInfo, Mathf.Infinity,(1<<8)+(1<<9)) && !HFUIManager.IsPointerOverUIElement())
        {
            IClickable canBeSelected = HitInfo.transform.GetComponentInParent<IClickable>();

            if(canBeSelected != null)
            {
                SelectEntity(canBeSelected);
            }
            else
            {
                ClearSelection();
                CurrentEntity?.Deselected();

                HFEventManager.TriggerEvent(HFEventID.OnEntitySelected, null as EntityBehavior, 0);
            }
        }
        else
        {
           if(HFUIManager.IsPointerOverUIElement()==false)
            {
                CurrentEntity?.Deselected();
                ClearSelection();
                
                HFEventManager.TriggerEvent(HFEventID.OnEntitySelected, null as EntityBehavior, 0);
            }
        }

    }

    public void SelectEntity(IClickable inClickable)
    {
        if (inClickable is EntityBehavior)
        {
            if ((EntityBehavior)inClickable != CurrentEntity)
            {
                CurrentEntity?.Deselected();
                ClearSelection();
                HFEventManager.TriggerEvent(HFEventID.OnEntitySelected, (EntityBehavior)inClickable, 0);
                inClickable?.Click();
            }
        }
        else
        {
            inClickable.Click();
        }
    }

    public void SelectEntity(EntityBehavior inEntity)
    {
        if (inEntity is EntityBehavior)
        {
            if ((EntityBehavior)inEntity != CurrentEntity)
            {
                CurrentEntity?.Deselected();
                ClearSelection();
                HFEventManager.TriggerEvent(HFEventID.OnEntitySelected, (EntityBehavior)inEntity, 0);
                inEntity?.Click();
            }
        }
        else
        {
            inEntity.Click();
        }
    }

    public void ClearSelection()
    {
        CurrentEntity = null;
    }

    public void RemoveSelection()
    {
        ClearSelection();
        HFEventManager.TriggerEvent(HFEventID.OnEntitySelected, null as EntityBehavior, 0);
    }

    #endregion

    #region Command to current entity

    private void TroopAction()
    {
        if (HFGameManager.Instance.CurrentGameState == GameStates.Pause) return;

        if (CurrentEntity != null && CurrentEntity.EntityPlayerType == PlayerType.Player)
            CommandBasedOnClicckedObject();
    }

    private void CommandBasedOnClicckedObject()
    {
        RaycastHit HitInfo;
        Ray Ray = Camera.main.ScreenPointToRay(mousePositon);

        if (Physics.Raycast(Ray, out HitInfo, Mathf.Infinity,(1<<8)+(1<<9)+(1<<10)) && !HFUIManager.IsPointerOverUIElement())
        {
            EntityBehavior entity = HitInfo.transform.GetComponentInParent<EntityBehavior>();
            LayerMask tempLayer = HitInfo.transform.gameObject.layer;
            
            if(entity != null && CurrentEntity != entity)
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
                        HFEventManager.TriggerEvent(HFEventID.OnTutorialQuestCompleted, TutorialID.Move_Unit);
                    }
                    
                }
            }
        }
    }

    #endregion
}


