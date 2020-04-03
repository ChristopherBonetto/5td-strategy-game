using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HFUIManager : Singleton<HFUIManager>
{
    //--------------------------------------------------------
    // Override how it get the instance
    // If the instance is = null, then load it from resources.
    // The first time it will be called it initialize the 
    // UI controls collection.
    //--------------------------------------------------------
    new public static HFUIManager Instance
    {
        get
        {
            if (applicationIsQuitting)
                return null;

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = (HFUIManager)FindObjectOfType(typeof(HFUIManager));

                    if (_instance == null)
                    {
                        GameObject outGO = Instantiate(Resources.Load<GameObject>("Managers/UIManager"));
                        _instance = outGO.GetComponent<HFUIManager>();

                    }

                    DontDestroyOnLoad(_instance);
                    _instance.Initialize();
                }

                return _instance;
            }
        }
    }



    //-------------------------------------------------------
    // UI Controls
    //-------------------------------------------------------

    private Dictionary<UIControlID, HFUIControl> m_UIControls;
	public Dictionary<UIControlID, HFUIControl> UIControls
    {
        get
        {
            if (m_UIControls == null)
            {
                m_UIControls = new Dictionary<UIControlID, HFUIControl>();
            }
            return m_UIControls;
        }
    }

    public HFUIControl LastUIControlShown { get; private set; }


    //--------------------------------------------------------
    // Utils
    //--------------------------------------------------------

	private UnityEngine.UI.GraphicRaycaster m_graphicRaycaster = null;
    public Canvas ScreenCanvas;

    #region MonoBehaviour

    protected void Awake()
	{
    	m_graphicRaycaster = GetComponentInChildren<UnityEngine.UI.GraphicRaycaster>();

        if (Instance != null && Instance != this)
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        // Listen to game manager state changes
        HFEventManager.SubscribeTo<GameStates>(HFEventID.OnGameStateChanged, OnGameStateChange);
    }

    private void OnDisable()
    {
        //Stop to listen to game manager state changes
        HFEventManager.UnsubscribeFrom<GameStates>(HFEventID.OnGameStateChanged, OnGameStateChange);
    }

    private void Start()
    {
        OnGameStateChange(HFGameManager.Instance.CurrentGameState);
    }


    private void Initialize()
    {
        foreach(HFUIControl control in ScreenCanvas.GetComponentsInChildren<HFUIControl>())
        {
            control.gameObject.SetActive(false);
            AddControl(control);
        }
    }

    #endregion

    #region UI Controls Management
    /// <summary>
    /// Add new UIControl
    /// <see cref="UIControl"/>
    /// </summary>
    public void AddControl(HFUIControl uiControl)
    {
        if (uiControl != null && !UIControls.ContainsKey(uiControl.Name))
            UIControls.Add(uiControl.Name, uiControl);
    }

    /// <summary>
    /// Remove an existing UIControl
    /// <see cref="UIControl"/>
    /// </summary>
    public void RemoveControl(HFUIControl uiControl)
    {
        if (uiControl != null && UIControls.ContainsKey(uiControl.Name))
            UIControls.Remove(uiControl.Name);
    }

    /// <summary>
    /// Show a UIControl by ID.
    /// <see cref="UIControlID"/>
    /// </summary>
    public void Show(UIControlID id)
    {
        if (UIControls.TryGetValue(id, out HFUIControl control))
        {
            control.OnShow();
            LastUIControlShown = control;
        }
    }

    /// <summary>
    /// Hide a UIControl by ID.
    /// <see cref="UIControlID"/>
    /// </summary>
    public void Hide(UIControlID id)
    {
        if (UIControls.TryGetValue(id, out HFUIControl control))
            control.OnHide();
    }

    /// <summary>
    /// Show a UIControl by ID and
    /// Hide a UIControl by class type.
    /// <see cref="UIControlID"/>
    /// <seealso cref="HFUIControl"/>
    /// </summary>
    public void ShowAndHide(UIControlID id, HFUIControl controlToHide)
    {
		if (!controlToHide)
		{
			Debug.LogWarning("Received request to hide a null UIControl when showing " + id.ToString());
			return;
		}
		
        // If they are the same control... return
        if (id == controlToHide.Name) return;
            

        if (UIControls.TryGetValue(id, out HFUIControl control))
        {
            if (!control.gameObject.activeSelf)
            {
                /// <see cref="HFUIManager.Show(UIControlID)"/>
                Show(control.Name);
            }

            if (controlToHide.gameObject.activeSelf)
            {
                /// <see cref="HFUIManager.Hide(UIControlID)"/>
                Hide(controlToHide.Name);
            }
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

    private void OnGameStateChange(GameStates inState)
    {
        Debug.Log($"Game state change in: {inState}");

        if (LastUIControlShown != null)
            LastUIControlShown.OnHide();

        // Handle all game state variables.
        switch (inState)
        {
            case GameStates.LoadStartingInfo:
                break;
            case GameStates.StartGame:
                Show(UIControlID.MainMenu);
                break;
            case GameStates.WarRoom:
                Show(UIControlID.LevelSelection);
                break;
            case GameStates.InitializeLevel:
                break;
            case GameStates.PlayingLevel:
                Show(UIControlID.InGameWindow);
                break;
            case GameStates.Pause:
                break;
            case GameStates.EndLevel:
                break;
        }
    }
}
