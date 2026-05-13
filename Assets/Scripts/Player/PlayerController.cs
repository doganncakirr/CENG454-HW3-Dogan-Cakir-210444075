using UnityEngine;
using CoreBreach.Interfaces;
using CoreBreach.Systems;
using CoreBreach.Weapons;

namespace CoreBreach.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour, IDamageable
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        
        [Header("Look Settings")]
        [SerializeField] private float mouseSensitivity = 200f; 
        [SerializeField] private Transform playerCamera;
        private float xRotation = 0f;

        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;
        private float currentHealth;

        [Header("Weapon Settings")]
        [SerializeField] private Transform firePoint;
        private float nextFireTime;
        private IWeapon currentWeapon;

        private Rigidbody rb;
        private Vector2 moveInput;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            currentHealth = maxHealth;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            currentWeapon = new BasicBlaster();
        }

        private void Start()
        {
            GameEvents.FirePlayerHealthChanged(currentHealth, maxHealth);
        }

        private void Update()
        {
            // 1. Hareket Girdileri
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");

            // 2. Kamera ve Yön Girdileri
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f); 
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            transform.Rotate(Vector3.up * mouseX);

            // 3. YENİ ATEŞ ETME SİSTEMİ Decorator Pattern ile
            if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + currentWeapon.FireRate; 
                currentWeapon.Fire(firePoint);
            }

            // Klavyeden 1'e basınca RapidFire modülü takılsın
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                currentWeapon = new RapidFireDecorator(currentWeapon);
                Debug.Log("Seri Ateş Modülü Takıldı! Yeni Hız: " + currentWeapon.FireRate);
            }
            
            // Klavyeden 2'ye basınca TripleShot modülü takılsın
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                currentWeapon = new TripleShotDecorator(currentWeapon);
                Debug.Log("Üçlü Mermi Modülü Takıldı!");
            }
        }

        private void FixedUpdate()
        {
            Vector3 moveDirection = (transform.right * moveInput.x + transform.forward * moveInput.y).normalized;
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
        }

        public void TakeDamage(float damageAmount)
        {
            currentHealth = Mathf.Max(0, currentHealth - damageAmount);
            GameEvents.FirePlayerHealthChanged(currentHealth, maxHealth);

            if (currentHealth <= 0) Die();
        }

        public void Die()
        {
            Debug.Log("Player Died!");
            GameEvents.FirePlayerDied();
        }
    }
}