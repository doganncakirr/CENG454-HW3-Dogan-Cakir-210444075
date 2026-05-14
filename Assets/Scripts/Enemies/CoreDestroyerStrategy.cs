using UnityEngine;
using CoreBreach.Interfaces;

namespace CoreBreach.Enemies
{
    public class CoreDestroyerStrategy : IEnemyTargetingStrategy
    {
        public Transform DetermineTarget(Transform enemyTransform, Transform playerTransform, Transform coreTransform)
        {
            // Öncelik Core.
            if (coreTransform != null)
            {
                return coreTransform;
            }
            
            // Eğer Core yok edildiyse oyuncuya saldır!
            return playerTransform;
        }
    }
}