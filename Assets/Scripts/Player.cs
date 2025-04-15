using System;
using UnityEngine;
using UnityEngine.AI;

namespace Rts
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _navMesh;
        [SerializeField] private Animator _animator;
        private const float RotationSpeed = 10f;

        private const string Speed = "Speed";

        private void Awake()
        {
            _navMesh.updateRotation = false;
        }

        private void Update()
        {
            _animator.SetFloat(Speed, _navMesh.velocity.magnitude);

            Rotate();
        }

        private void Rotate()
        {
            if (_navMesh.velocity.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_navMesh.velocity.normalized);
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
            }
        }

        public void MoveToDirection(Vector3 direction) =>
            _navMesh.SetDestination(direction);
    }
}