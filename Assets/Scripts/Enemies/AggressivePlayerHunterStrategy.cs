using UnityEngine;

namespace CoreBreach.Enemies
{
    [CreateAssetMenu(
        fileName = "AggressivePlayerHunterStrategy",
        menuName = "Core Breach/Enemy Strategies/Aggressive Player Hunter"
    )]
    public class AggressivePlayerHunterStrategy : EnemyTargetingStrategySO
    {
        public override Transform DetermineTarget(
            Transform enemyTransform,
            Transform playerTransform,
            Transform coreTransform
        )
        {
            if (playerTransform != null)
            {
                return playerTransform;
            }

            return coreTransform;
        }
    }
}