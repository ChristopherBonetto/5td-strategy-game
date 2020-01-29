using System.Collections.Generic;

public class UIManager : Singleton<UIManager>
{
    /// <summary>
    /// Controls Collection.
    /// Every key must provides only one value.
    /// </summary>
    public Dictionary<UIControlID, UIControl> controls = new Dictionary<UIControlID, UIControl>();


    #region Methods
    /// <summary>
    /// Add new UIControl
    /// <see cref="UIControl"/>
    /// </summary>
    public void AddControl(UIControl uiControl)
    {
        if (uiControl != null && !controls.ContainsKey(uiControl.Name))
            controls.Add(uiControl.Name, uiControl);
    }

    /// <summary>
    /// Remove an existing UIControl
    /// <see cref="UIControl"/>
    /// </summary>
    public void RemoveControl(UIControl uiControl)
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
        if (controls.TryGetValue(id, out UIControl control))
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
        if (controls.TryGetValue(id, out UIControl control))
            control.OnHide();
    }

    /// <summary>
    /// Show a UIControl by ID and
    /// Hide a UIControl by class type.
    /// <see cref="UIControlID"/>
    /// <seealso cref="UIControl"/>
    /// </summary>
    public void ShowAndHide(UIControlID id, UIControl controlToHide)
    {
        // If they are the same control... return
        if (id == controlToHide.Name) return;


        if (controls.TryGetValue(id, out UIControl control))
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
    #endregion
}
