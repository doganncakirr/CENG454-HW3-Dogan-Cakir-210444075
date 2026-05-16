using UnityEngine;
using CoreBreach.Interfaces;
using CoreBreach.Systems;

namespace CoreBreach.Weapons
{
    public class TripleShotDecorator : WeaponDecorator
    {
        public TripleShotDecorator(IWeapon weapon) : base(weapon) { }

        public override void Fire(Transform firePoint)
        {
            // Hedef noktayı hesapla
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(100f);
            Vector3 centerDirection = (targetPoint - firePoint.position).normalized;

            float sideOffset = 0.18f;      // Sağ-sol mermi aralığı
            float forwardOffset = 0.45f;   // Mermileri player collider'ından biraz önde başlatır

            Quaternion bulletRotation = Quaternion.LookRotation(centerDirection);

            // 1. Ana mermi
            Vector3 centerPos = firePoint.position + centerDirection * forwardOffset;
            ObjectPoolManager.Instance.SpawnFromPool("PlayerBullet", centerPos, bulletRotation);

            // 2. Sağ mermi
            Vector3 rightPos = firePoint.position + firePoint.right * sideOffset + centerDirection * forwardOffset;
            ObjectPoolManager.Instance.SpawnFromPool("PlayerBullet", rightPos, bulletRotation);

            // 3. Sol mermi
            Vector3 leftPos = firePoint.position - firePoint.right * sideOffset + centerDirection * forwardOffset;
            ObjectPoolManager.Instance.SpawnFromPool("PlayerBullet", leftPos, bulletRotation);
        }
    }
}