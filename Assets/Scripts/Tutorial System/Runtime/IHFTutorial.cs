using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for all components that are part of tutorial
/// </summary>
public interface IHFTutorial
{
    /// <summary>
    /// Tutorial id declared in line.
    /// </summary>
    TutorialID TutorialID { get; set; }

    /// <summary>
    /// Called when the tutorial handler start.
    /// </summary>
    void OnGlobalInitialization();

    /// <summary>
    /// Called when the step of the id start.
    /// </summary>
    void OnStepInitialization();

    /// <summary>
    /// Called when the step of the id end.
    /// Trigger HFEvent, then perform this function.
    /// </summary>
    void OnStepCompleted();
}
