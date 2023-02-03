using System;
using Game.Characters.Player;
using Game.Gamemode.Wave;
using Game.Global.Data;
using Game.Items.Weapons;
using Game.Weapons;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Game.UI.HUD
{
    public sealed class HudController : MonoBehaviour
    {
        private const string WinText = "WIN!";
        private const string LoseText = "You're Dead!";

        [Header("References")] [SerializeField]
        private TMP_Text[] weaponSlotsText = new TMP_Text[3];

        [SerializeField] private TMP_Text weaponAmmoText;
        [SerializeField] private TMP_Text endOfMathText;
        [SerializeField] private Image healthBar;
        [SerializeField] private GameObject escapeMenu;
        [SerializeField] private GameObject resumeButton;

        [Header("Stats")] [SerializeField] private float healthReduceSpeed = 2.0f;

        private PlayerController _playerController;
        private WeaponHolderController _weaponHolderController;
        private TMP_Text _currentActiveWeaponSlot;

        private float _currentHealth = 1.0f;

        private void Awake()
        {
            _playerController = GetComponentInParent<PlayerController>();
            _playerController.WeaponSwitchEvent += OnWeaponSwitch;
            WaveManager.WavesEndEvent += OnWin;
        }

        private void Start()
        {
            _playerController.health.OnHealthChange += OnHealthChange;
            _playerController.health.DeathEvent += OnDeath;

            _playerController.weaponHolderController.PickedWeaponEvent += OnPickedWeapon;
            _playerController.weaponHolderController.WeaponSwitchEvent += OnWeaponAction;
            _playerController.EscapePressedEvent += OnEscapePressed;

            foreach (var weapon in _playerController.weaponHolderController.weapons)
            {
                weapon.ReloadFinishedEvent += OnWeaponAction;
                weapon.FireEvent += OnWeaponAction;
            }

            OnWeaponAction();
            _currentActiveWeaponSlot =
                weaponSlotsText[(uint)_playerController.weaponHolderController.currentWeapon.type];

            _currentActiveWeaponSlot.fontStyle = FontStyles.Bold;
            var color = _currentActiveWeaponSlot.color;
            color.a = 1.0f;
            _currentActiveWeaponSlot.color = color;
        }

        private void Update()
        {
            healthBar.fillAmount =
                Mathf.MoveTowards(healthBar.fillAmount, _currentHealth, healthReduceSpeed * Time.deltaTime);
        }

        // public void Unpause()
        // {
        //     InputSystem.EnableDevice(Keyboard.current);
        //     Time.timeScale = 1.0f;
        //     Cursor.lockState = CursorLockMode.Locked;
        //     Cursor.visible = false;
        // }

        private void SetNewWeaponSlotIndex(uint weaponIndex)
        {
            _currentActiveWeaponSlot.fontStyle = FontStyles.Normal;

            _currentActiveWeaponSlot = weaponSlotsText[weaponIndex];
            if (Math.Abs(_currentActiveWeaponSlot.color.a - 1.0f) > float.Epsilon)
                SetWeaponSlotSetAlphaToOne();

            _currentActiveWeaponSlot.fontStyle = FontStyles.Bold;
        }

        private void SetWeaponSlotSetAlphaToOne()
        {
            var color = _currentActiveWeaponSlot.color;
            color.a = 1.0f;
            _currentActiveWeaponSlot.color = color;
        }

        private void ActivateEndOfMatchTextWithMenu()
        {
            escapeMenu.gameObject.SetActive(true);
            endOfMathText.gameObject.SetActive(true);
        }

        private void OnWeaponSwitch(uint weaponIndex)
        {
            if (!_playerController.weaponHolderController.IsPickedWeaponByIndex(weaponIndex))
                return;

            SetNewWeaponSlotIndex(weaponIndex);
        }

        private void OnPickedWeapon(WeaponItemController weaponItemController)
        {
            SetNewWeaponSlotIndex((uint)weaponItemController.weaponType);

            OnWeaponAction();
        }

        private void OnWeaponAction()
        {
            var weaponController = _playerController.weaponHolderController.currentWeapon;
            weaponAmmoText.text = $"{weaponController.currentAmmoClip}/{weaponController.currentAmmo}";
        }

        private void OnEscapePressed()
        {
            escapeMenu.gameObject.SetActive(!escapeMenu.gameObject.activeSelf);
        }

        private void OnHealthChange(float maxHealth, float currentHeath)
        {
            _currentHealth = currentHeath / maxHealth;
        }

        private void OnDeath()
        {
            ActivateEndOfMatchTextWithMenu();
            endOfMathText.text = LoseText;
            Data.AddLose();
        }

        private void OnWin()
        {
            ActivateEndOfMatchTextWithMenu();
            endOfMathText.text = WinText;
            Data.AddWin();
        }
    }
}