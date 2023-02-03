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
        public Action WeaponSwitchEvent;

        public float fieldOfViewScoped => currentWeapon.GetFieldOfViewScoped();
        public float zoomInFieldOfView => currentWeapon.GetZoomInFieldOfView();

        public WeaponController[] weapons { get; private set; }
        public WeaponController currentWeapon { get; private set; }
        public bool isFiring { get; private set; }

        private PlayerController _playerController;
        private PlayerAnimationController _playerAnimationController;

        private bool _isReloading;
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

            weapons = GetComponentsInChildren<WeaponController>(true);
            foreach (var weapon in weapons)
            {
                weapon.ReloadFinishedEvent += OnReloadFinished;
                weapon.gameObject.SetActive(false);
            }

            if ((currentWeapon = weapons.First()) == null)
                throw new NullReferenceException("Empty array of `_weapons`.");

            currentWeapon.gameObject.SetActive(true);
            currentWeapon.isPickedUp = true;
            _playerAnimationController.TriggerWeaponSwitchEvent?.Invoke(currentWeapon.animator);
        }

        private void Update()
        {
            _playerAnimationController.TriggerFireAnimationEvent?.Invoke(isFiring &&
                                                                         currentWeapon.CanShoot());
        }

        public bool IsPickedWeaponByIndex(uint weaponIndex)
        {
            return weapons[weaponIndex].isPickedUp;
        }

        private void OnFire(bool isPressedFire)
        {
            isFiring = isPressedFire;

            switch (currentWeapon.isEnoughAmmoInClip)
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
            if (currentWeapon.isEnoughAmmo)
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
            if (currentWeapon.isEnoughAmmo)
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
            if (_previousIndex == weaponIndex || !IsPickedWeaponByIndex(weaponIndex))
                return;
            _previousIndex = weaponIndex;

            _isReloading = false;
            isFiring = false;

            currentWeapon.gameObject.SetActive(false);

            currentWeapon = weapons[weaponIndex];
            currentWeapon.gameObject.SetActive(true);

            _playerAnimationController.TriggerWeaponSwitchEvent?.Invoke(currentWeapon.animator);
            WeaponSwitchEvent?.Invoke();
        }

        private void OnAim(bool isAiming)
        {
            if (isAiming)
                _isReloading = false;
        }

        private void OnPickedWeapon(WeaponItemController weaponItemController)
        {
            var weaponIndex = (uint)weaponItemController.weaponType;
            var weapon = weapons[weaponIndex];
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