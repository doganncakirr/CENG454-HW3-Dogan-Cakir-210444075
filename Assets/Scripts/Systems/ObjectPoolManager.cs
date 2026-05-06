using System.Collections.Generic;
using UnityEngine;

namespace CoreBreach.Systems
{
    public class ObjectPoolManager : MonoBehaviour
    {
        // Singleton yapısı: Diğer scriptlerin bu havuza kolayca ulaşmasını sağlar.
        public static ObjectPoolManager Instance { get; private set; }

        [System.Serializable]
        public class Pool
        {
            public string tag;           // Havuzun adı (örn: "PlayerBullet", "BasicEnemy")
            public GameObject prefab;    // Üretilecek obje
            public int size;             // Havuzda kaç tane olacağı
        }

        [Header("Pool Settings")]
        public List<Pool> pools;
        private Dictionary<string, Queue<GameObject>> poolDictionary;

        private void Awake()
        {
            // Singleton Kurulumu
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

            foreach (Pool pool in pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.size; i++)
                {
                    GameObject obj = Instantiate(pool.prefab, transform); // Hiyerarşi düzeni için objeleri manager'ın altına alıyoruz
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        // Havuzdan obje çağırma metodu
        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
                return null;
            }

            // Sıradaki objeyi al
            GameObject objectToSpawn = poolDictionary[tag].Dequeue();

            // Objeyi aktif et ve konumlandır
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            // İşi bitince tekrar kullanabilmek için sıranın en arkasına geri ekle
            poolDictionary[tag].Enqueue(objectToSpawn);

            return objectToSpawn;
        }
    }
}