using Game.Characters.Components;
using UnityEngine;

namespace Game.Characters
{
    [RequireComponent(typeof(HealthComponent))]
    public abstract class EntityController : MonoBehaviour
    {
        [Header("Stats")] [SerializeField] protected float walkSpeed = 20.0f;

        public HealthComponent health { get; protected set; }
    }
}