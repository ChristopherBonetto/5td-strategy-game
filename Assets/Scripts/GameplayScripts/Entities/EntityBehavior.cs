using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using System.ComponentModel;
using System;
using System.Xml.Schema;

[RequireComponent(typeof(BehaviorTree))]
public class EntityBehavior : MonoBehaviour, ITakeCommand, ITakeDamage, IAttack
{
    #region Variables

    #region Component Var and Interface instances

    protected BehaviorTree m_behaviorTree;

    protected IAttackTypes m_attackType;

    protected HFIEvent3D m_3DSoundInterface;

    #endregion

    #region Stats Var

    protected EntityStatsSO m_entityStats;
    public virtual EntityStatsSO EntityStats
    {
        get { return m_entityStats; }
        set { m_entityStats = value; }
    }

    protected PlayerType m_entityPlayerType;
    public PlayerType EntityPlayerType { get => m_entityPlayerType; }

    protected int m_currentHp;
    public virtual int CurrentHp
    {
        get
        {
            return m_currentHp;
        }
        set
        {
            m_currentHp = value;
        }
    }

    #endregion

    #region Commands Var

    protected List<Command> m_commands;
    protected int m_currentCommandIndex;

    #endregion

    #region Target Var

    protected EntityBehavior m_focusEntity = null;
    public EntityBehavior FocusEntity { get { return m_focusEntity; } set { m_focusEntity = value; } }

    #endregion

    #region Generic Var

    public bool m_isBusy = false;
    public bool IsBusy
    {
        get { return m_isBusy; }
        set
        {
            m_isBusy = value;
        }
    }

    public bool m_isFreezed = false;

    #endregion

    #endregion

    #region Events

    protected virtual void OnEnable()
    {
        HFEventManager.SubscribeTo<bool>(HFEventID.OnPauseMode, FreezeMode);
        HFEventManager.SubscribeTo<GameStates>(HFEventID.OnGameStateChanged, GameStateChanged);
        HFEventManager.SubscribeTo<EntityBehavior>(HFEventID.OnEntityDeath, MyTargetIsDeath);
    }

    protected virtual void OnDisable()
    {
        HFEventManager.UnsubscribeFrom<bool>(HFEventID.OnPauseMode, FreezeMode);
        HFEventManager.UnsubscribeFrom<GameStates>(HFEventID.OnGameStateChanged, GameStateChanged);
        HFEventManager.SubscribeTo<EntityBehavior>(HFEventID.OnEntityDeath, MyTargetIsDeath);
    }

    #endregion

    #region Behaviour Cycle

    public virtual void Awake()
    {
        m_behaviorTree = gameObject.GetComponent<BehaviorTree>();
        StopTree(true);
        m_3DSoundInterface = new HFIAttachPlay3D();

    }
    public virtual void Start()
    {
        m_commands = new List<Command>();

        m_attackType = new AttackBehaviors();

        IsBusy = false;
    }

    #endregion

    #region Stats methods

    public virtual void AssignStats(EntityStatsSO inStats)
    {
        m_entityStats = inStats as EntityStatsSO;
        Debug.Log(m_entityStats);
    }

    public virtual void AssignPlayer(PlayerType inPlayerType)
    {
        m_entityPlayerType = inPlayerType;

        if (EntityPlayerType == PlayerType.Player)
        {
            gameObject.layer = GameController.Instance.m_playerLayer;
        }
        else if (EntityPlayerType == PlayerType.AI)
        {
            gameObject.layer = GameController.Instance.m_aiLayer;
        }
    }

    public EntityStatsSO GetStats()
    {
        return m_entityStats;
    }

    #endregion

    #region Reset and stop entity methods

    public void StopTree(bool inValue)
    {
        m_behaviorTree.enabled = !inValue;
    }

    protected virtual void DisableEntity()
    {
        this.gameObject.SetActive(false);
    }

    public virtual void FreezeMode(bool inValue)
    {
        m_isFreezed = inValue;
        StopTree(inValue);
        m_behaviorTree.ResetValuesOnRestart = !inValue;
    }

    protected virtual void PauseEntity(bool inValue)
    {
        if (m_isFreezed) return;

        StopTree(inValue);
        m_behaviorTree.ResetValuesOnRestart = !inValue;
    }

    #endregion

    #region Command

    public virtual bool AssignFocusEntity(EntityBehavior inEntity)
    {
        m_focusEntity = inEntity;
        Debug.Log(gameObject.name + " want to interact with " + inEntity.name);
        return true;
    }

    //MAYBE INTERFACE.
    public virtual void ExecuteCommand(Command inCommand)
    {
        m_commands.Add(inCommand);
        inCommand.Execute();
        m_currentCommandIndex = m_commands.Count - 1;
    }

    public virtual void Undo()
    {
        if (m_currentCommandIndex < 0)
        {
            return;
        }
        m_commands[m_currentCommandIndex].Undo();
        m_commands.RemoveAt(m_currentCommandIndex);
        m_currentCommandIndex--;
    }

    public virtual void Redo()
    {
        m_commands[m_currentCommandIndex].Execute();
        m_currentCommandIndex++;
    }

    #endregion

    #region Click Interface

    public virtual void Click()
    {
        InputReaderManager.Instance.CurrentEntity = this;
    }
    public virtual void Deselected()
    {
        if (this == null)
        {
            InputReaderManager.Instance.RemoveSelection();
            return;
        } 

        if(this == InputReaderManager.Instance.CurrentEntity)
        {
            InputReaderManager.Instance.RemoveSelection();
        }
    }

    #endregion

    #region Hp Methods

    public virtual void RefreshHp()
    {
        CurrentHp = m_entityStats.MaxHp;
    }

    public virtual bool TakeDamage(int Damage)
    {
        if(m_entityStats.CanTakeDamage)
        {
            Damage = Mathf.Clamp(Damage, 1, m_entityStats.MaxHp + m_entityStats.Armor);

            if (CurrentHp <= Damage)
            {
                Debug.Log("Death");
                Death();
                return true;
            }
            else
            {
                CurrentHp -= Damage;
                Debug.Log(CurrentHp);
                if (InputReaderManager.Instance.CurrentEntity == this)
                {
                    InputReaderManager.Instance.CurrentEntity = null;
                }
                return false;
            }
        }
        return false;
    }

    public virtual void Death()
    {
        m_focusEntity = null;

        if(EntityPlayerType == PlayerType.Player)
        {
            GameController.Instance.RemoveFromDictionary(this);
        }

        Deselected();

        this.gameObject.SetActive(false);

        //HFEventManager.TriggerEvent(HFEventID.OnEntityDeath, this);
    }

    #endregion

    #region Attack

    public virtual void Attack()
    {
    }

    protected virtual void MyTargetIsDeath(EntityBehavior inEntity)
    {
        if(inEntity == FocusEntity)
        {
            Debug.Log("IL MIO TARGET E MORTO");
        }
    }

    #endregion

    #region Specialization
    public virtual void Specialization(UnitType type)
    {
        //Deselected();
        HFEventManager.TriggerEvent(HFEventID.OnUnitSpecialized, this, 0);
        HFEventManager.TriggerEvent(HFEventID.OnTutorialQuestCompleted, TutorialID.Upgrade_Unit);
        HFEventManager.TriggerEvent(HFEventID.OnTutorialQuestCompleted, TutorialID.Specialize_Unit);
    }

    public virtual void Specialization(BuildingType type)
    {
        //Deselected();
        HFEventManager.TriggerEvent(HFEventID.OnUnitSpecialized, this, 0);
        HFEventManager.TriggerEvent(HFEventID.OnTutorialQuestCompleted, TutorialID.Upgrade_Unit);
        HFEventManager.TriggerEvent(HFEventID.OnTutorialQuestCompleted, TutorialID.Specialize_Turret);
    }
    #endregion

    #region Sound methods

    public void AttachAndPlaySound(string eventPath)
    {
        HFCustomEvent tempEvent = HFSoundManager.Instance.GetFreeEventFromDictionaryKey(eventPath);

        if(tempEvent == null)
        {
            Debug.LogError("Can't play sound saved from path : " + eventPath);
            return;
        }
        m_3DSoundInterface.AttachAndPlay(tempEvent, this.gameObject);
    }

    #endregion

    #region GameState event
    protected virtual void GameStateChanged(GameStates inState)
    {
        if ((inState == GameStates.EndLevel))
        {
            StopTree(true);
        }
        else if(inState == GameStates.WarRoom)
        {
            Deselected();
            DisableEntity();
        }
        else if (inState == GameStates.Pause)
        {
            PauseEntity(true);
        }
        else if (inState == GameStates.PlayingLevel)
        {
            PauseEntity(false);
        }
    }

    #endregion
    }
