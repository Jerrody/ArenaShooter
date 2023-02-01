using Game.Characters.Player;
using UnityEngine;

namespace Game.Items
{
    [RequireComponent(typeof(BoxCollider), typeof(MeshRenderer), typeof(MeshFilter))]
    public abstract class ItemController : MonoBehaviour
    {
        [Header("Stats")] [SerializeField] private float resetTime = 5.0f;

        protected BoxCollider Collider;

        private Transform _parentTransform;
        private MeshRenderer _meshRenderer;

        private float _nextTimeToActivate;

        private void Awake()
        {
            Collider = GetComponent<BoxCollider>();
            Collider.isTrigger = true;

            _parentTransform = GetComponentInParent<Transform>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            var isMeshRendererEnabled = _meshRenderer.enabled;
            if (_nextTimeToActivate < Time.time && !isMeshRendererEnabled)
            {
                _meshRenderer.enabled = true;
                Collider.enabled = true;
            }

            if (!isMeshRendererEnabled) return;

            _parentTransform.Rotate(0.0f, 25.0f * Time.deltaTime, 0.0f, Space.Self);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != PlayerController.layerMask) return;

            _meshRenderer.enabled = false;
            Collider.enabled = false;
            _nextTimeToActivate = Time.time + resetTime;
        }
    }
}