using System;
using Game.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Characters.Player
{
    [RequireComponent(typeof(PlayerInput), typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour // TODO: Make based class for the all Entities.
    {
        public Action<bool> AimEvent;

        public event Action<bool> FireEvent;
        public event Action ReloadEvent;
        public event Action<bool> ZoomEvent;
        public event Action<bool> RunEvent;
        public event Action<uint> WeaponSwitchEvent;

        [Header("Stats")] [SerializeField] private float walkSpeed = 20.0f;
        [SerializeField] private float runSpeed = 30.0f;
        [SerializeField] private float walkSpeedScoped = 10.0f;

        [Header("Preferences")]
        // TODO: Move to the struct aka `Config or something in this way. 
        [SerializeField]
        private float mouseSensitivity = 30.0f;

        private CharacterController _controller;
        private PlayerCameraController _cameraController;
        private WeaponHolderController _weaponHolderController;

        public bool isMoving => _controller.velocity.z is > float.Epsilon or < -float.Epsilon;

        public bool isAiming { get; private set; }
        private bool isRunning { get; set; }
        public bool isJumping { get; private set; }
        public float rotation { get; private set; }

        private Vector3 _moveDirection;
        private Vector2 _mouseDelta; // TODO: Remove it later.
        private float _speed;

        public void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;

            _controller = GetComponent<CharacterController>();
            _cameraController = GetComponentInChildren<PlayerCameraController>();
            _weaponHolderController = GetComponentInChildren<WeaponHolderController>();

            _speed = walkSpeed;
        }

        private void Start()
        {
            SetCameraFieldOfViewAndZoom();
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;

            _controller.Move(transform.TransformDirection(_moveDirection) * (_speed * deltaTime));

            rotation -= _mouseDelta.y * Time.deltaTime * mouseSensitivity;
            rotation = Mathf.Clamp(rotation, -75.0f, 75.0f);

            transform.Rotate(Vector3.up * (_mouseDelta.x * deltaTime * mouseSensitivity));
        }

        private void LateUpdate()
        {
            transform.Rotate(Vector3.up * (_mouseDelta.x * Time.deltaTime * mouseSensitivity));
        }

        public void Move(InputAction.CallbackContext ctx)
        {
            var direction = ctx.ReadValue<Vector2>();
            _moveDirection.x = direction.x;
            _moveDirection.z = direction.y;
        }

        public void Rotate(InputAction.CallbackContext ctx)
        {
            _mouseDelta = ctx.ReadValue<Vector2>();
        }

        public void Aim(InputAction.CallbackContext ctx)
        {
            isAiming = ctx.started || ctx.performed;

            AimEvent?.Invoke(isAiming);

            _speed = isAiming ? walkSpeedScoped : walkSpeed;
        }

        public void Fire(InputAction.CallbackContext ctx)
        {
            FireEvent?.Invoke(ctx.performed && !ctx.canceled);
        }

        public void Run(InputAction.CallbackContext ctx)
        {
            isRunning = ctx.started || ctx.performed;

            switch (isRunning)
            {
                case true when isAiming:
                    ZoomEvent?.Invoke(isRunning);
                    RunEvent?.Invoke(false);
                    break;
                case true when !isAiming:
                    _speed = runSpeed;
                    RunEvent?.Invoke(true);
                    break;
                case false when isAiming:
                    ZoomEvent?.Invoke(isRunning);
                    break;
                case false:
                    _speed = walkSpeed;
                    RunEvent?.Invoke(false);
                    break;
            }
        }

        public void Reload(InputAction.CallbackContext ctx)
        {
            if (!ctx.started)
                return;

            ReloadEvent?.Invoke();
            isAiming = false;
            AimEvent?.Invoke(isAiming);
        }

        public void SwitchWeapon(InputAction.CallbackContext ctx)
        {
            if (!ctx.started) return;

            var weaponIndex = uint.Parse(ctx.control.name) - 1;

            WeaponSwitchEvent?.Invoke(weaponIndex);
            SetCameraFieldOfViewAndZoom();
        }

        private void SetCameraFieldOfViewAndZoom()
        {
            _cameraController.SetFieldOfViewScoped(_weaponHolderController.fieldOfViewScoped);
            _cameraController.SetZoomInFieldOfView(_weaponHolderController.zoomInFieldOfView);
        }
    }
}