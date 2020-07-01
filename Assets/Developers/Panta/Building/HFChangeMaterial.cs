using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HFChangeMaterial : MonoBehaviour, IHFTutorial
{
    public GameEventData Event;
    public Material NewMaterial;
    public Material DefaultMaterial;

    public bool UseSkinnedMesh = false;
    public MeshRenderer Mesh;
    public SkinnedMeshRenderer SkinnedMesh;

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
    }

    public void OnStepCompleted()
    {

        if (UseSkinnedMesh)
            SkinnedMesh.material = DefaultMaterial;
        else
            Mesh.material = DefaultMaterial;
    }

    public void OnStepInitialization()
    {
        if (UseSkinnedMesh)
            SkinnedMesh.material = NewMaterial;
        else
            Mesh.material = NewMaterial;
    }

    public void Reset()
    {
        if (UseSkinnedMesh)
            SkinnedMesh.material = DefaultMaterial;
        else
            Mesh.material = DefaultMaterial;
    }
}
