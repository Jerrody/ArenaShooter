using System;
using UnityEngine;

namespace Game.Weapons
{
    [Serializable]
    public struct Ammo
    {
        public uint maxAmmo;
        public uint ammoClip;
    }

    [RequireComponent(typeof(Animator))]
    public class WeaponController : MonoBehaviour
    {
        public Action ReloadFinishedEvent;

        [Header("Stats")] [SerializeField] protected float fireRate;
        [SerializeField] private float maxHitscanRange = 1000.0f;
        [SerializeField] protected Ammo ammo;

        [Header("Field of View Stats")] [SerializeField]
        private float fieldOfViewScoped = 45.0f;

        [SerializeField] private float zoomInFieldOfView = 35.0f;

        public Animator animator { get; private set; }

        public bool isEnoughAmmo => _currentAmmo > 0;
        public bool isEnoughAmmoInClip => _currentAmmoClip > 0;

        private float _nextTimeToFire;

        private uint _currentAmmoClip;
        private uint _currentAmmo;


        private void Awake()
        {
            animator = GetComponent<Animator>();

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
            if (isEnoughAmmo)
            {
                var previousAmmo = _currentAmmo;
                _currentAmmo = Math.Clamp(_currentAmmo - (ammo.ammoClip - _currentAmmoClip), 0, _currentAmmo);
                _currentAmmoClip += previousAmmo - _currentAmmo;
                print($"{_currentAmmoClip}/{_currentAmmo}");
            }

            ReloadFinishedEvent?.Invoke();
        }

        public bool CanShoot()
        {
            if (Time.time < _nextTimeToFire || !isEnoughAmmoInClip)
                return false;

            _nextTimeToFire = Time.time + 1.0f / fireRate;

            return true;
        }

        public float GetFieldOfViewScoped()
        {
            return fieldOfViewScoped;
        }

        public float GetZoomInFieldOfView()
        {
            return zoomInFieldOfView;
        }
    }
}