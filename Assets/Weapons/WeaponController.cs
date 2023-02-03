using System;
using Game.Characters.Interfaces;
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

    [RequireComponent(typeof(Animator))]
    public class WeaponController : MonoBehaviour
    {
        public Action ReloadFinishedEvent;
        public Action FireEvent;

        [Header("References")]
        [SerializeField] protected Transform bulletSpawnPoint;
        [SerializeField] protected GameObject scope;

        [Header("Info")] 
        [SerializeField] protected WeaponType weaponType;

        [Header("Stats")]
        [SerializeField] protected float damage = 30.0f;
        [SerializeField] protected float fireRate;
        [SerializeField] protected float maxHitscanRange = 1000.0f;
        [SerializeField] protected Ammo ammo;

        [Header("Field of View Stats")]
        [SerializeField] private float fieldOfViewScoped = 45.0f;
        [SerializeField] private float zoomInFieldOfView = 35.0f;

        public Animator animator { get; private set; }
        public uint currentAmmoClip { get; private set; }
        public uint currentAmmo { get; private set; }

        public WeaponType type => weaponType;
        public bool isEnoughAmmo => currentAmmo > 0;
        public bool isEnoughAmmoInClip => currentAmmoClip > 0;

        public bool isPickedUp;

        private ParticleSystem _muzzleFlesh;
        private float _nextTimeToFire;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            _muzzleFlesh = GetComponentInChildren<ParticleSystem>();

            currentAmmoClip = ammo.ammoClip;
            currentAmmo = ammo.maxAmmo;
        }

        private void Start()
        {
            if (!Data.jsonData.weaponData[(int)weaponType].isScopeSet)
                scope.gameObject.SetActive(false);
        }

        public void Fire()
        {
            if (!isEnoughAmmoInClip)
                return;

            currentAmmoClip--;

            _muzzleFlesh.Play();

            FireRaycast();
            FireEvent.Invoke();
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
            currentAmmo = Math.Clamp(currentAmmo + ammoAmount, 0, ammo.ammoClip - currentAmmoClip + ammo.maxAmmo);
        }

        protected virtual void FireRaycast()
        {
            if (!Physics.Raycast(bulletSpawnPoint.position, transform.TransformDirection(Vector3.forward),
                    out var raycastHit, maxHitscanRange)) return;

            if (raycastHit.collider.gameObject.TryGetComponent<IDamageable>(out var damageable))
                damageable.TakeDamage(damage);
        }

        private void Reload()
        {
            if (isEnoughAmmo)
            {
                var previousAmmo = currentAmmo;
                currentAmmo = (uint)Math.Clamp((int)currentAmmo - (ammo.ammoClip - currentAmmoClip), 0, currentAmmo);
                currentAmmoClip = previousAmmo - currentAmmo + currentAmmoClip;
            }

            ReloadFinishedEvent?.Invoke();
        }
    }
}