using UnityEngine;

/// <summary>
/// Abstract structure for all UI panels.
/// </summary>
public abstract class HFUIControl : MonoBehaviour
{
    /// <summary>
    /// The control name or ID
    /// </summary>
    public abstract UIControlID Name { get; }


    protected virtual void OnDestroy()
    {
        if (HFUIManager.Instance != null)
            HFUIManager.Instance.RemoveControl(this);
    }


    public virtual void OnShow()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void OnHide()
    {
        this.gameObject.SetActive(false);
    }
}
