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
            // 1. mermi
            base.Fire(firePoint);

            //  Üç merminin de aynı noktaya odaklanması için hesaplama.
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            Vector3 targetPoint;
            
            if (Physics.Raycast(ray, out RaycastHit hit))
                targetPoint = hit.point;
            else
                targetPoint = ray.GetPoint(100f);

            // 2. mermi (sağ)
            Vector3 rightOffsetPosition = firePoint.position + firePoint.right * 0.5f;
            Vector3 rightDirection = targetPoint - rightOffsetPosition;
            ObjectPoolManager.Instance.SpawnFromPool("PlayerBullet", rightOffsetPosition, Quaternion.LookRotation(rightDirection));

            // 3. mermi (sol)
            Vector3 leftOffsetPosition = firePoint.position - firePoint.right * 0.5f;
            Vector3 leftDirection = targetPoint - leftOffsetPosition;
            ObjectPoolManager.Instance.SpawnFromPool("PlayerBullet", leftOffsetPosition, Quaternion.LookRotation(leftDirection));
        }
    }
}