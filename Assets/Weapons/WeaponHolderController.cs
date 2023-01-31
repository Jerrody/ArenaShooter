using System;
using System.Linq;
using Game.Characters.Player;
using Game.Items.Weapons;
using UnityEngine;

namespace Game.Weapons
{
    public sealed class WeaponHolderController : MonoBehaviour
    {
        public Action<WeaponItemController> PickedWeaponEvent;

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
            _playerController.AimEvent += OnAim;

            PickedWeaponEvent += OnPickedWeapon;

            _weapons = GetComponentsInChildren<WeaponController>(true);
            foreach (var weapon in _weapons)
            {
                weapon.ReloadFinishedEvent += OnReloadFinished;
                weapon.gameObject.SetActive(false);
            }

            if ((_currentWeapon = _weapons.First()) == null)
                throw new NullReferenceException("Empty array of `_weapons`.");

            _currentWeapon.gameObject.SetActive(true);
            _currentWeapon.isPickedUp = true;
            _playerAnimationController.TriggerWeaponSwitchEvent?.Invoke(_currentWeapon.animator);
        }

        private void Update()
        {
            _playerAnimationController.TriggerFireAnimationEvent?.Invoke(_isFiring &&
                                                                         _currentWeapon.CanShoot());
        }

        private void OnFire(bool isFiring)
        {
            _isFiring = isFiring;

            switch (_currentWeapon.isEnoughAmmoInClip)
            {
                case false when !isFiring:
                    return;
                case true:
                    _isReloading = false;
                    return;
            }

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
            if (_previousIndex == weaponIndex || !_weapons[weaponIndex].isPickedUp)
                return;
            _previousIndex = weaponIndex;

            _isReloading = false;
            _isFiring = false;

            _currentWeapon.gameObject.SetActive(false);

            _currentWeapon = _weapons[weaponIndex];
            _currentWeapon.gameObject.SetActive(true);
            _playerAnimationController.TriggerWeaponSwitchEvent?.Invoke(_currentWeapon.animator);
        }

        private void OnAim(bool isAiming)
        {
            if (isAiming)
                _isReloading = false;
        }

        private void OnPickedWeapon(WeaponItemController weaponItemController)
        {
            var weaponIndex = (uint)weaponItemController.weaponType;
            var weapon = _weapons[weaponIndex];
            if (weapon.isPickedUp)
            {
                weapon.SetAmmo(weaponItemController.ammo);
            }
            else
            {
                weapon.isPickedUp = true;
                OnWeaponSwitch(weaponIndex);
            }
        }
    }
}