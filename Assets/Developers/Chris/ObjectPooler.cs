using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Types;

public class ObjectPooler : MonoBehaviour
{
    #region Inner struct

    /// <summary>
    /// Represents an object pooled by the ObjectPooler.
    /// </summary>
    [System.Serializable]
    public struct ObjectPoolItem
    {
        public ObjectPoolItem(GameObject inObj, int inAmount, bool inCanExpland)
        {
            ObjectToPool = inObj;
            AmountToPool = inAmount;
            ShouldExpand = inCanExpland;
        }

        /// <summary>
        /// Object of which copies will be pooled.
        /// </summary>
        [Tooltip("Object of which copies will be pooled.")]
        public GameObject ObjectToPool;

        /// <summary>
        /// How many instances of this object should be pooled.
        /// </summary>
        [Tooltip("How many instances of this object should be pooled.")]
        public int AmountToPool;

        /// <summary>
        /// If true, when there aren't enough available pooled objects of this type, the ObjectPooler will instantiate new ones, expanding the pool.
        /// </summary>
        [Tooltip("If ticked, when there aren't enough available pooled objects of this type, the ObjectPooler will instantiate new ones, expanding the pool.")]
        public bool ShouldExpand;
    }

    #endregion



    #region Static variables

    /// <summary>
    /// Singleton instance.
    /// </summary>
    public static ObjectPooler Instance { get; private set; }

    #endregion


    #region Serialized variables

    /// <summary>
    /// List of items that should be pooled at startup.
    /// </summary>
    [Tooltip("List of items that should be pooled at startup.")]
    [SerializeField] private List<ObjectPoolItem> m_itemsToPool;

    #endregion

    #region Private variables

    /// <summary>
    /// Currently pooled objects, ordered as (tag, List of objects) pairs.
    /// </summary>
    private Dictionary<string, List<GameObject>> m_pooledObjects;
    private Dictionary<UnitType, List<GameObject>> m_unitPooled;
    private Dictionary<BuildingType, List<GameObject>> m_buildingPooled;

    #endregion

    #region MonoBehaviour cycle

    private void Awake()
    {
        // Set singleton instance
        Instance = this;

        // Init Dictionary
        m_pooledObjects = new Dictionary<string, List<GameObject>>();
        m_unitPooled = new Dictionary<UnitType, List<GameObject>>();
        m_buildingPooled = new Dictionary<BuildingType, List<GameObject>>();
    }

    private void Start()
    {
        InitPool();
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Gets a pooled object with the specified tag.
    /// </summary>
    /// <param name="tag">Identifying tag.</param>
    /// <returns>An available pooled object (null if can't be found).</returns>
    public GameObject GetPooledObject(string tag)
    {
        // First check if the Dictionary actually contains the tag key
        if (m_pooledObjects.ContainsKey(tag))
        {
            // Get the list corresponding to the tag
            List<GameObject> objectsList = m_pooledObjects[tag];

            // Cycle the list to search for an available pooled object
            int count = objectsList.Count;

            for (int i = 0; i < count; i++)
            {
                GameObject obj = objectsList[i];

                // If an available object is found, return it
                if (!obj.activeInHierarchy) return obj;
            }


            // If an available object couldn't be found, get the ObjectPoolItem corresponding to the tag (by design, the OPI should always be found)
            foreach (ObjectPoolItem item in m_itemsToPool)
            {
                if (item.ObjectToPool.CompareTag(tag))
                {
                    // If the item allows expanding the pool, do it and return the new object
                    if (item.ShouldExpand)
                        return AddObjectToPool(objectsList, item);

                    // Otherwise, break the loop anyway
                    else break;
                }
            }

            // If previous attempts are unsuccessful, return null (since no object is available nor it can be added)
            return null;
        }
        else
        {
            // If the Dicitionary doesn't contain the tag key, send an error and return null
            Debug.LogError("ObjectPooler '" + name + "' doesn't contain the specified tag '" + tag + "'.");
            return null;
        }
    }

    public GameObject GetUnityObject(UnitType inType)
    {
        // First check if the Dictionary actually contains the tag key
        if (m_unitPooled.ContainsKey(inType))
        {
            // Get the list corresponding to the tag
            List<GameObject> objectsList = m_unitPooled[inType];

            // Cycle the list to search for an available pooled object
            int count = objectsList.Count;

            if(count == 0)
            {
                Debug.LogError("No one object to take with : " + inType + " key");
                return null;
            }

            for (int i = 0; i < count; i++)
            {
                GameObject obj = objectsList[i];

                // If an available object is found, return it
                if (!obj.activeInHierarchy) return obj;
            }

            UnitInfo unit = (UnitInfo)GameController.Instance.SearchUnit(inType);
            ObjectPoolItem tempItem = new ObjectPoolItem(unit.UnitStatsCopy.Prefab, unit.UnitPoolQuantity, true);
            return AddObjectToPool(objectsList, tempItem);

        }
        else
        {
            // If the Dicitionary doesn't contain the tag key, send an error and return null
            Debug.LogError("ObjectPooler '" + name + "' doesn't contain the specified tag '" + tag + "'.");
            return null;
        }
    }

    public GameObject GetBuildingObject(BuildingType inType)
    {
        // First check if the Dictionary actually contains the tag key
        if (m_buildingPooled.ContainsKey(inType))
        {
            // Get the list corresponding to the tag
            List<GameObject> objectsList = m_buildingPooled[inType];

            // Cycle the list to search for an available pooled object
            int count = objectsList.Count;

            if (count == 0)
            {
                Debug.LogError("No one object to take with : " + inType + " key");
                return null;
            }

            for (int i = 0; i < count; i++)
            {
                GameObject obj = objectsList[i];

                // If an available object is found, return it
                if (!obj.activeInHierarchy) return obj;
            }

            BuildingInfo building = (BuildingInfo)GameController.Instance.SearchBuilding(inType);
            ObjectPoolItem tempItem = new ObjectPoolItem(building.BuildingStatsCopy.Prefab, building.BuildingPoolQuantity, true);
            return AddObjectToPool(objectsList, tempItem);

        }
        else
        {
            // If the Dicitionary doesn't contain the tag key, send an error and return null
            Debug.LogError("ObjectPooler '" + name + "' doesn't contain the specified tag '" + tag + "'.");
            return null;
        }
    }

    #endregion

    /// <summary>
    /// Initializes the objects pool.
    /// </summary>
    private void InitPool()
    {
        foreach (ObjectPoolItem item in m_itemsToPool)
        {
            // Create the list corresponding to the item
            List<GameObject> objectsList = new List<GameObject>();

            // Add items to the pool
            for (int i = 0; i < item.AmountToPool; i++)
                AddObjectToPool(objectsList, item);

            // Add the list to the Dictionary, using the item's object's tag as a key
            m_pooledObjects.Add(item.ObjectToPool.tag, objectsList);
        }

        GameCollection collection = GameController.Instance.Collection;

        foreach(UnitType unit in collection.UnitsDictionary.Keys)
        {
            List<GameObject> unitList = new List<GameObject>();

            for(int i = 0; i < collection.UnitsDictionary[unit].UnitPoolQuantity; i++)
            {
                ObjectPoolItem tempItem = new ObjectPoolItem(collection.UnitsDictionary[unit].UnitStatsCopy.Prefab, collection.UnitsDictionary[unit].UnitPoolQuantity, true);
                AddObjectToPool(unitList, tempItem);
            }
            m_unitPooled.Add(unit, unitList);
        }

        foreach(BuildingType building in collection.BuildingsDictionary.Keys)
        {
            List<GameObject> buildingList = new List<GameObject>();

            for(int i = 0; i < collection.BuildingsDictionary[building].BuildingPoolQuantity; i++)
            {
                ObjectPoolItem tempItem = new ObjectPoolItem(collection.BuildingsDictionary[building].BuildingStatsCopy.Prefab, collection.BuildingsDictionary[building].BuildingPoolQuantity, true);
                AddObjectToPool(buildingList, tempItem);
            }
            m_buildingPooled.Add(building, buildingList);
        }
    }

    /// <summary>
    /// Adds an object to the pool.
    /// </summary>
    /// <param name="pooledList">Reference list.</param>
    /// <param name="item">Reference object item.</param>
    /// <returns>Newly instantiated object.</returns>
    private GameObject AddObjectToPool(List<GameObject> pooledList, ObjectPoolItem item)
    {
        GameObject obj = Instantiate(item.ObjectToPool);
        obj.SetActive(false);
        pooledList.Add(obj);

        return obj;
    }


    public List<GameObject> ReturnListFromDictionary(string tag)
    {
        if (m_pooledObjects.ContainsKey(tag))
        {
            return m_pooledObjects[tag];
        }
        else
        {
            return null;
        }
    }
}
