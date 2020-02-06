using System.Collections.Generic;
using UnityEngine;

public class HFUIManager : Singleton<HFUIManager>
{
	/* Components */

	private UnityEngine.UI.GraphicRaycaster m_graphicRaycaster = null;

	/// <summary>
	/// Controls Collection.
	/// Every key must provides only one value.
	/// </summary>
	public Dictionary<UIControlID, HFUIControl> controls = new Dictionary<UIControlID, HFUIControl>();


    #region Methods

	void Awake()
	{
		m_graphicRaycaster = GetComponent<UnityEngine.UI.GraphicRaycaster>();
	}

    /// <summary>
    /// Add new UIControl
    /// <see cref="UIControl"/>
    /// </summary>
    public void AddControl(HFUIControl uiControl)
    {
        if (uiControl != null && !controls.ContainsKey(uiControl.Name))
            controls.Add(uiControl.Name, uiControl);
    }

    /// <summary>
    /// Remove an existing UIControl
    /// <see cref="UIControl"/>
    /// </summary>
    public void RemoveControl(HFUIControl uiControl)
    {
        if (uiControl != null && controls.ContainsKey(uiControl.Name))
            controls.Remove(uiControl.Name);
    }

    /// <summary>
    /// Show a UIControl by ID.
    /// <see cref="UIControlID"/>
    /// </summary>
    public void Show(UIControlID id)
    {
        if (controls.TryGetValue(id, out HFUIControl control))
        {
            control.OnShow();
        }
    }

    /// <summary>
    /// Hide a UIControl by ID.
    /// <see cref="UIControlID"/>
    /// </summary>
    public void Hide(UIControlID id)
    {
        if (controls.TryGetValue(id, out HFUIControl control))
            control.OnHide();
    }

    /// <summary>
    /// Show a UIControl by ID and
    /// Hide a UIControl by class type.
    /// <see cref="UIControlID"/>
    /// <seealso cref="UIControl"/>
    /// </summary>
    public void ShowAndHide(UIControlID id, HFUIControl controlToHide)
    {
        // If they are the same control... return
        if (id == controlToHide.Name) return;


        if (controls.TryGetValue(id, out HFUIControl control))
        {
            // Show the control searched by name.
            if (!control.gameObject.activeSelf)
                control.OnShow();

            // Hide the control passed by class type.
            if (controlToHide.gameObject.activeSelf)
                controlToHide.OnHide();
        }
        else
            throw new System.Exception("Control with " + id + " id, doesn't exist or it's not register.");
    }

	/// <summary>
	/// Check if mouse is over a raycast target
	/// </summary>
	/// <returns>True if over a raycast target</returns>
	public bool IsMouseOverUI()
	{
		if (m_graphicRaycaster)
		{
			UnityEngine.EventSystems.PointerEventData ped = new UnityEngine.EventSystems.PointerEventData(null);
			ped.position = Input.mousePosition;
			List<UnityEngine.EventSystems.RaycastResult> results = new List<UnityEngine.EventSystems.RaycastResult>();
			m_graphicRaycaster.Raycast(ped, results);

			return results.Count > 0;
		}

		return false;
	}

	#endregion
}
