using Game.Characters.Interfaces;
using UnityEngine;

namespace Characters.Enemy.Components
{
    public sealed class HitAreaComponent : MonoBehaviour
    {
        [SerializeField] private uint attackDamage;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<IDamageable>(out var damageable) ||
                other.gameObject.layer == EnemyController.layerMask) return;

            gameObject.SetActive(false);
            damageable.TakeDamage(attackDamage);
        }
    }
}