using System;
using UnityEngine;

namespace Game.Characters.Player
{
    public sealed class PlayerAnimationController : MonoBehaviour // TODO: Make based class for the all Entities.
    {
        public Action<bool> FireAnimationEvent;
        public Action ReloadAmmoAnimationEvent;
        public Action ReloadNoAmmoAnimationEvent;

        private static readonly int IsAimingId = Animator.StringToHash("IsAiming");
        private static readonly int IsFiringId = Animator.StringToHash("IsFiring");
        private static readonly int ReloadAmmoId = Animator.StringToHash("ReloadAmmo");
        private static readonly int ReloadNoAmmoId = Animator.StringToHash("ReloadNoAmmo");
        private static readonly int IsMovingId = Animator.StringToHash("IsMoving");
        private static readonly int IsJumpingId = Animator.StringToHash("IsJumping");

        [Header("Stats")] [SerializeField] private float runAnimationSpeed = 1.2f;

        private Animator _animator;
        private PlayerController _playerController;

        private void Awake()
        {
            _animator =
                GetComponentInChildren<Animator>(); // Move logic of getting animator to the `WeaponHolderController`.
            _playerController = GetComponentInParent<PlayerController>();
            _playerController.RunEvent += OnRun;

            FireAnimationEvent += OnFire;
            ReloadAmmoAnimationEvent += OnReload;
            ReloadNoAmmoAnimationEvent += OnReloadNoAmmo;
        }

        private void Update()
        {
            _animator.SetBool(IsAimingId, _playerController.isAiming);
            _animator.SetBool(IsMovingId, _playerController.isMoving);
            _animator.SetBool(IsJumpingId, _playerController.isJumping);
        }

        private void OnRun(bool isRunning)
        {
            _animator.speed = isRunning ? runAnimationSpeed : 1.0f;
        }

        private void OnFire(bool isFiring)
        {
            _animator.SetBool(IsFiringId, isFiring);
        }

        private void OnReload()
        {
            _animator.SetTrigger(ReloadAmmoId);
        }

        private void OnReloadNoAmmo()
        {
            _animator.SetTrigger(ReloadNoAmmoId);
        }
    }
}