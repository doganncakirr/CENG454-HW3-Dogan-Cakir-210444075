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

        [Header("Ability Settings")]
        [SerializeField] private float tripleShotCooldown = 12f;
        [SerializeField] private float tripleShotDuration = 5f;
        [SerializeField] private float rapidFireCooldown = 10f;
        [SerializeField] private float rapidFireDuration = 3.5f;
        
        // Yetenek takibi için zamanlayıcılar. -1 demek yetenek şu an aktif değil demek
        private float rapidFireEndTime = -1f;
        private float tripleShotEndTime = -1f;

        // Cooldown takibi için zamanlayıcılar. Yetenek bittikten sonra sayacak
        private float nextRapidFireAvailableTime = 0f;
        private float nextTripleShotAvailableTime = 0f;

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

            // 3. Silah ve Yetenek Kontrolleri
            HandleWeaponState();
            HandleFiring();
            HandleAbilityInputs();
        }

        private void HandleWeaponState()
        {
            if (rapidFireEndTime > 0 && Time.time >= rapidFireEndTime)
            {
                ResetWeapon();
                rapidFireEndTime = -1f; // Yeteneği kapat
                nextRapidFireAvailableTime = Time.time + rapidFireCooldown; // Cooldown'ı BAŞLAT
                Debug.Log("Rapid Fire bitti, Cooldown başladı.");
            }

            if (tripleShotEndTime > 0 && Time.time >= tripleShotEndTime)
            {
                ResetWeapon();
                tripleShotEndTime = -1f; // Yeteneği kapat
                nextTripleShotAvailableTime = Time.time + tripleShotCooldown; // Cooldown'ı BAŞLAT
                Debug.Log("Triple Shot bitti, Cooldown başladı.");
            }
        }

        private void HandleFiring()
        {
            if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + currentWeapon.FireRate; 
                currentWeapon.Fire(firePoint);
            }
        }

        private void HandleAbilityInputs()
        {
            // Klavyeden 1'e basınca RapidFire modülü takılsın
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                // Yetenek hazırsa ve şu an zaten aktif değilse çalıştır
                if (Time.time >= nextRapidFireAvailableTime && rapidFireEndTime < 0)
                {
                    currentWeapon = new RapidFireDecorator(currentWeapon);
                    rapidFireEndTime = Time.time + rapidFireDuration; // Kapanacağı zamanı ayarla
                    Debug.Log($"Rapid Fire AKTİF ({rapidFireDuration} sn)");
                }
                else if (rapidFireEndTime < 0) // Eğer aktif değilse ama basıldıysa cooldown uyarısı ver
                {
                    float remaining = nextRapidFireAvailableTime - Time.time;
                    Debug.Log($"RapidFire Cooldown: {remaining:F1}s kaldı.");
                }
            }
            
            // "2'ye" basınca TripleShot modülü takılsın
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                // Yetenek hazırsa ve şu an zaten aktif değilse çalıştır
                if (Time.time >= nextTripleShotAvailableTime && tripleShotEndTime < 0)
                {
                    currentWeapon = new TripleShotDecorator(currentWeapon);
                    tripleShotEndTime = Time.time + tripleShotDuration; // Kapanacağı zamanı ayarla
                    Debug.Log($"Triple Shot AKTİF ({tripleShotDuration} sn)");
                }
                else if (tripleShotEndTime < 0)
                {
                    float remaining = nextTripleShotAvailableTime - Time.time;
                    Debug.Log($"TripleShot için {remaining:F1}s bekle!");
                }
            }
        }

        private void ResetWeapon()
        {
            // Yetenek bitince temel silaha geri dön
            currentWeapon = new BasicBlaster();
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