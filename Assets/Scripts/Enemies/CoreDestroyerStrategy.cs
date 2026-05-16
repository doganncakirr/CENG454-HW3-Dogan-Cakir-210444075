using UnityEngine;

namespace CoreBreach.Enemies
{
    [CreateAssetMenu(
        fileName = "CoreDestroyerStrategy",
        menuName = "Core Breach/Enemy Strategies/Core Destroyer"
    )]
    public class CoreDestroyerStrategy : EnemyTargetingStrategySO
    {
        public override Transform DetermineTarget(
            Transform enemyTransform,
            Transform playerTransform,
            Transform coreTransform
        )
        {
            if (coreTransform != null)
            {
                return coreTransform;
            }

            return playerTransform;
        }
    }
}