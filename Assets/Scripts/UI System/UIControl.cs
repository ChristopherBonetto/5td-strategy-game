using UnityEngine;

/// <summary>
/// Abstract structure for all UI panels.
/// </summary>
public abstract class UIControl : MonoBehaviour
{
    /// <summary>
    /// Name.
    /// <see cref="UIControlID"/>
    /// </summary>
    public abstract UIControlID Name { get; }


    protected virtual void Start()
    {
        UIManager.Instance.AddControl(this);

        this.gameObject.SetActive(false);
    }

    protected virtual void OnDestroy()
    {
        UIManager.Instance.RemoveControl(this);
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
