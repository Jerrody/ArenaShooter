using Game.Characters.Interfaces;
using UnityEngine;

namespace Game.Items
{
    [RequireComponent(typeof(BoxCollider))]
    public sealed class HealItemController : ItemController
    {
        [Header("Stats")] [SerializeField] private float healAmount;

        private void OnValidate()
        {
            if (!Collider || Collider.isTrigger) return;

            Collider.isTrigger = true;
            Debug.LogWarning("`BoxCollider` of Items should be always set `IsTrigger` to `true`.");
        }

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);

            if (other.gameObject.TryGetComponent<IHealable>(out var healable))
                healable.GetHeal(healAmount);
        }
    }
}