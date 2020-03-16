using UnityEngine;

public class HFCamera : MonoBehaviour
{
	private enum Movement
	{
		Free = 0,
		Drag = 1,
		Orbit = 2,
		Pitch = 3
	}

	#region Private variables

	/*** Components */

	[Header("Components")]

	[Tooltip("Camera target transform")]
	[SerializeField]
	Transform m_targetTransform = null;

	[Tooltip("Camera object")]
	[SerializeField]
	Camera m_camera = null;

	/*** Movements */

	[SerializeField]
	private Vector3 m_cameraStartPosition = new Vector3(0f, 0f, 0f);

	[Header("Movements")]
	[SerializeField]
	private bool m_scroll = true;
	[SerializeField]
	private bool m_zoom = true;
	[SerializeField]
	private bool m_orbit = true;
	[SerializeField]
	private bool m_pitch = true;

	private Movement m_currentMovement;

	/*** Scroll */

	[Header("Scroll")]

	[Tooltip("Scroll speed")]
	[SerializeField]
	float m_scrollSpeed = 100f;

	[Range(0f, 1f)]
	[Tooltip("Scroll interpolation speed")]
	[SerializeField]
	float m_scrollLerp = 0.45f;

	[Range(0f, 5f)]
	[Tooltip("Scroll speed boost when zoomed out")]
	[SerializeField]
	float m_scrollBoost = 2.5f;

	[Range(0.5f, 2f)]
	[Tooltip("Sensitivity for mouse scrolling (1 = constant speed)")]
	[SerializeField]
	float m_scrollSensitivity = 2f;

	[Range(0f, 5f)]
	[Tooltip("Edge size for mouse scrolling (0 = disabled)")]
	[SerializeField]
	float m_scrollSize = 0.5f;

	[Tooltip("Map object for size detection")]
	[SerializeField]
	Transform m_mapArea = null;

	// Map pixel boundaries
	Rect m_mapRect;

	// Scroll edge size in pixels
	float m_horEdgeSize;
	float m_vertEdgeSize;

	// Map size in units
	float m_horSize;
	float m_vertSize;

	Vector3 m_targetPosition;

	Vector3 m_dragOffset;
	Vector3 m_dragScreenStart;
	Vector3 m_dragWorldStart;

	/*** Zoom */

	[Header("Zoom")]

	[Tooltip("Zoom speed")]
	[SerializeField]
	float m_zoomSpeed = 100f;

	[Range(0f, 1f)]
	[Tooltip("Zoom interpolation speed")]
	[SerializeField]
	float m_zoomLerp = 0.3f;

	[Tooltip("Wheel sensitivity")]
	[SerializeField]
	float m_wheelSensitivity = 25f;

	[Range(1f, 4f)]
	[Tooltip("Minimum orthographic size")]
	[SerializeField]
	float m_minZoomSize = 2f;

	[Range(4f, 16f)]
	[Tooltip("Maximum orthographic size")]
	[SerializeField]
	float m_maxZoomSize = 8f;

	float m_targetZoom;

	/*** Orbit */

	[Header("Orbit")]

	[Tooltip("Orbit speed")]
	[SerializeField]
	float m_orbitSpeed = 200f;

	[Range(0, 1)]
	[Tooltip("Orbit interpolation speed")]
	[SerializeField]
	float m_orbitLerp = 0.3f;

	[Tooltip("Mouse sensitivity")]
	[SerializeField]
	float m_mouseSensitivity = 5f;

	/*** Pitch */

	[Header("Pitch")]

	[Tooltip("Pitch speed")]
	[SerializeField]
	float m_pitchSpeed = 5f;

	[Range(0f, 1f)]
	[Tooltip("Pitch interpolation speed")]
	[SerializeField]
	float m_pitchLerp = 0.3f;

	[Range(1f, 45f)]
	[Tooltip("Minimum pitch angle")]
	[SerializeField]
	float m_minPitchAngle = 30f;

	[Range(45f, 89f)]
	[Tooltip("Maximum pitch angle")]
	[SerializeField]
	float m_maxPitchAngle = 60f;

	Quaternion m_targetRotation;

	#endregion

	#region Core loop

	void Start()
	{
#if UNITY_EDITOR
		NullChecks();
#endif
		InitEdgeScrolling();
		m_dragOffset = Vector3.zero;
		m_targetPosition = m_targetTransform.position;
		m_targetZoom = m_camera.orthographicSize;
		m_targetRotation = m_targetTransform.rotation;
		m_currentMovement = Movement.Free;

		m_camera.orthographic = true;
		m_camera.transform.localPosition = m_cameraStartPosition;
		m_camera.nearClipPlane = -1000f;

		Cursor.lockState = CursorLockMode.Confined;
	}

	void Update()
	{
		if (m_scroll)
		{
			Scroll();

		}
		if (m_zoom)
		{
			Zoom(); 
		}
		if (m_orbit)
		{
			Orbit(); 
		}
		if (m_pitch)
		{
			Pitch(); 
		}
	}

	#endregion

	#region Movements and input

	/// <summary>
	/// Execute camera scroll (X-Z)
	/// </summary>
	private void Scroll()
	{
		// Update target position
		if (HandleScrollInput() != Vector3.zero)
		{
			// Apply input
			Vector3 target = m_targetTransform.position + HandleScrollInput() * Time.deltaTime;

			// Clamp within map bounds
			target.x = Mathf.Clamp(target.x, -m_horSize / 2f, m_horSize / 2f);
			target.z = Mathf.Clamp(target.z, -m_vertSize / 2f, m_vertSize / 2f);
			m_targetPosition = target;
		}

		m_targetTransform.position = Vector3.Lerp(m_targetTransform.position, m_targetPosition, m_scrollLerp);
	}

	/// <summary>
	/// Check keyboard and mouse input and calculate scroll direction and speed (mouse first)
	/// </summary>
	/// <returns>Scroll direction and length vector</returns>
	private Vector3 HandleScrollInput()
	{
		if (m_currentMovement != Movement.Free && m_currentMovement != Movement.Drag)
		{
			return Vector3.zero;
		}

		// Lock other movements
		if (Input.GetMouseButtonDown(1))
		{
			SetMovement(Movement.Drag);

			m_dragScreenStart = Input.mousePosition;
			m_dragWorldStart = GetTerrainPosition(m_dragScreenStart);
			// Align y
			m_dragWorldStart.y = m_targetTransform.position.y;

			// Save offset, use target position as reference to allow lerp completion
			m_dragOffset = m_dragWorldStart - m_targetTransform.position;
		}
		if (Input.GetMouseButtonUp(1))
		{
			SetMovement(Movement.Free);

			// Reset offset
			m_dragOffset = Vector3.zero;
			return Vector3.zero;
		}

		Vector3 scrollDirection = new Vector3();
		float currentSpeed = m_scrollSpeed;

		// Mouse (directly apply)
		if (Input.GetMouseButton(1))
		{
			Vector3 worldDrag = GetTerrainPosition(Input.mousePosition) - GetTerrainPosition(m_dragScreenStart);
			Vector3 target = m_dragWorldStart - m_dragOffset - worldDrag;

			// Clamp within map bounds
			target.x = Mathf.Clamp(target.x, -m_horSize / 2f, m_horSize / 2f);
			target.z = Mathf.Clamp(target.z, -m_vertSize / 2f, m_vertSize / 2f);

			m_targetPosition = target;
			return Vector3.zero;
		}
		// Keyboard
		else if (m_currentMovement != Movement.Drag && Input.GetAxis("ScrollX") != 0f || Input.GetAxis("ScrollZ") != 0f)
		{
			scrollDirection += Vector3.right * Input.GetAxis("ScrollX") + Vector3.up * Input.GetAxis("ScrollZ");
		}
		// Edge
		else if (m_currentMovement != Movement.Drag)
		{
			float rightEdge = Mathf.Clamp(Input.mousePosition.x, m_mapRect.max.x - m_horEdgeSize, m_mapRect.max.x);
			float leftEdge = Mathf.Clamp(Input.mousePosition.x, m_mapRect.min.x, m_mapRect.min.x + m_horEdgeSize);
			float topEdge = Mathf.Clamp(Input.mousePosition.y, m_mapRect.max.y - m_vertEdgeSize, m_mapRect.max.y);
			float bottomEdge = Mathf.Clamp(Input.mousePosition.y, m_mapRect.min.y, m_mapRect.min.y + m_vertEdgeSize);

			// Right
			if (Input.mousePosition.x == rightEdge)
			{
				scrollDirection += Vector3.right;
				currentSpeed *= m_scrollSensitivity - (m_mapRect.max.x - rightEdge) / (m_horEdgeSize / m_scrollSensitivity);
			}
			// Left
			else if (Input.mousePosition.x == leftEdge)
			{
				scrollDirection -= Vector3.right;
				currentSpeed *= m_scrollSensitivity - (leftEdge - m_mapRect.min.x) / (m_horEdgeSize / m_scrollSensitivity);
			}

			// Up
			if (Input.mousePosition.y == topEdge)
			{
				scrollDirection += Vector3.forward;
				currentSpeed *= m_scrollSensitivity - (m_mapRect.max.y - topEdge) / (m_vertEdgeSize / m_scrollSensitivity);
			}
			// Down
			else if (Input.mousePosition.y == bottomEdge)
			{
				scrollDirection -= Vector3.forward;
				currentSpeed *= m_scrollSensitivity - (bottomEdge - m_mapRect.min.y) / (m_vertEdgeSize / m_scrollSensitivity);
			}
		}

		// Boost speed curve when further
		float boost = (m_camera.orthographicSize - m_minZoomSize) / (m_maxZoomSize - m_minZoomSize) * m_scrollBoost;
		currentSpeed *= 1 + boost;

		// Rotate view and lock y axis
		Vector3 clampedDirection = m_targetTransform.rotation * scrollDirection;
		clampedDirection.y = 0f;

		return clampedDirection.normalized * currentSpeed;
	}

	/// <summary>
	/// Execute zoom
	/// </summary>
	private void Zoom()
	{
		// Update target zoom
		if (HandleZoomInput() != 0f)
		{
			// Damp speed curve when closer, boost when further
			float damp = (m_maxZoomSize - m_camera.orthographicSize) / (m_maxZoomSize - m_minZoomSize) - 0.5f;
			// Apply input
			float targetSize = m_camera.orthographicSize - HandleZoomInput() * Time.deltaTime * (1 - damp);
			// Clamp
			m_targetZoom = Mathf.Clamp(targetSize, m_minZoomSize, m_maxZoomSize); 
		}

		m_camera.orthographicSize = Mathf.Lerp(m_camera.orthographicSize, m_targetZoom, m_zoomLerp);
	}

	/// <summary>
	/// Check keyboard and mouse input and calculate zoom direction and speed (mouse first)
	/// </summary>
	/// <returns>Zoom direction and speed</returns>
	private float HandleZoomInput()
	{
		if (m_currentMovement != Movement.Free)
		{
			return 0f;
		}

		float zoomDelta = 0f;

		// Mouse
		if (Input.GetAxis("WheelZoom") != 0f && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
		{
			zoomDelta += Input.GetAxis("WheelZoom") * m_zoomSpeed * m_wheelSensitivity;
		}
		// Keyboard
		else if (Input.GetAxis("KeyZoom") != 0f)
		{
			zoomDelta += Input.GetAxis("KeyZoom") * m_zoomSpeed;
		}

		return zoomDelta;
	}

	/// <summary>
	/// Execute orbit (Y)
	/// </summary>
	private void Orbit()
	{
		// Update target rotation (Y)
		if (HandleOrbitInput() != 0f)
		{
			// Apply input
			Vector3 rotation = new Vector3(0f, HandleOrbitInput() * Time.deltaTime, 0f);
			m_targetRotation = Quaternion.Euler(m_targetTransform.rotation.eulerAngles + rotation);
		}

		m_targetTransform.rotation = Quaternion.Lerp(m_targetTransform.rotation, m_targetRotation, m_orbitLerp);
	}

	/// <summary>
	/// Check keyboard and mouse input and calculate orbit direction and speed (mouse first)
	/// </summary>
	/// <returns>Orbit direction and speed</returns>
	private float HandleOrbitInput()
	{
		if (m_currentMovement != Movement.Free && m_currentMovement != Movement.Orbit)
		{
			return 0f;
		}

		// Unlock other movements
		if (Input.GetMouseButtonUp(2) || Input.GetKeyUp(KeyCode.O))
		{
			SetMovement(Movement.Free);
		}

		float orbitDelta = 0f;

		// Mouse
		if (Input.GetMouseButton(2) || Input.GetKey(KeyCode.O))
		{
			if (Mathf.Abs(Input.GetAxis("Mouse X")) >= 0.1f)
			{
				// Lock other movements
				SetMovement(Movement.Orbit);
				orbitDelta += Input.GetAxis("Mouse X") * m_orbitSpeed * m_mouseSensitivity; 
			}
		}
		// Keyboard
		else if (Input.GetAxis("Orbit") != 0f && m_currentMovement != Movement.Orbit)
		{
			orbitDelta += Input.GetAxis("Orbit") * m_orbitSpeed;
		}

		return orbitDelta;
	}

	/// <summary>
	/// Execute pitch (X)
	/// </summary>
	private void Pitch()
	{
		// Update target rotation (X)
		if (HandlePitchInput() != 0f)
		{
			// Apply input
			float rotDeltaX = HandlePitchInput();
			// Clamp
			Vector3 rotation = m_targetTransform.rotation.eulerAngles;
			rotation.x = Mathf.Clamp(rotation.x + rotDeltaX, m_minPitchAngle, m_maxPitchAngle);
			m_targetRotation = Quaternion.Euler(rotation);
		}

		m_targetTransform.rotation = Quaternion.Lerp(m_targetTransform.rotation, m_targetRotation, m_pitchLerp);
	}

	/// <summary>
	/// Check keyboard and mouse input and calculate pitch direction and speed (mouse first)
	/// </summary>
	/// <returns>Pitch direction and speed</returns>
	private float HandlePitchInput()
	{
		if (m_currentMovement != Movement.Free && m_currentMovement != Movement.Pitch)
		{
			return 0f;
		}

		// Unlock other movements
		if (Input.GetMouseButtonUp(2) || Input.GetKeyUp(KeyCode.O))
		{
			SetMovement(Movement.Free);
		}

		float pitchDelta = 0f;

		// Mouse
		if (Input.GetMouseButton(2) || Input.GetKey(KeyCode.O))
		{
			if (Mathf.Abs(Input.GetAxis("Mouse Y")) >= 0.1f)
			{
				// Lock other movements
				SetMovement(Movement.Pitch);
				pitchDelta += Input.GetAxis("Mouse Y") * m_pitchSpeed * m_mouseSensitivity; 
			}
		}
		// Keyboard
		else if (Input.GetAxis("Pitch") != 0f && m_currentMovement != Movement.Pitch)
		{
			pitchDelta += Input.GetAxis("Pitch") * m_pitchSpeed;
		}

		return pitchDelta;
	}

	#endregion

	#region Helpers

#if UNITY_EDITOR
	/// <summary>
	/// Inspector reference checks
	/// </summary>
	private void NullChecks()
	{
		if (!m_camera)
		{
			Debug.LogError("No reference to camera.");
		}
		if (!m_targetTransform)
		{
			Debug.LogError("No reference to target transform.");
		}
		if (!m_mapArea)
		{
			Debug.LogError("No reference to a map object.");
		}
	}
#endif

	/// <summary>
	/// Calculate map area border positions
	/// </summary>
	private void InitEdgeScrolling()
	{
		Vector2 mapPosition;
		Vector2 mapSize;

		mapPosition.x = 0f;
		mapPosition.y = 0f;
		mapSize.x = Screen.width;
		mapSize.y = Screen.height;

		m_mapRect = new Rect(mapPosition, mapSize);

		m_vertEdgeSize = mapSize.y * m_scrollSize / 100f;
		// Using all-around constant edge thickness
		m_horEdgeSize = m_vertEdgeSize;

		m_horSize = m_mapArea.localScale.x * 10f;
		m_vertSize = m_mapArea.localScale.y * 10f;
	}

	/// <summary>
	/// Update current movement state
	/// </summary>
	/// <param name="inMovement">New movement state</param>
	private void SetMovement(Movement inMovement)
	{
		if (inMovement == m_currentMovement)
		{
			return;
		}

		m_currentMovement = inMovement;
	}

	/// <summary>
	/// Get terrain coordinates (y = 0) at mouse position
	/// </summary>
	/// <param name="mousePosition">Mouse position in screen coordinates</param>
	/// <returns>Terrain coordinates (y = 0)</returns>
	private Vector3 GetTerrainPosition(Vector3 mousePosition)
	{
		// Intersecting plane y = 0 with a straight line originating at ray.origin (P0) with ray.direction (a,b,c)
		Ray r = m_camera.ScreenPointToRay(mousePosition);
		// t = -y0 / b
		float t = -r.origin.y / r.direction.y;
		return new Vector3(r.direction.x * t + r.origin.x, 0f, r.direction.z * t + r.origin.z);
	}

	#endregion
}
