using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum SpawnType
{
    PRE_DELAY = 0,
    POST_DELAY = 1,
}

public class HFSpawnerManager : MonoBehaviour // Singleton<HFSpawnerManager>
{
    [Header("Area included")] [Tooltip("Objects will be never spawnned in a lower radius")]
    public float Radius;

    [Header("Spawn wave timer management")]
    public SpawnType CurrentSpawnType;  // This value will be checked from the sender of the event.

    [SerializeField] 
    private float m_MaxTime;
    public float MaxTime
    {
        get { return m_MaxTime; }
        set
        {
            m_MaxTime = value;
            Timer.MaxTime = m_MaxTime;
        }
    }

    private HFTimer m_Timer;
    public HFTimer Timer
    {
        get
        {
            if (m_Timer == null)
                m_Timer = new HFTimer(MaxTime);
            return m_Timer;
        }
    }

    public readonly float MinAngle = -1;
    public readonly float MaxAngle = 1;
    public float Angle { get { return UnityEngine.Random.Range(MinAngle, MaxAngle) * 360; } }


    /// <summary>
    /// Set position and rotation of the entity.
    /// In order to prevent errors, Instantiate or Pool 
    /// must be declared in "entity" slot or before.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="origin"></param>
    public void SpawnEntityInRandomPosition(GameObject entity, Vector3 origin)
    {
        // Null check.
        if (entity == null)
        {
            Debug.LogWarning("entity param can't be null");
            return;
        }

        // Set position
        entity.transform.position = RandomCircleSpawn(origin, Radius);

        // Set rotation
        entity.transform.rotation = Quaternion.LookRotation(origin - entity.transform.position, Vector3.up);
    }

    public Vector3 RandomCircleSpawn(Vector3 center, float radius)
    {
        Vector3 position;

        // Fixed radius offset
        float fixedRadius = radius + (radius / 2);

        // Find area, basic concept
        position.x = center.x + fixedRadius * Mathf.Sin(Angle * Mathf.Deg2Rad);
        position.y = center.y;
        position.z = center.z + fixedRadius * Mathf.Cos(Angle * Mathf.Deg2Rad);

        Debug.Log($"X : {position.x}, Z : {position.z}");

        // Add some semi-randomness
        position.x *= Mathf.Sign(Angle);
        position.z *= Mathf.Sign(Angle);

        return position;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        UnityEditor.Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, Radius);
    }
#endif


    // Those methods are tempporary.
    // I think they will be a concrete methods where generics
    // are existing type. 

    public void CallNextWave(Action callback)
    {
        StartCoroutine(Timer.DecreaseTime(callback));
    }

    public void CallNextWave<T>(Action<T> callback, T arg1) 
    {
        StartCoroutine(Timer.DecreaseTime<T>(callback, arg1));
    }

    public void CallNextWave<T, U>(Action<T, U> callback, T arg1, U arg2)
    {
        StartCoroutine(Timer.DecreaseTime<T, U>(callback, arg1, arg2));
    }
}
