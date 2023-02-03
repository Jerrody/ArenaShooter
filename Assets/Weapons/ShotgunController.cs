using Game.Characters.Interfaces;
using UnityEngine;

namespace Game.Weapons
{
    public sealed class ShotgunController : WeaponController
    {
        [Header("Stats")] [SerializeField] private uint fractionPerFire = 6;
        [SerializeField] private Vector3 bulletSpread = new(0.1f, 0.1f, 0.1f);

        private Vector3 _forwardDirection;

        protected override void FireRaycast()
        {
            for (var i = 0; i < fractionPerFire; i++)
            {
                var direction = GetFractionDirection();
                if (!Physics.Raycast(bulletSpawnPoint.position, direction,
                        out var raycastHit, maxHitscanRange)) return;

                if (raycastHit.collider.gameObject.TryGetComponent<IDamageable>(out var damageable))
                    damageable.TakeDamage(damage);
            }
        }

        private Vector3 GetFractionDirection()
        {
            var direction = transform.TransformDirection(Vector3.forward);

            direction += new Vector3(
                Random.Range(-bulletSpread.x, bulletSpread.x),
                Random.Range(-bulletSpread.y, bulletSpread.y),
                Random.Range(-bulletSpread.z, bulletSpread.z)
            );
            direction.Normalize();

            return direction;
        }
    }
}