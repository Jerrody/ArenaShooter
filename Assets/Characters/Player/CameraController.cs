using Game.Utils;
using UnityEngine;

namespace Game.Characters.Player
{
    public sealed class CameraController : MonoBehaviour
    {
        [Header("Field of View Stats")] [SerializeField]
        private float fieldOfViewScoped = 45.0f;

        [SerializeField] private float fieldOfView = 75.0f;
        [SerializeField] private float scopeSpeed = 5.0f;

        [Header("Zoom Stats")] [SerializeField]
        private float zoomInFieldOfView = 35.0f;

        [SerializeField] private float zoomOutFieldOfView = 45.0f;
        [SerializeField] private float zoomTime = 3.5f;
        [SerializeField] private float zoomResetTime = 2.0f;

        private Camera _camera;

        private Timer _zoomTimer;

        private float _targetFieldOfView;
        private bool _canZoom = true;
        private bool _isAiming;

        private void Awake()
        {
            var playerController = GetComponentInParent<PlayerController>();
            playerController.AimEvent += OnAim;
            playerController.ZoomEvent += OnZoom;

            _camera = GetComponent<Camera>();
            _camera.fieldOfView = fieldOfView;
            _targetFieldOfView = fieldOfView;
        }

        private void Update()
        {
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _targetFieldOfView, Time.deltaTime * scopeSpeed);
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
                isZooming ? zoomInFieldOfView : zoomOutFieldOfView;

            if (_zoomTimer == null)
            {
                _zoomTimer = Timer.Create(ZoomOut, zoomTime);
            }
            else
            {
                _zoomTimer.ResetTime();
            }
        }

        private void ZoomOut()
        {
            _zoomTimer = null;
            _canZoom = !_canZoom;

            if (_isAiming)
                _targetFieldOfView = zoomOutFieldOfView;

            Timer.Create(() => _canZoom = !_canZoom, zoomResetTime);
        }
    }
}