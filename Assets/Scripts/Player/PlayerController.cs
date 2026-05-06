using UnityEngine;
using CoreBreach.Interfaces;
using CoreBreach.Systems;

namespace CoreBreach.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour, IDamageable
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        
        [Header("Look Settings")]
        [SerializeField] private float mouseSensitivity = 200f; // Fare hassasiyeti
        [SerializeField] private Transform playerCamera; // Ana Kamerayı buraya sürükleyeceğiz
        private float xRotation = 0f;

        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 50f;
        private float currentHealth;

        [Header("Weapon Settings")]
        [SerializeField] private Transform firePoint;
        [SerializeField] private float fireRate = 0.15f;
        private float nextFireTime;

        private Rigidbody rb;
        private Vector2 moveInput;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            currentHealth = maxHealth;

            // Fare imlecini oyun ekranına kilitle ve gizle (Tam bir FPS klasiği)
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            // 1. Hareket Girdileri (WASD)
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");

            // 2. Bakış Girdileri (Mouse Aiming)
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            // Kamerayı Yukarı/Aşağı Döndürme (X ekseni)
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Kafayı geriye katlamamak için -90/90 sınırlandırması
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // Karakteri Sağa/Sola Döndürme (Y ekseni)
            transform.Rotate(Vector3.up * mouseX);

            // 3. Ateş Etme
            if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
            {
                Shoot();
            }
        }

        private void FixedUpdate()
        {
            // FPS tarzı yerel (Local) eksende hareket
            Vector3 moveDirection = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
        }

        private void Shoot()
        {
            nextFireTime = Time.time + fireRate;

            // 1. Kameranın tam ortasından (baktığımız yerden) ileriye görünmez bir ışın (Ray) at
            Ray ray = new Ray(playerCamera.position, playerCamera.forward);
            Vector3 targetPoint;

            // 2. Işın bir şeye çarptı mı kontrol et
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Çarptıysa hedefimiz o çarpma noktasıdır
                targetPoint = hit.point;
            }
            else
            {
                // Çarpmazsa (gökyüzüne falan bakıyorsak) kameranın 100 birim ilerisini hedef al
                targetPoint = ray.GetPoint(100f); 
            }

            // 3. Merminin çıkış noktasından (namludan) hedefe doğru olan açıyı/yönü hesapla
            Vector3 direction = targetPoint - firePoint.position;

            // 4. Havuzdan mermiyi çağır ve hesaplanan bu yeni açıya doğru döndürerek ateşle
            ObjectPoolManager.Instance.SpawnFromPool("PlayerBullet", firePoint.position, Quaternion.LookRotation(direction));
        }

        public void TakeDamage(float damageAmount)
        {
            currentHealth = Mathf.Max(0, currentHealth - damageAmount);
            if (currentHealth <= 0) Die();
        }

        public void Die()
        {
            Debug.Log("Player Died!");
            gameObject.SetActive(false);
        }
    }
}