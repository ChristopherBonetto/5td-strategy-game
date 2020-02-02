using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// It's a reference position game object.
/// Allow us to check if something is spawned in this position.
/// </summary>
public class HFSpawner : MonoBehaviour
{
    private Transform m_Transform;
    public Transform Transform
    {
        get
        {
            if (m_Transform == null)
                m_Transform = GetComponent<Transform>();
            return m_Transform;
        }
    }

    private bool m_IsAlreadyEmployed;   // bool property in case we'll need to call some event.
    /// <summary>
    /// Check if something it's already spawned in this position.
    /// </summary>
    public bool IsAlreadyEmployed
    {
        get { return m_IsAlreadyEmployed; }
        set { m_IsAlreadyEmployed = value; }
    }

    private GameObject m_Troop;         // property in case we'll need to call some event.
    /// <summary>
    /// Assign the troop to spawn at this position.
    /// </summary>
    public GameObject Troop
    {
        get { return m_Troop; }
        set 
        {
            m_Troop = value;

            // Instantiate troop, set position, set rotation.

            // get position of the map to calculate the rotation.
            // I suppose the map is positioned in (0,0,0) coordinates.
            if (m_Troop != null)
                Instantiate(m_Troop, transform.position, Quaternion.LookRotation(Vector3.zero - transform.position, Vector3.up));
        }
    }

    /// <summary>
    /// Reset the spawner's values.
    /// </summary>
    public void ResetSpawner()
    {
        IsAlreadyEmployed = false;
        Troop = null;
    }
}
