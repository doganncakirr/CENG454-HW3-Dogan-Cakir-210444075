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
            // 1. Merkez Mermi
            base.Fire(firePoint);

            // Hedef noktayı hesapla
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit) ? hit.point : ray.GetPoint(100f);
            Vector3 centerDirection = (targetPoint - firePoint.position).normalized;

            float offsetDistance = 0.14f; // Mermileri yanlara kaydırma mesafesi
            float spreadAngle = 7f;      // Dışarı doğru açılma açısı

            // 2. Sağdaki Mermi
            Vector3 rightPos = firePoint.position + firePoint.right * offsetDistance;
            Vector3 rightDir = Quaternion.Euler(0, spreadAngle, 0) * centerDirection;
            ObjectPoolManager.Instance.SpawnFromPool("PlayerBullet", rightPos, Quaternion.LookRotation(rightDir));

            // 3. Soldaki Mermi
            Vector3 leftPos = firePoint.position - firePoint.right * offsetDistance;
            Vector3 leftDir = Quaternion.Euler(0, -spreadAngle, 0) * centerDirection;
            ObjectPoolManager.Instance.SpawnFromPool("PlayerBullet", leftPos, Quaternion.LookRotation(leftDir));
        }
    }
}