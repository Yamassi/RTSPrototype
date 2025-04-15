using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Rts
{
    public class PopUp : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _resourceValue;
        [SerializeField] private CanvasGroup _canvasGroup;
        private const float FadeDuration = 0.3f;
        private const float DisplayDuration = 0.8f;
        private Sequence _sequence;

        private void Awake()
        {
            Reset();
        }

        private void OnDisable()
        {
            _sequence.Kill();
            Reset();
        }

        public void Show(string resource, string value)
        {
            _resourceValue.text = $"{resource} - {value}";
            _sequence?.Kill();
            _sequence = DOTween.Sequence()
                .Append(_canvasGroup.DOFade(1, FadeDuration))
                .AppendInterval(DisplayDuration)
                .Append(_canvasGroup.DOFade(0, FadeDuration));
        }

        private void Reset() => _canvasGroup.alpha = 0;
    }
}