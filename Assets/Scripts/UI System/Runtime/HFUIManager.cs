using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;

namespace HF.Refactoring
{
    public class HFUIManager : MonoBehaviour
    {
        #region Singleton
        private static HFUIManager m_Instance;
        public static HFUIManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    HFUIManager[] managers = FindObjectsOfType<HFUIManager>();

                    // Destroy if there are multiple instance of them.
                    if (managers.Length > 0)
                    {
                        for (int i = 1; i < managers.Length; i++)
                        {
                            Destroy(managers[i].gameObject);
                        }

                        m_Instance = managers[0];
                    }

                    
                    if (m_Instance == null)
                    {
                        m_Instance = Instantiate(Resources.Load("Managers/UIManager", typeof(HFUIManager))) as HFUIManager;
                    }

                    if (m_Instance)
                    {
                        m_Instance.Initialization();
                        DontDestroyOnLoad(m_Instance);
                    }
                }

                return m_Instance;
            }
        }
        #endregion

        public Canvas ScreenCanvas;
        public Canvas WorldCanvas;
        private Dictionary<HFUIWindowID, HFUIWindow> m_WindowCollection;
        public Dictionary<HFUIWindowID, HFUIWindow> WindowCollection => m_WindowCollection;
        private bool m_isInitialized;

        #region Multiple active windows management
        //---------------------------------------------------------------------
        // To manage multiple active windows I decide to create a stack that
        // store the latest window enabled, so now undo action is supported.
        // In order to deny user to interact with background windows I also 
        // create a delegate that trigger every time a new window is shown.
        // All buttons will be subscribed to this delegate and that button 
        // works only if the windowID match the assigne one.
        /// <see cref="HFButton"/> 
        // Note: Loading screen will never be assigned to this stack. It operate
        // async from the other windows.
        //---------------------------------------------------------------------
        public delegate void GetIsListeningInput(HFUIWindowID myWindowId);
        public GetIsListeningInput IsListeningInput;
        private Stack<HFUIWindow> m_windowsHistory;   
        #endregion

        #region Helpers
        private const string m_debugColor = "#7FFFD4";
        #endregion

        private void Awake()
        {
            Initialization();
            ShowAndAddToHistory(HFUIWindowID.MAIN_MENU);
        }

        private void OnEnable()
        {
            HFEventManager.SubscribeTo<GameStates>(HFEventID.OnGameStateChanged, OnGameStateChange);
        }

        private void OnDisable()
        {
            HFEventManager.UnsubscribeFrom<GameStates>(HFEventID.OnGameStateChanged, OnGameStateChange);
        }

        private void Start()
        {
            OnGameStateChange(HFGameManager.Instance.CurrentGameState);
        }

        private void Initialization()
        {
            if (!m_isInitialized)
            {
                m_WindowCollection = new Dictionary<HFUIWindowID, HFUIWindow>();
                m_windowsHistory = new Stack<HFUIWindow>();

                GetWindows();
                HideAllWindowsAtInitialization();

                m_isInitialized = true;
            }
        }

        #region Events
        private void OnGameStateChange(GameStates state)
        {
            Debug.Log($"<color={m_debugColor}><b>[{this.GetType().Name}]</b></color> : {state}");

            switch (state)
            {
                case GameStates.None:
                    break;
                case GameStates.LoadStartingInfo:
                    break;
                case GameStates.StartGame:
                    ShowAndClearHistory(HFUIWindowID.MAIN_MENU);
                    break;
                case GameStates.WarRoom:
                    WorldCanvas.worldCamera = Camera.main;
                    ShowAndClearHistory(HFUIWindowID.WR_LEVEL_SELCTION);
                    break;
                case GameStates.InitializeLevel:
                    break;
                case GameStates.PlayingLevel:
                    ShowAndClearHistory(HFUIWindowID.HUD);
                    break;
                case GameStates.EndLevel:
                    ShowAndClearHistory(HFUIWindowID.LEVEL_ENDING);
                    break;
            }
        }
        #endregion

        #region Utils
        /// <summary>
        /// Get loading screen
        /// </summary>
        public T Getwindow<T>(HFUIWindowID id) where T : HFUIWindow
        {
            return (T)Convert.ChangeType(m_WindowCollection[id], typeof(T));
        }

        /// <summary>
        /// Get all windows under the screen canvas.
        /// Called to initialize the UI.
        /// </summary>
        private void GetWindows()
        {
            if (ScreenCanvas == null || WorldCanvas == null)
            {
                Debug.LogError($"<color={m_debugColor}><b>[{this.GetType().Name}]</b></color> : Screen or World canvas is null, make sure to drag it in inspector");
                return;
            }

            // Get windows from screen canvas
            foreach (HFUIWindow window in ScreenCanvas.GetComponentsInChildren<HFUIWindow>())
            {
                if (!m_WindowCollection.ContainsKey(window.ID))
                {
                    m_WindowCollection.Add(window.ID, window);
                    Debug.Log($"<color={m_debugColor}><b>[{this.GetType().Name}]</b></color> : Window [ID: {window.ID} | object name: {window.gameObject.name}] is added");
                }
            }

            // Get windows from world canvas
            foreach (HFUIWindow window in WorldCanvas.GetComponentsInChildren<HFUIWindow>())
            {
                if (!m_WindowCollection.ContainsKey(window.ID))
                {
                    m_WindowCollection.Add(window.ID, window);
                    Debug.Log($"<color={m_debugColor}><b>[{this.GetType().Name}]</b></color> : Window [ID: {window.ID} | object name: {window.gameObject.name}] is added");
                }
            }
        }

        /// <summary>
        /// Hide all windows in the collection.
        /// </summary>
        private void HideAllWindowsAtInitialization()
        {
            foreach (HFUIWindow window in m_WindowCollection.Values)
            {
                if (window.gameObject.activeSelf)
                {
                    window.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Detect if the mouse is over UI.
        /// </summary>
        public static bool IsPointerOverUIElement()
        {
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }
        #endregion

        #region Window management
        /// <summary>
        /// After clear the history show the window passed in as ID.
        /// Mostly used when load a new scene or event like "ens level".
        /// </summary>
        /// <param name="id"></param>
        public void ShowAndClearHistory(HFUIWindowID id)
        {
            if (!IsHistoryEmpty(0))
            {
                while (m_windowsHistory.Count > 0)
                {
                    m_windowsHistory.Pop().OnHide();
                }
            }

            m_windowsHistory.Clear();

            TryGetWindow(id);
        }

        /// <summary>
        /// Hide the current window,
        /// Show the window passed in as ID and
        /// add it to the history.
        /// </summary>
        /// <param name="id"></param>
        public void ShowAndAddToHistory(HFUIWindowID id, bool addittive = false)
        {
            if (!IsHistoryEmpty(0))
            {
                if (!addittive)
                {
                    m_windowsHistory.Peek().OnHide();
                }
            }

            TryGetWindow(id);
        }

        /// <summary>
        /// Hide the top window, pop it and
        /// show the previous window.
        /// </summary>
        public void Undo()
        {
            if (!IsHistoryEmpty(1))
            {
                m_windowsHistory.Pop().OnHide();
                HFUIWindow window = m_windowsHistory.Peek();
                window.OnShow();
                IsListeningInput?.Invoke(window.ID);
            }
            else
            {
                Debug.LogWarning($"<color={m_debugColor}><b>[{this.GetType().Name}]</b></color> : You can't undo. History is empty");
            }
        }

        private void TryGetWindow(HFUIWindowID id)
        {
            if (m_WindowCollection.TryGetValue(id, out HFUIWindow window))
            {
                window.OnShow();
                m_windowsHistory.Push(window);

                // This make sure that only the window
                // on the top of the satck is listening
                // to input
                IsListeningInput?.Invoke(id);
            }
        }

        private bool IsHistoryEmpty(int count)
        {
            return m_windowsHistory.Count <= count;
        }
        #endregion
    }
}