using UnityEngine;
using UnityEngine.AI;
using CoreBreach.Interfaces;
using CoreBreach.Systems;

namespace CoreBreach.Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyController : MonoBehaviour, IDamageable
    {
        [Header("Enemy Settings")]
        [SerializeField] private float maxHealth = 30f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackRate = 1f;
        [SerializeField] private string poolTag = "BasicEnemy";

        private IEnemyTargetingStrategy targetingStrategy;
        private float currentHealth;
        private bool isDead;
        private float nextAttackTime;
        private NavMeshAgent agent;
        private Transform playerTransform;
        private Transform coreTransform;
        private Transform currentTarget; 

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.stoppingDistance = attackRange;
        }

        private void OnEnable()
        {
            isDead = false;
            currentHealth = maxHealth;
            nextAttackTime = 0f;
            currentTarget = null;

            if (agent != null && agent.isOnNavMesh)
            {
                agent.ResetPath();
                agent.isStopped = false;
            }

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;

            GameObject coreObj = GameObject.FindGameObjectWithTag("GameCore"); 
            if (coreObj != null) 
            {
                coreTransform = coreObj.transform;
            }
            else 
            {
            coreTransform = null;
            }

            if (Random.value > 0.5f)
                targetingStrategy = new CoreDestroyerStrategy();
            else
                targetingStrategy = new AggressivePlayerHunterStrategy();
        }

        private void OnDisable()
        {
            currentTarget = null;

            if (agent != null && agent.isOnNavMesh)
            {
                agent.ResetPath();
                agent.isStopped = true;
            }
        }

        private void Update()
        {
            if (targetingStrategy != null)
            {
                currentTarget = targetingStrategy.DetermineTarget(transform, playerTransform, coreTransform);
            }

            if (currentTarget == null) return;

            if (agent.isOnNavMesh)
            {
                agent.SetDestination(currentTarget.position);
            }

            float distanceToTarget = GetDistanceToTarget();
            if (distanceToTarget <= attackRange && Time.time >= nextAttackTime)
            {
                Attack();
            }
        }

        private float GetDistanceToTarget()
        {
            Collider targetCollider = currentTarget.GetComponent<Collider>();

            if (targetCollider != null)
            {
                Vector3 closestPoint = targetCollider.ClosestPoint(transform.position);
                return Vector3.Distance(transform.position, closestPoint);
            }

            return Vector3.Distance(transform.position, currentTarget.position);
        }

        private void Attack()
        {
            nextAttackTime = Time.time + attackRate;
            IDamageable damageableTarget = currentTarget.GetComponent<IDamageable>();
            if (damageableTarget != null)
            {
                damageableTarget.TakeDamage(damage);
                Debug.Log($"Enemy attacked {currentTarget.name} for {damage} damage!");
            }
        }

        public void TakeDamage(float damageAmount)
        {
            currentHealth = Mathf.Max(0, currentHealth - damageAmount);
            if (currentHealth <= 0) Die();
        }

        public void Die()
        {
            if (isDead) return;

            isDead = true;
            GameEvents.FireEnemyDied(10);

            if (ObjectPoolManager.Instance != null)
            {
                ObjectPoolManager.Instance.ReturnToPool(poolTag, gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}