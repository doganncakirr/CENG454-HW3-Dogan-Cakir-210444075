using System;

namespace CoreBreach.Systems
{
    public static class GameEvents
    {
        // --- PLAYER OLAYLARI ---
        public static event Action<float, float> OnPlayerHealthChanged; // Mevcut Can, Max Can
        public static event Action OnPlayerDied;

        // --- CORE OLAYLARI ---
        public static event Action<float, float> OnCoreHealthChanged;
        public static event Action OnCoreDied;

        // --- ENEMY OLAYLARI ---
        public static event Action<int> OnEnemyDied; // Kazanılan Puan

        // --- GAME STATE OLAYLARI ---
        public static event Action OnGameWon;
        public static event Action<float, float> OnSurvivalTimeChanged; // Kalan Süre, Toplam Süre

        // Sinyalleri Tetikleme Metotları
        public static void FirePlayerHealthChanged(float currentHealth, float maxHealth) => 
            OnPlayerHealthChanged?.Invoke(currentHealth, maxHealth);

        public static void FirePlayerDied() => 
            OnPlayerDied?.Invoke();

        public static void FireCoreHealthChanged(float currentHealth, float maxHealth) => 
            OnCoreHealthChanged?.Invoke(currentHealth, maxHealth);

        public static void FireCoreDied() => 
            OnCoreDied?.Invoke();

        public static void FireEnemyDied(int scoreValue) => 
            OnEnemyDied?.Invoke(scoreValue);

        public static void FireGameWon() => 
            OnGameWon?.Invoke();

        public static void FireSurvivalTimeChanged(float remainingTime, float totalTime) => 
            OnSurvivalTimeChanged?.Invoke(remainingTime, totalTime);
    }
}