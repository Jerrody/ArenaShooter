using Game.Characters;
using UnityEngine;

namespace Game
{
    public sealed class HealthComponent : MonoBehaviour, IHealable, IDamageable
    {
        [Header("Stats")] [SerializeField, Range(0.0f, float.MaxValue)]
        private float health = 100.0f;

        public bool isDead => _currentHealth <= 0.0f;

        private float _currentHealth;

        private void Awake()
        {
            _currentHealth = health;
        }

        public void GetDamage(float damageAmount)
        {
            _currentHealth = Mathf.Clamp(_currentHealth - damageAmount, 0.0f, health);
        }

        public void GetHeal(float healAmount)
        {
            _currentHealth = Mathf.Clamp(_currentHealth + healAmount, 0.0f, health);
        }
    }
}