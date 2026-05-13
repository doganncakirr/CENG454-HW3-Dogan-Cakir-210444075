using UnityEngine;
using CoreBreach.Interfaces;

namespace CoreBreach.Weapons
{
    // Bu sınıf soyuttur abstract. Diğer eklentiler bundan türeyecek.
    public abstract class WeaponDecorator : IWeapon
    {
        protected IWeapon decoratedWeapon;

        public WeaponDecorator(IWeapon weapon)
        {
            this.decoratedWeapon = weapon;
        }

        // Eğer eklenti FireRate'i değiştirmezse, orijinal silahın FireRate'ini döndür
        public virtual float FireRate => decoratedWeapon.FireRate;

        // Eğer eklenti ateşlemeyi değiştirmezse, orijinal silahın ateşlemesini çalıştır
        public virtual void Fire(Transform firePoint)
        {
            decoratedWeapon.Fire(firePoint);
        }
    }
}