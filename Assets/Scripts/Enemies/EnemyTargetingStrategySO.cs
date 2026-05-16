using UnityEngine;
using CoreBreach.Interfaces;

namespace CoreBreach.Enemies
{
    public abstract class EnemyTargetingStrategySO : ScriptableObject, IEnemyTargetingStrategy
    {
        public abstract Transform DetermineTarget(
            Transform enemyTransform,
            Transform playerTransform,
            Transform coreTransform
        );
    }
}