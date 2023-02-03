using System;
using System.Collections;
using Game.Characters.Components;
using Game.Global.Data;
using Game.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Characters.Player
{
    [RequireComponent(typeof(PlayerInput), typeof(CharacterController))]
    public sealed class PlayerController : EntityController
    {
        public static LayerMask layerMask { get; private set; }

        public event Action<bool> FireEvent;
        public event Action ReloadEvent;
        public event Action<bool> ZoomEvent;
        public event Action<bool> RunEvent;
        public event Action<uint> WeaponSwitchEvent;
        public event Action EscapePressedEvent;

        public Action<bool> AimEvent;

        [Header("Stats")]
        [SerializeField] private AnimationCurve jumpFallOff;
        [SerializeField] private float runSpeed = 30.0f;
        [SerializeField] private float walkSpeedScoped = 10.0f;
        [SerializeField] private float jumpMultiplier = 2.0f;

        [Header("Preferences")]
        [SerializeField] private float mouseSensitivity = 15.0f;

        public bool isMoving => _controller.velocity.z is > float.Epsilon or < -float.Epsilon;

        public WeaponHolderController weaponHolderController { get; private set; }
        public bool isAiming { get; private set; }
        public float rotation { get; private set; }
        public Vector2 mouseDelta { get; private set; }

        private CharacterController _controller;
        private PlayerCameraController _cameraController;
        private Transform _transform;

        private Vector3 _moveDirection;
        private float _speed;
        private bool _isJumping;
        private bool _isRunning;

        public void Awake()
        {
            layerMask = LayerMask.NameToLayer("Player");

            InputSystem.EnableDevice(Keyboard.current);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _controller = GetComponent<CharacterController>();
            _cameraController = GetComponentInChildren<PlayerCameraController>();
            weaponHolderController = GetComponentInChildren<WeaponHolderController>();

            health = GetComponent<HealthComponent>();
            health.DeathEvent += OnDeath;

            _speed = walkSpeed;
        }

        private void Start()
        {
            SetCameraFieldOfViewAndZoom();
        }

        private void Update()
        {
            _transform = transform;
            var forwardMovement = _transform.forward * _moveDirection.z;
            var rightMovement = _transform.right * _moveDirection.x;
            _controller.SimpleMove(forwardMovement + rightMovement);

            var deltaTime = Time.deltaTime;
            rotation -= mouseDelta.y * deltaTime * mouseSensitivity;
            rotation = Mathf.Clamp(rotation, -75.0f, 75.0f);

            transform.Rotate(Vector3.up * (mouseDelta.x * deltaTime * mouseSensitivity));
        }

        private void LateUpdate()
        {
            transform.Rotate(Vector3.up * (mouseDelta.x * Time.deltaTime * mouseSensitivity));
        }

        public void Move(InputAction.CallbackContext ctx)
        {
            var direction = ctx.ReadValue<Vector2>();
            _moveDirection.x = direction.x * _speed;
            _moveDirection.z = direction.y * _speed;
        }

        public void Jump(InputAction.CallbackContext ctx)
        {
            if (!ctx.started && _isJumping) return;

            _isJumping = true;
            StartCoroutine(JumpEvent());
        }

        public void Rotate(InputAction.CallbackContext ctx)
        {
            mouseDelta = ctx.ReadValue<Vector2>();
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
            _isRunning = (ctx.started || ctx.performed) && !weaponHolderController.isFiring;

            switch (_isRunning)
            {
                case true when isAiming:
                    ZoomEvent?.Invoke(!_isJumping && _isRunning);
                    RunEvent?.Invoke(false);
                    break;
                case true when !isAiming:
                    _speed = runSpeed;
                    RunEvent?.Invoke(true);
                    break;
                case false when isAiming:
                    ZoomEvent?.Invoke(!_isJumping && _isRunning);
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

        public void Escape(InputAction.CallbackContext ctx)
        {
            if (!ctx.started) return;

            EscapePressedEvent?.Invoke();
        }

        private static void OnDeath()
        {
            Data.AddDeath();
        }

        private void SetCameraFieldOfViewAndZoom()
        {
            _cameraController.SetFieldOfViewScoped(weaponHolderController.fieldOfViewScoped);
            _cameraController.SetZoomInFieldOfView(weaponHolderController.zoomInFieldOfView);
        }

        private IEnumerator JumpEvent()
        {
            _controller.slopeLimit = 90.0f;
            var timeInAir = 0.0f;

            do
            {
                var jumpForce = jumpFallOff.Evaluate(timeInAir);
                _controller.Move(Vector3.up * (jumpForce * jumpMultiplier * Time.deltaTime));
                timeInAir += Time.deltaTime;
                yield return null;
            } while (!_controller.isGrounded && _controller.collisionFlags != CollisionFlags.Above);

            _controller.slopeLimit = 45.0f;
            _isJumping = false;
        }
    }
}