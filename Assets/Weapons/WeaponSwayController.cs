using Game.Characters.Player;
using UnityEngine;

namespace Game.Weapons
{
    public sealed class WeaponSwayController : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private float smooth;
        [SerializeField] private float multiplier;

        private PlayerController _playerController;

        private void Awake()
        {
            _playerController = GetComponentInParent<PlayerController>();
        }

        private void Update()
        {
            var mouseDelta = _playerController.mouseDelta;
            var rotationX = Quaternion.AngleAxis(-mouseDelta.y * multiplier, Vector3.right);
            var rotationY = Quaternion.AngleAxis(mouseDelta.x * multiplier, Vector3.up);

            var currentTransform = transform;
            currentTransform.localRotation = Quaternion.Slerp(currentTransform.localRotation, rotationX * rotationY,
                smooth * Time.deltaTime);
        }
    }
}