using System.Collections;
using UnityEngine;
using CoreBreach.Systems; // Havuz sistemimize erişmek için

namespace CoreBreach.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawner Settings")]
        [SerializeField] private string enemyTag = "BasicEnemy"; // Havuzdaki Tag ile birebir aynı olmalı
        [SerializeField] private Transform[] spawnPoints; // Düşmanların doğacağı noktaların listesi
        [SerializeField] private float spawnInterval = 3f; // Kaç saniyede bir düşman doğacak
        [SerializeField] private bool isSpawning = true;

        private void Start()
        {
            // Oyun başlar başlamaz üretimi başlat
            StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            while (isSpawning)
            {
                // Belirlenen süre kadar bekle
                yield return new WaitForSeconds(spawnInterval);

                if (spawnPoints.Length > 0)
                {
                    // Belirlediğimiz noktalardan rastgele birini seç
                    int randomIndex = Random.Range(0, spawnPoints.Length);
                    Transform selectedPoint = spawnPoints[randomIndex];

                    //Object Pool'dan düşmanı seçilen noktaya çağır
                    ObjectPoolManager.Instance.SpawnFromPool(enemyTag, selectedPoint.position, selectedPoint.rotation);
                }
                else
                {
                    Debug.LogWarning("EnemySpawner: No Spawn Point assigned!");
                }
            }
        }
    }
}