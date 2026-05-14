using UnityEngine;

namespace CoreBreach.Interfaces
{
    public interface IEnemyTargetingStrategy
    {
        Transform DetermineTarget(Transform enemyTransform, Transform playerTransform, Transform coreTransform);
    }
}