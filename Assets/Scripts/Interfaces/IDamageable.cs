namespace CoreBreach.Interfaces
{
    public interface IDamageable
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }

        void TakeDamage(float damageAmount);
        void Die();
    }
}