using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Characters.Player
{
    public sealed class PlayerController : MonoBehaviour // TODO: Make based class for the all Entities.
    {
        public event Action<bool> AimEvent;
        public event Action<bool> FireEvent;
        public event Action<bool> ZoomEvent;
        public event Action<bool> RunEvent;

        [Header("Stats")] [SerializeField] private float walkSpeed = 20.0f;
        [SerializeField] private float runSpeed = 30.0f;
        [SerializeField] private float walkSpeedScoped = 10.0f;

        [Header("Preferences")]
        // TODO: Move to the struct aka `Config or something in this way. 
        [SerializeField]
        private float mouseSensitivity = 30.0f;

        private CharacterController _controller;
        private Camera _camera;

        public bool isAiming { get; private set; }

        public bool isRunning { get; private set; }
        public bool isFiring { get; private set; }
        public bool isMoving => _controller.velocity.z is > float.Epsilon or < -float.Epsilon;
        public bool isJumping { get; private set; }

        private Vector3 _moveDirection;
        private Vector2 _mouseDelta; // TODO: Remove it later.
        private float _rotation;
        private float _speed;

        public void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;

            _controller = GetComponent<CharacterController>();
            _camera = GetComponentInChildren<Camera>();

            _speed = walkSpeed;
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;

            _controller.Move(transform.TransformDirection(_moveDirection) * (_speed * deltaTime));

            _rotation -= _mouseDelta.y * Time.deltaTime * mouseSensitivity;
            _rotation = Mathf.Clamp(_rotation, -75.0f, 75.0f);
            _camera.transform.localRotation = Quaternion.Euler(_rotation, 0.0f, 0.0f);

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
            if (ctx.performed)
                FireEvent?.Invoke(ctx.performed); // TODO: Remove input parameter.
            if (ctx.canceled)
                FireEvent?.Invoke(false); // TODO: Remove input parameter.
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
    }
}