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

        public float fieldOfViewScoped => _currentWeapon.GetFieldOfViewScoped();
        public float zoomInFieldOfView => _currentWeapon.GetZoomInFieldOfView();

        private bool _isReloading;

        private bool _isFiring;
        private uint _previousIndex;

        private void Awake()
        {
            _playerAnimationController = GetComponentInParent<PlayerAnimationController>();

            _playerController = GetComponentInParent<PlayerController>();
            _playerController.FireEvent += OnFire;
            _playerController.ReloadEvent += OnReload;
            _playerController.WeaponSwitchEvent += OnWeaponSwitch;

            _weapons = GetComponentsInChildren<WeaponController>(true);
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
            _playerAnimationController.TriggerFireAnimationEvent?.Invoke(_isFiring &&
                                                                         _currentWeapon.CanShoot());
        }

        private void OnFire(bool isFiring)
        {
            _isFiring = isFiring;

            if (_currentWeapon.isEnoughAmmoInClip)
            {
                _isReloading = false;
                return;
            }

            if (_isReloading || !_isFiring)
                return;

            _isReloading = true;
            if (_currentWeapon.isEnoughAmmo)
            {
                _playerAnimationController.TriggerReloadAmmoAnimationEvent?.Invoke();
            }
            else
            {
                _playerAnimationController.TriggerReloadNoAmmoAnimationEvent?.Invoke();
            }

            _playerController.AimEvent?.Invoke(false);
        }

        private void OnReload()
        {
            if (_isReloading)
                return;

            _isReloading = true;
            if (_currentWeapon.isEnoughAmmo)
            {
                _playerAnimationController.TriggerReloadAmmoAnimationEvent?.Invoke();
            }
            else
            {
                _playerAnimationController.TriggerReloadNoAmmoAnimationEvent?.Invoke();
            }
        }

        private void OnReloadFinished()
        {
            _isReloading = false;
            _playerController.AimEvent?.Invoke(_playerController.isAiming);
        }

        private void OnWeaponSwitch(uint weaponIndex)
        {
            if (_previousIndex == weaponIndex)
                return;
            _previousIndex = weaponIndex;

            _isReloading = false;
            _isFiring = false;

            _currentWeapon.gameObject.SetActive(false);

            _currentWeapon = _weapons[weaponIndex];
            _currentWeapon.gameObject.SetActive(true);
            _playerAnimationController.TriggerWeaponSwitchEvent?.Invoke(_currentWeapon.animator);
        }
    }
}