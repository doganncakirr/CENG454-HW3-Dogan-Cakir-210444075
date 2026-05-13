using UnityEngine;
using CoreBreach.Interfaces;
using CoreBreach.Systems;

namespace CoreBreach.Weapons
{
    //silahımız
    public class BasicBlaster : IWeapon
    {
        private float baseFireRate = 0.15f;

        public float FireRate => baseFireRate;

        public void Fire(Transform firePoint)
        {
            // Mermiyi gideceği yöne doğru hesaplaması
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            Vector3 targetPoint;
            
            if (Physics.Raycast(ray, out RaycastHit hit))
                targetPoint = hit.point;
            else
                targetPoint = ray.GetPoint(100f);

            Vector3 direction = targetPoint - firePoint.position;
            
            // Havuzdan mermiyi çağır
            ObjectPoolManager.Instance.SpawnFromPool("PlayerBullet", firePoint.position, Quaternion.LookRotation(direction));
        }
    }
}