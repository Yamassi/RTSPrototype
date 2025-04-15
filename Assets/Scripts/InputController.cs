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
        private const float HoldThreshold = 0.2f;

        private CompositeDisposable _disposable = new CompositeDisposable();
        private float _minX = -3;
        private float _maxX = 3;
        private float _minZ = -12;
        private float _maxZ = -1;
        private float _clickStartTime;
        private bool _isHold;


        public InputController(Player player)
        {
            _player = player;
            _input = new InputSystem_Actions();
        }

        public void Initialize()
        {
            _input.Enable();

            _input.Rts.Click.started += StartSelect;
            _input.Rts.Click.canceled += EndSelect;
            _input.Rts.Move.performed += MoveScreen;
            _input.Rts.Move.canceled += CancelMoveScreen;

            _camera = Camera.main;

            _isHold = false;

            MoveScreen();
        }

        public void Dispose()
        {
            _input.Rts.Click.started -= StartSelect;
            _input.Rts.Click.canceled -= EndSelect;
            _input.Rts.Move.performed -= MoveScreen;
            _input.Rts.Move.canceled -= CancelMoveScreen;
            _input.Dispose();

            _disposable.Dispose();
        }

        private void MoveScreen()
        {
            var touchSpeedMultiplier = 0.5f;
            var reverseInputMultiplier = -1f;
            Observable.EveryUpdate().Subscribe(_ =>
            {
                if (IsPointerOverUI())
                    return;

                if (!_isHold)
                    return;

                Vector2 input = _screenMoveInput;
                float moveSpeed = ScreenMoveSpeed;

                input *= reverseInputMultiplier;

                Vector3 right = _camera.transform.right;
                Vector3 forward = Vector3.Cross(right, Vector3.up);

                Vector3 move = (right * input.x + forward * input.y).normalized * moveSpeed * Time.deltaTime;
                Vector3 newPosition = _camera.transform.position + move;

                Vector3 isoPos = new Vector3(
                    Vector3.Dot(newPosition, right),
                    Vector3.Dot(newPosition, forward)
                );

                isoPos.x = Mathf.Clamp(isoPos.x, _minX, _maxX);
                isoPos.y = Mathf.Clamp(isoPos.y, _minZ, _maxZ);

                newPosition = right * isoPos.x + forward * isoPos.y + Vector3.up * newPosition.y;

                _camera.transform.position = newPosition;
            }).AddTo(_disposable);
        }

        private void CancelMoveScreen(InputAction.CallbackContext context) =>
            _screenMoveInput = Vector2.zero;

        private void MoveScreen(InputAction.CallbackContext context) =>
            _screenMoveInput = context.ReadValue<Vector2>();

        private void StartSelect(InputAction.CallbackContext context)
        {
            _isHold = true;
            _clickStartTime = Time.time;
        }

        private async void EndSelect(InputAction.CallbackContext context)
        {
            _isHold = false;
            float holdTime = Time.time - _clickStartTime;
            if (holdTime > HoldThreshold)
                return;

            await UniTask.WaitForEndOfFrame();

            if (IsPointerOverUI())
                return;

            Vector2 hitPosition;
            hitPosition = _input.Rts.Position.ReadValue<Vector2>();

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
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}