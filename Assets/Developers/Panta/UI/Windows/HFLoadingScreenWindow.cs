using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HFLoadingScreenWindow : HFUIControl
{
    public override UIControlID Name => UIControlID.LoadingScreen;

    /// <summary>
    /// The reference to the current loading operation running in the background.
    /// </summary>
    public AsyncOperation CurrentLoadingOperation;

    /// <summary>
    /// The minimum time to show this screen.
    /// </summary>
    public const float MinTimeToShow = 1f;

    /// <summary>
    /// Take care bout time elapsed during loading screen
    /// </summary>
    private float m_timeElapsed;

    /// <summary>
    /// A fleg to tell whether a scen is being loaded or not.
    /// </summary>
    private bool m_isLoading;

    /// <summary>
    /// Store the window to turn off before loading screen.
    /// </summary>
    private UIControlID m_windowToHide;

    /// <summary>
    /// Store the window to turn on after loading screen.
    /// </summary>
    private UIControlID m_windowToShow;


    private void Update()
    {
        if (m_isLoading)
        {
            // If the loading is complete, hide the loading screen:
            if (CurrentLoadingOperation.isDone)
            {
                OnHide();
            }
            else
            {
                m_timeElapsed += Time.deltaTime;

                if (m_timeElapsed >= MinTimeToShow)
                {
                    // The loading screen has been showing for the minimum time required.
                    // Allow the loading operation to formally finish:
                    CurrentLoadingOperation.allowSceneActivation = true;
                }
            }
        }
    }

    public void OnShow(AsyncOperation asyncOperation, UIControlID windowToHide, UIControlID windowToShow)
    {
        // Store the windows
        m_windowToHide = windowToHide;
        m_windowToShow = windowToShow;
        HideWindowBeforeLoading();

        gameObject.SetActive(true);

        // reset timer
        m_timeElapsed = 0;

        // Store the reference
        CurrentLoadingOperation = asyncOperation;


        // Stop the loading operation from finishing, even if it technically did:
        CurrentLoadingOperation.allowSceneActivation = false;

        m_isLoading = true;
    }

    public override void OnHide()
    {
        gameObject.SetActive(false);

        CurrentLoadingOperation = null;

        m_isLoading = false;

        ShowWindowAfterLoading();
    }

    private void ShowWindowAfterLoading()
    {
        if (m_windowToShow != UIControlID.None)
            HFUIManager.Instance.Show(m_windowToShow);

        // Reset the value.
        m_windowToShow = UIControlID.None;
    }

    private void HideWindowBeforeLoading()
    {
        if (m_windowToHide != UIControlID.None)
            HFUIManager.Instance.Hide(m_windowToHide);

        // Reset the value.
        m_windowToHide = UIControlID.None;
    }
}
