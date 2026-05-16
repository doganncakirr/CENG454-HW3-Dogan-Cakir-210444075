using System.Collections.Generic;
using UnityEngine;

namespace CoreBreach.Systems
{
    public class ObjectPoolManager : MonoBehaviour
    {
        public static ObjectPoolManager Instance { get; private set; }

        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        [Header("Pool Settings")]
        public List<Pool> pools;

        private Dictionary<string, Queue<GameObject>> poolDictionary;
        private Dictionary<GameObject, string> objectTagLookup;
        private HashSet<GameObject> availableObjects;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializePools();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializePools()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
            objectTagLookup = new Dictionary<GameObject, string>();
            availableObjects = new HashSet<GameObject>();

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab, transform);
                    obj.SetActive(false);

                    objectPool.Enqueue(obj);
                    objectTagLookup[obj] = pool.tag;
                    availableObjects.Add(obj);
                }

                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
                return null;
            }

            if (poolDictionary[tag].Count == 0)
            {
                Debug.LogWarning($"Pool with tag {tag} has no available inactive objects.");
                return null;
            }

            GameObject objectToSpawn = poolDictionary[tag].Dequeue();
            availableObjects.Remove(objectToSpawn);

            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            objectToSpawn.SetActive(true);

            return objectToSpawn;
        }

        public void ReturnToPool(GameObject objectToReturn)
        {
            if (objectToReturn == null) return;

            if (!objectTagLookup.TryGetValue(objectToReturn, out string tag))
            {
                Debug.LogWarning($"{objectToReturn.name} is not registered in any object pool.");
                objectToReturn.SetActive(false);
                return;
            }

            ReturnToPool(tag, objectToReturn);
        }

        public void ReturnToPool(string tag, GameObject objectToReturn)
        {
            if (objectToReturn == null) return;

            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
                objectToReturn.SetActive(false);
                return;
            }

            if (availableObjects.Contains(objectToReturn))
            {
                return;
            }

            if (!objectTagLookup.ContainsKey(objectToReturn))
            {
                objectTagLookup[objectToReturn] = tag;
            }

            objectToReturn.SetActive(false);
            objectToReturn.transform.SetParent(transform);

            poolDictionary[tag].Enqueue(objectToReturn);
            availableObjects.Add(objectToReturn);
        }
    }
}