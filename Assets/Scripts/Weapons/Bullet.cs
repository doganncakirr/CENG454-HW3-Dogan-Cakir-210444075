using UnityEngine;
using CoreBreach.Interfaces;
using CoreBreach.Systems;

namespace CoreBreach.Weapons
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed = 20f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private float lifeTime = 2f; // Ekranda kalma süresi

        private bool hasReturnedToPool; // Merminin iki kez havuza dönmesini engeller
        private void OnEnable()
        {
            hasReturnedToPool = false;
            Invoke(nameof(Deactivate), lifeTime);
        }

        private void OnDisable()
        {
            // Mermi kapandığında sayacı sıfırla ki bir sonraki çıkışta bug olmasın
            CancelInvoke();
        }

        private void Update()
        {
            // Mermiyi kendi Z ekseninde hareket ettir
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                return;
            }

            IDamageable damageable = other.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                Deactivate();
                return;
            }

            Deactivate();
        }

        private void Deactivate()
        {
            if (hasReturnedToPool) return;

            hasReturnedToPool = true;

            if (ObjectPoolManager.Instance != null)
            {
                ObjectPoolManager.Instance.ReturnToPool(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}