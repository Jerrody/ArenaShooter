# Welcome

This is a game made in 37 hours!
Prebuild game version for Windows can be found [here](https://drive.google.com/drive/folders/1pMYLDrPvMPC-EO5E-T0ETQBeSHiKHsW7?usp=sharing).

## Unity Version

Currently used version 2022.2.3f1

### NOTE

In Editor mode could appear unexpected internal engine warnings that cannot be affected, like:

```cs
[Worker0] Internal: There are remaining Allocations on the JobTempAlloc. This is a leak, and will impact performance
```

```cs
[Worker0] To Debug, run app with -diag-job-temp-memory-leak-validation cmd line argument. This will output the callstacks of the leaked allocations.
```

## Features

- Data Storage
- Weapon System
- Health System
- Enemies
- Attachment system (more like hardcoded than flexible)
- Progress System
- Particle System for the weapons (not so much)
- Wave Gamemode

## Controls

- WASD - movement
- Space - Jump
- Left Click - Shoot
- Right Click - Aim
- R - Reload
- Shift - Run and Zoom (in Aim mode)
- F1/Escape - Menu

## Code Style

Access order in code:

```cs
    public float x;
    protected  bool Y;
    private uint _z;
```

```cs
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
    public sealed class FooController : MonoBehaviour
    {
        public const string Name = "Foo";

        public static LayerMask layerMask { get; private set; }

        public event Action EscapePressedEvent;

        public Action<bool> AimEvent;

        [Header("Stats")]
        [SerializeField] private AnimationCurve jumpFallOff;
        [SerializeField] private float runSpeed = 30.0f;

        [Header("Preferences")] [SerializeField]
        private float mouseSensitivity = 15.0f;

        public WeaponHolderController weaponHolderController { get; private set; }

        private CharacterController _controller;
        private PlayerCameraController _cameraController;

        public bool isMoving => _controller.velocity.z is > float.Epsilon or < -float.Epsilon;

        public bool isAiming { get; private set; }
        private bool isRunning { get; set; }
        public float rotation { get; private set; }
        public Vector2 mouseDelta { get; private set; }

        private Transform _transform;
        private Vector3 _moveDirection;
        private float _speed;
        private bool _isJumping;

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
            isRunning = (ctx.started || ctx.performed) && !weaponHolderController.isFiring;

            switch (isRunning)
            {
                case true when isAiming:
                    ZoomEvent?.Invoke(!_isJumping && isRunning);
                    RunEvent?.Invoke(false);
                    break;
                case true when !isAiming:
                    _speed = runSpeed;
                    RunEvent?.Invoke(true);
                    break;
                case false when isAiming:
                    ZoomEvent?.Invoke(!_isJumping && isRunning);
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

        private static void OnDeath()
        {
            Data.AddDeath();
        }
    }
}
```
