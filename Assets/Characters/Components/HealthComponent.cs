using System;
using Game.Characters.Interfaces;
using UnityEngine;

namespace Game.Characters.Components
{
    public sealed class HealthComponent : MonoBehaviour, IHealable, IDamageable
    {
        public event Action DeathEvent;
        public event Action<float, float> OnHealthChange;

        [Header("Stats")] [SerializeField, Range(0.0f, float.MaxValue)]
        private float health = 100.0f;

        private bool isDead => _currentHealth <= float.Epsilon;
        private float _currentHealth;

        private void Awake()
        {
            _currentHealth = health;
        }

        public void TakeDamage(float damageAmount)
        {
            if (isDead)
                return;

            _currentHealth = Mathf.Clamp(_currentHealth - damageAmount, 0.0f, health);
            OnHealthChange?.Invoke(health, _currentHealth);

            if (isDead)
                DeathEvent?.Invoke();
        }

        public void GetHeal(float healAmount)
        {
            _currentHealth = Mathf.Clamp(_currentHealth + healAmount, 0.0f, health);
            OnHealthChange?.Invoke(health, _currentHealth);
        }
    }
}