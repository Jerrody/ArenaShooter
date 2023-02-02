using System;
using Game.Global.Data;
using UnityEngine;

namespace Game.Weapons
{
    [Serializable]
    public enum WeaponType : uint
    {
        Ak,
        Shotgun,
        Smg,
    }

    [Serializable]
    public struct Ammo
    {
        public uint maxAmmo;
        public uint ammoClip;
    }

    // TODO: Make WeaponController derivable for the children and create an interface.
    [RequireComponent(typeof(Animator))]
    public class WeaponController : MonoBehaviour
    {
        public Action ReloadFinishedEvent;

        [Header("References")] [SerializeField]
        protected Transform bulletSpawnPoint;

        [SerializeField] protected GameObject scope;

        [Header("Info")] [SerializeField] protected WeaponType weaponType;

        [Header("Stats")] [SerializeField] protected float damage = 30.0f;
        [SerializeField] protected float fireRate;
        [SerializeField] protected float maxHitscanRange = 1000.0f;
        [SerializeField] protected Ammo ammo;

        [Header("Field of View Stats")] [SerializeField]
        private float fieldOfViewScoped = 45.0f;

        [SerializeField] private float zoomInFieldOfView = 35.0f;

        public Animator animator { get; private set; }

        private ParticleSystem _muzzleFlesh;

        public bool isEnoughAmmo => _currentAmmo > 0;
        public bool isEnoughAmmoInClip => _currentAmmoClip > 0;

        public bool isPickedUp;

        private float _nextTimeToFire;
        private uint _currentAmmoClip;
        private uint _currentAmmo;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            _muzzleFlesh = GetComponentInChildren<ParticleSystem>();

            if (!Data.jsonData.weaponData[(int)weaponType].isScopeSet)
                scope.gameObject.SetActive(false);

            _currentAmmoClip = ammo.ammoClip;
            _currentAmmo = ammo.maxAmmo;
        }

        public void Fire()
        {
            if (!isEnoughAmmoInClip)
                return;

            _currentAmmoClip--;
            print($"{_currentAmmoClip}");

            _muzzleFlesh.Play();

            FireRaycast();
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

        public void SetAmmo(uint ammoAmount)
        {
            _currentAmmo = Math.Clamp(_currentAmmo + ammoAmount, 0, ammo.ammoClip - _currentAmmoClip + ammo.maxAmmo);
        }

        protected virtual void FireRaycast()
        {
            if (!Physics.Raycast(bulletSpawnPoint.position, transform.TransformDirection(Vector3.forward),
                    out var raycastHit, maxHitscanRange)) return;

            Debug.DrawRay(bulletSpawnPoint.position, transform.TransformDirection(Vector3.forward) * 10.0f,
                Color.red, 20.0f);
        }

        private void Reload()
        {
            if (isEnoughAmmo)
            {
                var previousAmmo = _currentAmmo;
                _currentAmmo = Math.Clamp(_currentAmmo - (ammo.ammoClip - _currentAmmoClip), 0, ammo.maxAmmo);
                _currentAmmoClip = previousAmmo - _currentAmmo + _currentAmmoClip;
                print($"{_currentAmmoClip}/{_currentAmmo}");
            }

            ReloadFinishedEvent?.Invoke();
        }
    }
}