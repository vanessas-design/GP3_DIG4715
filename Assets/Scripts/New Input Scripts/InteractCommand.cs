using Assets.Scripts;
using UnityEngine;

namespace Assets.Scripts
{
    public class InteractCommand : Command
    {
        private Transform _transform;
        private IInteractable _interactedWith;

        private const string LayerName = "Interactable";

        private void Awake()
        {
            _transform = transform;
        }

        public override void Execute()
        {
            Physics.Raycast(_transform.position + Vector3.up,
                _transform.forward, out var hit, 2f,
                LayerMask.GetMask(LayerName));

            Debug.Log($"{gameObject.name} is trying to interact!");

            if (hit.collider == null) return;

            Debug.Log($"{gameObject.name} interacted with {hit.collider.name}");

            _interactedWith = hit.collider.GetComponent<IInteractable>();
            _interactedWith?.Interact();
        }
    }
}
