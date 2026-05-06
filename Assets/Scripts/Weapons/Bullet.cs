using UnityEngine;
using CoreBreach.Interfaces;

namespace CoreBreach.Weapons
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed = 20f;
        [SerializeField] private float damage = 10f;
        [SerializeField] private float lifeTime = 2f; // Ekranda kalma süresi

        private void OnEnable()
        {
            // Mermi havuzdan çıkıp aktif olduğunda, belli bir süre sonra kendini kapatması için sayacı başlat
            Invoke(nameof(Deactivate), lifeTime);
        }

        private void OnDisable()
        {
            // Mermi kapandığında sayacı sıfırla ki bir sonraki çıkışta bug olmasın
            CancelInvoke();
        }

        private void Update()
        {
            // Mermiyi kendi Z ekseninde (ileri doğru) hareket ettir
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            // Çarptığımız objede IDamageable arayüzü var mı kontrol et
            IDamageable damageable = other.GetComponent<IDamageable>();
            
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            // Çarptıktan sonra yok olma, havuza geri dön!
            Deactivate();
        }

        private void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}