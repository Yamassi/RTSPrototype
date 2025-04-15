using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Rts
{
    public class InputController : IDisposable
    {
        private InputSystem_Actions _input;
        private Player _player;
        private Camera _camera;
        private Vector2 _screenMoveInput;

        private const int MaxHitDistance = 100;
        private const float ScreenMoveSpeed = 10f;

        private CompositeDisposable _disposable = new CompositeDisposable();

        public InputController(Player player)
        {
            _player = player;
            _input = new InputSystem_Actions();
        }

        public void Initialize()
        {
            _input.Enable();

            _input.Rts.Click.performed += Select;
            _input.Rts.Move.performed += MoveScreen;
            _input.Rts.Move.canceled += CancelMoveScreen;
            _camera = Camera.main;

            MoveScreen();
        }

        public void Dispose()
        {
            _input.Rts.Click.performed -= Select;
            _input.Rts.Move.performed -= MoveScreen;
            _input.Rts.Move.canceled -= CancelMoveScreen;
            _input.Dispose();

            _disposable.Dispose();
        }

        private void CancelMoveScreen(InputAction.CallbackContext context) => _screenMoveInput = Vector2.zero;

        private void MoveScreen()
        {
            var touchSpeedMultiplier = 2f;
            var reverseInputMultiplier = -1f;
            Observable.EveryUpdate().Subscribe(_ =>
            {
                if (IsPointerOverUI())
                    return;

                Vector2 input = _screenMoveInput;
                float moveSpeed = ScreenMoveSpeed;

#if UNITY_ANDROID || UNITY_IOS
                if (Touchscreen.current != null)
                {
                    input *= reverseInputMultiplier;
                    moveSpeed *= touchSpeedMultiplier;
                }
#endif

                Vector3 right = _camera.transform.right;
                Vector3 forward = Vector3.Cross(right, Vector3.up);

                Vector3 move = (right * input.x + forward * input.y).normalized;
                move *= moveSpeed * Time.deltaTime;

                _camera.transform.position += move;
            }).AddTo(_disposable);
        }

        private void MoveScreen(InputAction.CallbackContext context) =>
            _screenMoveInput = context.ReadValue<Vector2>();

        private async void Select(InputAction.CallbackContext context)
        {
            if (Input.touchCount > 1)
                return;

            await UniTask.WaitForEndOfFrame();

            if (IsPointerOverUI())
            {
                Debug.Log("IsPointerOverGameObject.");
                return;
            }

            var hitPosition = _input.Rts.Position.ReadValue<Vector2>();

            if (hitPosition == Vector2.zero)
                return;

            Ray ray = Camera.main.ScreenPointToRay(hitPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, MaxHitDistance))
            {
                Vector3 direction = hit.point;

                if (hit.collider.gameObject.TryGetComponent(out Building building))
                {
                    direction = building.PlayerGetResourcePoint;
                }

                _player.MoveToDirection(direction);
            }
        }

        bool IsPointerOverUI()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return EventSystem.current.IsPointerOverGameObject();
#elif UNITY_ANDROID || UNITY_IOS
            if (Input.touchCount > 0)
                return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
            else
                return false;
#endif
        }
    }
}