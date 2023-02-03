using Game.Characters.Player;
using Game.Weapons;
using UnityEngine;

namespace Game.Items.Weapons
{
    public sealed class WeaponItemController : ItemController
    {
        [Header("Stats")] [SerializeField] private uint ammoAmount = 30;
        [SerializeField] private WeaponType weaponName;

        public WeaponType weaponType => weaponName;
        public uint ammo => ammoAmount;

        private void OnValidate()
        {
            if (!Collider || Collider.isTrigger) return;

            Collider.isTrigger = true;
            Debug.LogWarning("`BoxCollider` of Items should be always set `IsTrigger` to `true`.");
        }

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);

            if (!other.gameObject.TryGetComponent<PlayerController>(out var playerController)) return;

            playerController.weaponHolderController.PickedWeaponEvent?.Invoke(this);
        }
    }
}