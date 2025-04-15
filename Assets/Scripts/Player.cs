using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Rts
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _navMesh;
        [SerializeField] private Animator _animator;
        private InputSystem_Actions _input;
        
        private const string GroundMask = "Ground";
        private const string Speed = "Speed";

        private void Awake()
        {
            _input = new InputSystem_Actions();
            _input.Enable();

            _input.Rts.Click.started += Select;
        }

        private void OnDestroy()
        {
            _input.Rts.Click.started -= Select;
        }

        private void Update()
        {
            _animator.SetFloat(Speed, _navMesh.velocity.magnitude);
        }

        private void Select(InputAction.CallbackContext context)
        {
            var hitPosition = _input.Rts.Position.ReadValue<Vector2>();
            Ray ray = Camera.main.ScreenPointToRay(hitPosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerMask.GetMask(GroundMask)))
            {
                Vector3 direction = hit.point;
                Debug.Log($"Direction: {direction}");
                MoveToDirection(direction);
            }
        }

        private void MoveToDirection(Vector3 direction)
        {
            _navMesh.SetDestination(direction);
        }
    }
}