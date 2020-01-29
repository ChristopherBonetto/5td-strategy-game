using UnityEngine;

/// <summary>
/// Abstract structure for all UI panels.
/// </summary>
public abstract class HFUIControl : MonoBehaviour
{
    /// <summary>
    /// Name.
    /// <see cref="UIControlID"/>
    /// </summary>
    public abstract UIControlID Name { get; }


    protected virtual void Start()
    {
        HFUIManager.Instance.AddControl(this);

        this.gameObject.SetActive(false);
    }

    protected virtual void OnDestroy()
    {
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
