using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFEnableModel : MonoBehaviour, IHFTutorial
{
    public GameEventData Event;
    public GameObject TutorialModel;
    public GameObject DefaultModel;


    private void OnEnable()
    {
        Event.AddListener(this);
    }

    private void OnDisable()
    {
        Event.RemoveListener(this);
    }


    private TutorialID m_TutorialID = TutorialID.Select_Castle;
    public TutorialID TutorialID { get => m_TutorialID; set => m_TutorialID = value; }

    public void OnGlobalInitialization()
    {
        TutorialModel.SetActive(false);
    }

    public void OnStepCompleted()
    {
        TutorialModel.SetActive(false);
    }

    public void OnStepInitialization()
    {
        TutorialModel.SetActive(true);
    }

    public void Reset()
    {
        TutorialModel.SetActive(false);
    }
}
