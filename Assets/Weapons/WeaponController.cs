using System;
using UnityEngine;

namespace Game.Weapons
{
    [Serializable]
    internal struct Ammo
    {
        public float MaxAmmo;
        public float AmmoClip;
    }

    public class WeaponController : MonoBehaviour
    {
        [Header("Stats")] [SerializeField] protected float fireRate;
        [SerializeField] private Ammo _ammo;

        private float _nextTimeToFire;

        public bool CanShoot()
        {
            if (Time.time < _nextTimeToFire)
                return false;

            _nextTimeToFire = Time.time + 1.0f / fireRate;

            return true;
        }
    }
}