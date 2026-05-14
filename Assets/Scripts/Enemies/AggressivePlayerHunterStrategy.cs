using UnityEngine;
using CoreBreach.Interfaces;

namespace CoreBreach.Enemies
{
    public class AggressivePlayerHunterStrategy : IEnemyTargetingStrategy
    {
        public Transform DetermineTarget(Transform enemyTransform, Transform playerTransform, Transform coreTransform)
        {
            // Oyuncu yaşıyorsa ona saldırır, oyuncu yoksa Core'a saldırır
            if (playerTransform != null) return playerTransform;
            return coreTransform;
        }
    }
}