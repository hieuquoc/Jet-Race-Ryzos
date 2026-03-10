using UnityEngine;
using UnityEngine.UI;

namespace ZyroX
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [Header("Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Clips - Music")]
        public AudioClip MenuMusic;
        public AudioClip GameMusic;

        [Header("Clips - SFX")]
        public AudioClip CoinSFX;
        public AudioClip ExplosionSFX;
        public AudioClip EffectPickupSFX;
        public AudioClip ButtonClickSFX;

        public Toggle SoundToggle;

        private const string PlayerPrefsSoundKey = "SoundEnabled_v1";
        private bool soundEnabled = true;
        private bool soundToggleListenerAdded = false;

        [Header("Settings")]
        [Range(0f, 1f)] public float MusicVolume = 1f;
        [Range(0f, 1f)] public float SFXVolume = 1f;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            musicSource.loop = true;
            // load saved sound enabled state
            soundEnabled = PlayerPrefs.GetInt(PlayerPrefsSoundKey, 1) == 1;
            if (SoundToggle != null)
            {
                // Toggle ON means muted, Toggle OFF means sound on
                SoundToggle.isOn = !soundEnabled;
                SoundToggle.onValueChanged.AddListener(OnSoundToggleChanged);
                soundToggleListenerAdded = true;
            }
            ApplyVolume();
            musicSource.mute = !soundEnabled;
            sfxSource.mute = !soundEnabled;
        }

        void OnDestroy()
        {
            if (soundToggleListenerAdded && SoundToggle != null)
            {
                SoundToggle.onValueChanged.RemoveListener(OnSoundToggleChanged);
            }
        }

        // ── Music ─────────────────────────────────────────────

        public void PlayMusic(AudioClip clip)
        {
            if (clip == null || musicSource.clip == clip) return;
            musicSource.clip = clip;
            musicSource.Play();
        }

        public void StopMusic() => musicSource.Stop();

        public void PauseMusic() => musicSource.Pause();

        public void ResumeMusic() => musicSource.UnPause();

        public void SetMusicVolume(float volume)
        {
            MusicVolume = Mathf.Clamp01(volume);
            musicSource.volume = MusicVolume;
            ApplyVolume();
        }

        // ── SFX ───────────────────────────────────────────────

        public void PlaySFX(AudioClip clip)
        {
            if (clip == null) return;
            sfxSource.PlayOneShot(clip, SFXVolume);
        }

        public void SetSFXVolume(float volume)
        {
            SFXVolume = Mathf.Clamp01(volume);
            sfxSource.volume = SFXVolume;
            ApplyVolume();
        }


        // ── Helpers ───────────────────────────────────────────

        private void ApplyVolume()
        {
            musicSource.volume = MusicVolume;
            sfxSource.volume = SFXVolume;
        }

        private void OnSoundToggleChanged(bool isOn)
        {
            // UI Toggle ON means muted, OFF means sound on
            SetSoundEnabled(!isOn);
        }

        // Sound toggle API
        public void SetSoundEnabled(bool enabled)
        {
            soundEnabled = enabled;
            PlayerPrefs.SetInt(PlayerPrefsSoundKey, enabled ? 1 : 0);
            PlayerPrefs.Save();
            musicSource.mute = !enabled;
            sfxSource.mute = !enabled;
        }

        public void ToggleSound()
        {
            SetSoundEnabled(!soundEnabled);
            if (SoundToggle != null && SoundToggle.isOn != !soundEnabled)
                SoundToggle.isOn = !soundEnabled;
        }

        public bool IsSoundEnabled() => soundEnabled;

        // Shortcut methods để gọi nhanh từ chỗ khác
        public void PlayCoin()      => PlaySFX(CoinSFX);
        public void PlayExplosion() => PlaySFX(ExplosionSFX);
        public void PlayEffectPickup() => PlaySFX(EffectPickupSFX);
        public void PlayMenuMusic() => PlayMusic(MenuMusic);
        public void PlayGameMusic() => PlayMusic(GameMusic);
        public void PlayButtonClick() => PlaySFX(ButtonClickSFX);
    }
}
