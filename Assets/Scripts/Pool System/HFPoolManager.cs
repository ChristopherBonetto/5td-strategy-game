using System.Collections.Generic;
using UnityEngine;

public class HFPoolManager : MonoBehaviour
{
	#region Singleton
	private static HFPoolManager m_Instance;
	public static HFPoolManager Instance
	{
		get
		{
			if (m_Instance == null)
			{
				HFPoolManager[] managers = FindObjectsOfType<HFPoolManager>();
				Debug.Log(managers.Length);

				// Destroy if there are multiple instance of them.
				if (managers.Length > 1)
				{
					for (int i = 1; i < managers.Length; i++)
					{
						Destroy(managers[i].gameObject);
					}
					
				}

				m_Instance = managers[0];

				if (m_Instance == null)
				{
					m_Instance = Resources.Load("Managers/PoolManager", typeof(HFPoolManager)) as HFPoolManager;
				}

				if (m_Instance)
				{
					m_Instance.StartPooling();
					DontDestroyOnLoad(m_Instance);
				}
			}

			return m_Instance;
		}
	}
	#endregion

	#region Object Pool item inner class
	[System.Serializable]
	private class ObjectPoolItem
	{
		[SerializeField]
		public HFPoolableObject ObjectPrefab = null;

		[SerializeField]
		public int BasePoolSize = 10;

		[SerializeField]
		public bool CanExpand = true;

		[SerializeField]
		public int PoolExpandSize = 1;

		[SerializeField]
		public int MaxPoolSize = 15;

		[SerializeField]
		public HFPoolID uniqueID = null;

		public bool SpawnUnderCanvas = false;
		public HF.Refactoring.HFUIWindowID WindowID;

		[HideInInspector]
		public int CurrentCount = 0;
	}
    #endregion

	//-------------------------------------------------------------------------
	// Object declarated to pool
	//-------------------------------------------------------------------------
    [SerializeField]
	private List<ObjectPoolItem> m_poolItems = new List<ObjectPoolItem>();


	//-------------------------------------------------------------------------
	// Object created and put in the pool
	//-------------------------------------------------------------------------
	private List<HFPoolableObject> m_objectPool = new List<HFPoolableObject>();

	#region Helpers
	private const string m_debugColor = "#FF4500";
	#endregion

	public void StartPooling()
	{
		foreach (ObjectPoolItem item in m_poolItems)
		{
			for (int i = 0; i < item.BasePoolSize; i++)
			{
				CreateNewObject(item, item.SpawnUnderCanvas);
			}
		}
	}

	public bool AddPoolItem(HFPoolableObject newPoolObject, int basePoolsize, bool bCanExpand = true)
	{
		if (!newPoolObject || ContainsPoolItem(newPoolObject.uniqueID.ID))
		{
			return false;
		}

		ObjectPoolItem newItem = new ObjectPoolItem();
		newItem.ObjectPrefab = newPoolObject;
		newItem.uniqueID = newPoolObject.uniqueID;
		newItem.BasePoolSize = basePoolsize;
		newItem.CanExpand = bCanExpand;
		m_poolItems.Add(newItem);

		// Pool has been previously initialized, add new item
		if (m_objectPool.Count > 0)
		{
			CreateNewObject(newItem);
		}

		return true;
	}

	private bool ContainsPoolItem(int poolID)
	{
		foreach (ObjectPoolItem item in m_poolItems)
		{
			if (item.uniqueID.ID == poolID)
			{
				return true;
			}
		}
		return false;
	}

	/* Pooled objects might have an interface to Reset when they aren't needed any more */

	public GameObject GetPooledObject(int poolID)
	{
		for (int i = 0; i < m_objectPool.Count; i++)
		{
			if (m_objectPool[i].uniqueID.ID == poolID)
			{
				GameObject go = m_objectPool[i].gameObject;

				if (!go.activeInHierarchy)
				{
					return go;
				}
			}
		}

		for (int i = 0; i < m_poolItems.Count; i++)
		{
			// I may decide not to expand pool for some categories, e.g. sound or unreliable fx
			if (m_poolItems[i].uniqueID.ID == poolID && m_poolItems[i].CanExpand)
			{
				HFPoolableObject obj = null;
				// Warn to review design
				for (int j = 0; j < m_poolItems[i].PoolExpandSize && m_poolItems[i].CurrentCount < m_poolItems[i].MaxPoolSize; j++)
				{
					obj = CreateNewObject(m_poolItems[i]);
				}
				return obj ? obj.gameObject : null;
			}
		}
		return null;
	}

	public int GetPoolSize(int poolID)
	{
		for (int i = 0; i < m_poolItems.Count; i++)
		{
			if (m_poolItems[i].uniqueID.ID == poolID)
			{
				return m_poolItems[i].CurrentCount;
			}
		}
		return 0;
	}

	private HFPoolableObject CreateNewObject(ObjectPoolItem item, bool spawnInUI = false)
	{
		HFPoolableObject prefab = item.ObjectPrefab;
		if (prefab)
		{
			HFPoolableObject obj = Instantiate(prefab);

			if (spawnInUI)
			{
				// Spawn under the window declareted in the object
				obj.transform.SetParent(HF.Refactoring.HFUIManager.Instance.WindowCollection[item.WindowID].transform);
			}
			else
			{
				// spawn under the Pool Manager.
				obj.transform.SetParent(gameObject.transform);
			}

			obj.gameObject.SetActive(false);
			item.CurrentCount++;
			m_objectPool.Add(obj);

			Debug.Log($"<color={m_debugColor}><b>[{this.GetType().Name}]</b></color> : gameobject [Name = {item.ObjectPrefab.name} | ID = {item.uniqueID}]");

			return obj;
		}
		else
		{
			return null;
		}
	}
}
