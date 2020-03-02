using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

public class HFUIManager : Singleton<HFUIManager>
{
    // Override how it get the instance
    // If the instance is = null, then load it from resources.
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

#if UNITY_EDITOR
                        // I have to force the subscribes of the UI Controls
                        Canvas outCanvas = FindObjectOfType<Canvas>();
                        foreach (var uiControl in outCanvas.GetComponentsInChildren<HFUIControl>())
                        {
                            // Add control
                            _instance.AddControl(uiControl);
                            // Hide panel/window
                            uiControl.OnHide();
                            // Notify that is in the editor mode.
                            uiControl.IsInEditorMode = true;
                        }
#endif

                        DontDestroyOnLoad(_instance);
                    }
                    else
                        DontDestroyOnLoad(_instance);
                }

                return _instance;
            }
        }
    }

	private UnityEngine.UI.GraphicRaycaster m_graphicRaycaster = null;

	/// <summary>
	/// Controls Collection.
	/// Every key must provides only one value.
	/// </summary>
	public Dictionary<UIControlID, HFUIControl> UIControls = new Dictionary<UIControlID, HFUIControl>();
    public HFUIControl LastUIControlActivated;

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

#if UNITY_EDITOR
    private void Start()
    {
        // I need this part of code because if we start from a scene that isn't the first one
        // game manager doesn't send notification. So i need scene refereces.

            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                Show(UIControlID.MainMenu);
            }
            else if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                Show(UIControlID.LevelSelection);
            }
            else if (SceneManager.GetActiveScene().buildIndex > 1)
            {
                Show(UIControlID.InGameWindow);
            }
    }
#endif


    #region UI Methods
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
            LastUIControlActivated = control;
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
    /// <seealso cref="UIControl"/>
    /// </summary>
    public void ShowAndHide(UIControlID id, HFUIControl controlToHide)
    {
        // If they are the same control... return
        if (id == controlToHide.Name) return;


        if (UIControls.TryGetValue(id, out HFUIControl control))
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

    public HFUIControl GetUIControl(UIControlID uIControlID)
    {
        if (UIControls.TryGetValue(uIControlID, out HFUIControl value)) return value;
        return value;
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
        if (UIControls == null) return;
        // I init the the variable here because the trigger can happen before start.
        // I store the last window enabled to allow the UI system run also in editor mode,
        // but can help also in build mode.
        if (LastUIControlActivated == null) LastUIControlActivated = UIControls[UIControlID.MainMenu];

        // Handle all game state variables.
        switch (inState)
        {
            case GameStates.StartGame:
                ShowAndHide(UIControlID.MainMenu, LastUIControlActivated);
                break;
            case GameStates.WarRoom:
                ShowAndHide(UIControlID.LevelSelection, LastUIControlActivated);
                break;
            case GameStates.InitializeLevel:
                ShowAndHide(UIControlID.InGameWindow, LastUIControlActivated);
                break;

                // Put other conditions...

            default:
                return;
        }
    }
}
