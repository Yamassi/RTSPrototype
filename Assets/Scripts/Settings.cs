using UnityEngine;
using UnityEngine.UI;

namespace Rts
{
    public class Settings : MonoBehaviour
    {
        [SerializeField] private Button _close;
        [SerializeField] private Button _open;
        [SerializeField] private Slider _soundSlider, _musicSlider;
        private AudioService _audioService;

        public void Initialize(AudioService audioService)
        {
            _audioService = audioService;
            
            _close.onClick.AddListener(()=>gameObject.SetActive(false));
            _open.onClick.AddListener(()=>gameObject.SetActive(true));

            _soundSlider.value = _audioService.GetSoundVolume();
            _musicSlider.value = _audioService.GetMusicVolume();
            
            _soundSlider.onValueChanged.AddListener((value)=>_audioService.SetSoundVolume(value));
            _musicSlider.onValueChanged.AddListener((value)=>_audioService.SetMusicVolume(value));
        }
    }
}