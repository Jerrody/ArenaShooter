using System.Linq;
using Game.Characters.Player;
using UnityEngine;

namespace Game.Weapons
{
    public sealed class WeaponHolderController : MonoBehaviour
    {
        private WeaponController[] _weapons;
        private WeaponController _currentWeapon;

        private PlayerAnimationController _playerAnimationController;

        private bool _isFiring;

        private void Awake()
        {
            _weapons = GetComponentsInChildren<WeaponController>();
            _playerAnimationController = GetComponentInParent<PlayerAnimationController>();
            var playerController = GetComponentInParent<PlayerController>();
            playerController.FireEvent += OnFire;

            foreach (var weapon in _weapons)
            {
                weapon.gameObject.SetActive(false);
            }

            if ((_currentWeapon = _weapons.First()) != null)
            {
                _currentWeapon.gameObject.SetActive(true);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            _playerAnimationController.FireAnimationEvent?.Invoke(_isFiring && _currentWeapon.CanShoot());
        }

        private void OnFire(bool isFiring)
        {
            _isFiring = isFiring;
        }
    }
}