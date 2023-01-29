using System;
using System.Linq;
using Game.Characters.Player;
using UnityEngine;

namespace Game.Weapons
{
    public sealed class WeaponHolderController : MonoBehaviour
    {
        private WeaponController[] _weapons;
        private WeaponController _currentWeapon;

        private PlayerController _playerController;
        private PlayerAnimationController _playerAnimationController;

        public bool isReloading { get; private set; }

        private bool _isFiring;

        private void Awake()
        {
            _playerAnimationController = GetComponentInParent<PlayerAnimationController>();

            _playerController = GetComponentInParent<PlayerController>();
            _playerController.FireEvent += OnFire;
            _playerController.ReloadEvent += OnReload;

            _weapons = GetComponentsInChildren<WeaponController>();
            foreach (var weapon in _weapons)
            {
                weapon.ReloadFinishedEvent += OnReloadFinished;
                weapon.gameObject.SetActive(false);
            }

            if ((_currentWeapon = _weapons.First()) == null)
                throw new NullReferenceException("[ERROR]: Empty array of `Weapons`.");

            _currentWeapon.gameObject.SetActive(true);
        }

        private void Update()
        {
            _playerAnimationController.FireAnimationEvent?.Invoke(_isFiring &&
                                                                  _currentWeapon.CanShoot());
        }

        private void OnFire(bool isFiring)
        {
            _isFiring = isFiring;

            if (_currentWeapon.isEnoughAmmoInClip)
            {
                isReloading = false;
                return;
            }

            if (isReloading)
                return;

            isReloading = true;
            if (_currentWeapon.isEnoughAmmo)
            {
                _playerAnimationController.ReloadAmmoAnimationEvent?.Invoke();
            }
            else
            {
                _playerAnimationController.ReloadNoAmmoAnimationEvent?.Invoke();
            }

            _playerController.AimEvent?.Invoke(false);
        }

        private void OnReload()
        {
            if (isReloading)
                return;

            isReloading = true;
            if (_currentWeapon.isEnoughAmmo)
            {
                _playerAnimationController.ReloadAmmoAnimationEvent?.Invoke();
            }
            else
            {
                _playerAnimationController.ReloadNoAmmoAnimationEvent?.Invoke();
            }
        }

        private void OnReloadFinished()
        {
            isReloading = false;
            _playerController.AimEvent?.Invoke(_playerController.isAiming);
        }
    }
}