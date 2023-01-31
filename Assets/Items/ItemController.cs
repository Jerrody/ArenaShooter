using Game.Characters.Player;
using UnityEngine;

namespace Game.Items
{
    [RequireComponent(typeof(BoxCollider), typeof(MeshRenderer), typeof(MeshFilter))]
    public abstract class ItemController : MonoBehaviour
    {
        protected BoxCollider Collider;
        protected Transform ParentTransform;

        private void Update()
        {
            ParentTransform.Rotate(0.0f, 25.0f * Time.deltaTime, 0.0f, Space.Self);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != PlayerController.layerMask) return;

            var gm = gameObject;
            gm.GetComponent<MeshRenderer>().enabled = false;
            Collider.gameObject.SetActive(false);
            Destroy(gm, 5.0f);
        }
    }
}