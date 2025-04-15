using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Rts
{
    [RequireComponent(typeof(BoxCollider))]
    public class Building : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _resourceValue;
        [SerializeField] private TextMeshProUGUI _resourceName;
        [SerializeField] private ResourceType _resourceType;
        [SerializeField] private float _miningTime;
        [SerializeField] private Transform _playerGetResourcePoint;
        private Action<ReceivedResource> _onGetResource;
        private Tween _tween;
        private int _amount;

        private float _time;
        public Vector3 PlayerGetResourcePoint => _playerGetResourcePoint.position;

        private void Awake()
        {
            GetComponent<BoxCollider>().isTrigger = true;
            _amount = 0;
            _time = 0;

            _resourceName.text = _resourceType.ToString();
            RefreshView();
        }

        private void OnDestroy()
        {
            _tween.Kill();
        }

        public void Initialize(Action<ReceivedResource> onGetResource)
        {
            _onGetResource = onGetResource;
        }

        private void Update()
        {
            _time += Time.deltaTime;

            if (_time >= _miningTime)
            {
                _time = 0;
                _amount++;
                RefreshViewWithAnimation();
            }
        }

        private void RefreshView() =>
            _resourceValue.text = _amount.ToString();

        private void RefreshViewWithAnimation()
        {
            _resourceValue.text = _amount.ToString();

            Vector3 punchScale = new Vector3(1.2f, 1.2f, 1.2f);
            float duration = 0.3f;

            _resourceValue.color = Color.green;
            _tween = _resourceValue.transform.DOPunchScale(punchScale, duration)
                .OnComplete(() => _resourceValue.color = Color.white);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player))
            {
                if (_amount > 0)
                {
                    _onGetResource?.Invoke(new ReceivedResource(_resourceType, _amount));
                    _amount = 0;
                    RefreshView();
                }
            }
        }
    }
}