using UnityEngine;

namespace CoreBreach.Interfaces
{
    public interface IWeapon
    {
        float FireRate { get; } 
        
        // Silahın ateş etme komutu
        void Fire(Transform firePoint);
    }
}