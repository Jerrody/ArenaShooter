using Game.Utils;
using UnityEngine;

namespace Game.Characters.Player
{
    public sealed class PlayerCameraController : MonoBehaviour
    {
        [Header("Field of View Stats")]
        [SerializeField] private float fieldOfView = 75.0f;
        [SerializeField] private float fieldOfViewScoped = 45.0f;
        [SerializeField] public float zoomInFieldOfView = 35.0f;
        [SerializeField] private float scopeSpeed = 5.0f;

        [Header("Zoom Stats")]
        [SerializeField] private float zoomTime = 3.5f;
        [SerializeField] private float zoomResetTime = 2.0f;

        private PlayerController _playerController;
        private Camera _camera;
        private Timer _zoomTimer;

        private float _targetFieldOfView;
        private bool _canZoom = true;
        private bool _isAiming;

        private void Awake()
        {
            _playerController = GetComponentInParent<PlayerController>();
            _playerController.AimEvent += OnAim;
            _playerController.ZoomEvent += OnZoom;

            _camera = GetComponent<Camera>();
            _camera.fieldOfView = fieldOfView;
            _targetFieldOfView = fieldOfView;
        }

        private void Update()
        {
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _targetFieldOfView, Time.deltaTime * scopeSpeed);
            _camera.transform.localRotation = Quaternion.Euler(_playerController.rotation, 0.0f, 0.0f);
        }

        private void OnAim(bool isAiming)
        {
            _isAiming = isAiming;
            _targetFieldOfView = isAiming ? fieldOfViewScoped : fieldOfView;
        }

        private void OnZoom(bool isZooming)
        {
            if (!_canZoom) return;

            _targetFieldOfView =
                isZooming ? zoomInFieldOfView : fieldOfViewScoped;

            if (_zoomTimer == null)
            {
                _zoomTimer = Timer.Create(ZoomOut, zoomTime);
            }
            else
            {
                _zoomTimer.ResetTime();
            }
        }

        public void SetFieldOfViewScoped(float newFieldOfViewScoped)
        {
            fieldOfViewScoped = newFieldOfViewScoped;
        }

        public void SetZoomInFieldOfView(float newZoomInFieldOfView)
        {
            zoomInFieldOfView = newZoomInFieldOfView;
        }

        private void ZoomOut()
        {
            _zoomTimer = null;
            _canZoom = !_canZoom;

            if (_isAiming)
                _targetFieldOfView = fieldOfViewScoped;

            Timer.Create(() => _canZoom = !_canZoom, zoomResetTime);
        }
    }
}