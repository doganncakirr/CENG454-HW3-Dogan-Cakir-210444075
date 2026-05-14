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

        private IEnemyTargetingStrategy targetingStrategy;
        private float currentHealth;
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
        }

        private void OnEnable()
        {
            currentHealth = maxHealth;
            
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

            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
            if (distanceToTarget <= attackRange && Time.time >= nextAttackTime)
            {
                Attack();
            }
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
            GameEvents.FireEnemyDied(10);
            gameObject.SetActive(false);
        }
    }
}