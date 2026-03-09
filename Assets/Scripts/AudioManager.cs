using UnityEngine;

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
            ApplyVolume();
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
        }


        // ── Helpers ───────────────────────────────────────────

        private void ApplyVolume()
        {
            musicSource.volume = MusicVolume;
            sfxSource.volume = SFXVolume;
        }

        // Shortcut methods để gọi nhanh từ chỗ khác
        public void PlayCoin()      => PlaySFX(CoinSFX);
        public void PlayExplosion() => PlaySFX(ExplosionSFX);
        public void PlayEffectPickup() => PlaySFX(EffectPickupSFX);
        public void PlayMenuMusic() => PlayMusic(MenuMusic);
        public void PlayGameMusic() => PlayMusic(GameMusic);
        public void PlayButtonClick() => PlaySFX(ButtonClickSFX);
    }
}
