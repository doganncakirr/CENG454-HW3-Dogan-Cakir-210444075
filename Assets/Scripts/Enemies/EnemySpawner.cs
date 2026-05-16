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

        private Coroutine spawnRoutine;

        private void OnEnable()
        {
            GameEvents.OnGameWon += StopSpawning;
            GameEvents.OnPlayerDied += StopSpawning;
            GameEvents.OnCoreDied += StopSpawning;
        }

        private void OnDisable()
        {
            GameEvents.OnGameWon -= StopSpawning;
            GameEvents.OnPlayerDied -= StopSpawning;
            GameEvents.OnCoreDied -= StopSpawning;
        }

        private void Start()
        {
            if (isSpawning)
            {
                spawnRoutine = StartCoroutine(SpawnRoutine());
            }
        }

        private IEnumerator SpawnRoutine()
        {
            while (isSpawning)
            {
                yield return new WaitForSeconds(spawnInterval);

                if (!isSpawning) yield break;

                if (spawnPoints.Length > 0)
                {
                    int randomIndex = Random.Range(0, spawnPoints.Length);
                    Transform selectedPoint = spawnPoints[randomIndex];

                    ObjectPoolManager.Instance.SpawnFromPool(enemyTag, selectedPoint.position, selectedPoint.rotation);
                }
                else
                {
                    Debug.LogWarning("EnemySpawner: No Spawn Point assigned!");
                }
            }
        }

        private void StopSpawning()
        {
            isSpawning = false;

            if (spawnRoutine != null)
            {
                StopCoroutine(spawnRoutine);
                spawnRoutine = null;
            }
        }
    }
}