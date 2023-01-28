using System;
using UnityEngine;

namespace Game.Weapons
{
    [Serializable]
    internal struct Ammo
    {
        public uint maxAmmo;
        public uint ammoClip;
    }

    public class WeaponController : MonoBehaviour
    {
        public Action ReloadFinishedEvent;

        [Header("Stats")] [SerializeField] protected float fireRate;
        [SerializeField] private float maxHitscanRange = 1000.0f;
        [SerializeField] private Ammo ammo;

        public bool isEnoughAmmo => _currentAmmo > 0;
        public bool isEnoughAmmoInClip => _currentAmmoClip > 0;

        private Animator _animator;

        private float _nextTimeToFire;

        private uint _currentAmmoClip;
        private uint _currentAmmo;


        private void Awake()
        {
            _currentAmmoClip = ammo.ammoClip;
            _currentAmmo = ammo.maxAmmo;
        }

        public void Fire()
        {
            if (!isEnoughAmmoInClip)
                return;

            _currentAmmoClip--;
            print($"{_currentAmmoClip}");

            var ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2.0f, Screen.height / 2.0f));
            if (!Physics.Raycast(ray, out var raycastHit, maxHitscanRange))
            {
            }
        }

        private void Reload()
        {
            var previousAmmo = _currentAmmo;
            _currentAmmo = Math.Clamp(_currentAmmo - (ammo.ammoClip - _currentAmmoClip), 0, _currentAmmo);
            _currentAmmoClip += previousAmmo - _currentAmmo;
            print($"{_currentAmmoClip}/{_currentAmmo}");

            ReloadFinishedEvent?.Invoke();
        }

        public bool CanShoot()
        {
            if (Time.time < _nextTimeToFire || !isEnoughAmmoInClip)
                return false;

            _nextTimeToFire = Time.time + 1.0f / fireRate;

            return true;
        }
    }
}