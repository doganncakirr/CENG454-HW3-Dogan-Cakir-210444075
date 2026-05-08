using UnityEngine;
using UnityEngine.AI;
using CoreBreach.Interfaces;

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

        private float currentHealth;
        private float nextAttackTime;
        
        private NavMeshAgent agent;
        
        // İki potansiyel hedefimiz
        private Transform playerTransform;
        private Transform coreTransform;
        
        // O anki aktif hedefimiz
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
            
            // Oyuncuyu bul
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;

            // Merkez üssü bul (Oluşturduğumuz GameCore etiketiyle)
            GameObject coreObj = GameObject.FindGameObjectWithTag("GameCore"); 
            if (coreObj != null) coreTransform = coreObj.transform;
        }

        private void Update()
        {
            // Önce kime saldıracağımıza karar ver
            DetermineTarget();

            // Hedef yoksa hiçbir şey yapma
            if (currentTarget == null) return;

            if (agent.isOnNavMesh)
            {
                agent.SetDestination(currentTarget.position);
            }

            // Hedefe yeterince yakınsa ve saldırı süresi geldiyse vur
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);
            if (distanceToTarget <= attackRange && Time.time >= nextAttackTime)
            {
                Attack();
            }
        }

        private void DetermineTarget()
        {
            // İkisi de yoksa hedef yok
            if (playerTransform == null && coreTransform == null)
            {
                currentTarget = null;
                return;
            }

            // Sadece biri varsa direkt ona git
            if (playerTransform == null) 
            {
                currentTarget = coreTransform;
                return;
            }
            if (coreTransform == null)
            {
                currentTarget = playerTransform;
                return;
            }

            // İkisi de hayattaysa (sahnedeyse) mesafeleri ölç
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            float distanceToCore = Vector3.Distance(transform.position, coreTransform.position);

            // Hangisi daha yakınsa mevcut hedefimiz o olsun
            if (distanceToPlayer < distanceToCore)
            {
                currentTarget = playerTransform;
            }
            else
            {
                currentTarget = coreTransform;
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
            Debug.Log($"Enemy took {damageAmount} damage. HP: {currentHealth}");
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            gameObject.SetActive(false);
        }
    }
}