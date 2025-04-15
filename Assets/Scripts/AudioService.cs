using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

namespace Rts
{
    public class AudioService : MonoBehaviour
    {
        [SerializeField] protected AudioMixer _audioMixer;
        
        private const string Sound = "Sound";
        private const string Music = "Music";
        
        public async void Initialize()
        {
            await UniTask.Delay(100);
            LoadVolumes();
        }

        public void LoadVolumes()
        {
            SetSoundVolume(PlayerPrefs.GetFloat(Sound));
            SetMusicVolume(PlayerPrefs.GetFloat(Music));
        }

        public void SetSoundVolume(float volume)
        {
            _audioMixer.SetFloat("Sound", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
            PlayerPrefs.SetFloat(Sound, volume);
        }

        public void SetMusicVolume(float volume)
        {
            _audioMixer.SetFloat("Music", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
            PlayerPrefs.SetFloat(Music, volume);
        }
        
        public float GetSoundVolume() => PlayerPrefs.GetFloat(Sound);
        public float GetMusicVolume() => PlayerPrefs.GetFloat(Music);
    }
}