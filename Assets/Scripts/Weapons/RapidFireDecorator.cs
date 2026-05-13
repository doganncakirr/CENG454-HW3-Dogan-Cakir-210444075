using CoreBreach.Interfaces;

namespace CoreBreach.Weapons
{
    public class RapidFireDecorator : WeaponDecorator
    {
        public RapidFireDecorator(IWeapon weapon) : base(weapon) { }

        //Silahın fireRate'ini yarıya indirir, yani 2 kat hızlı sıkar.
        public override float FireRate => base.FireRate * 0.5f; 
    }
}