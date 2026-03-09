using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace ZyroX
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        public TextMeshProUGUI DistanceText;
        public HomeUI HomeUI;
        public InGameUI InGameUI;
        public Shop ShopUI;
        public GameOverUI GameOverUI;
        public GameObject FlashEffect;

        [SerializeField] private float flashFadeInDuration = 0.3f;
        [SerializeField] private float flashHoldDuration = 0.1f;
        [SerializeField] private float flashFadeOutDuration = 0.3f;

        private Image flashImage;
        private Coroutine flashCoroutine;

        public UIPopUpBase CurrentPopUp { get; private set; }
        public UIPopUpBase PreviousPopUp { get; private set; }


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            InGameUI.gameObject.SetActive(false);
            ShopUI.gameObject.SetActive(false);
            GameOverUI.gameObject.SetActive(false);

            if (FlashEffect != null)
            {
                flashImage = FlashEffect.GetComponent<Image>();
                if (flashImage == null)
                    flashImage = FlashEffect.AddComponent<Image>();
                SetFlashAlpha(0f);
                FlashEffect.SetActive(false);
            }
        }

        void Start()
        {
            ShowPopUp(HomeUI);
        }

        void Update()
        {

        }

        public void PlayGame()
        {
            ShowPopUp(InGameUI);
        }

        public void ShowPopUp(UIPopUpBase popUp)
        {
            if (CurrentPopUp != null)
            {
                CurrentPopUp.Hide();
                PreviousPopUp = CurrentPopUp;
            }
            CurrentPopUp = popUp;
            CurrentPopUp.Show();
        }

        /// <summary>
        /// Flash trắng/đen → gọi onMidFlash giữa chừng (lúc màn đen/trắng) → fade out
        /// </summary>
        public void FlashAndLoad(Action onMidFlash = null)
        {
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashRoutine(onMidFlash));
        }

        private IEnumerator FlashRoutine(Action onMidFlash)
        {
            if (flashImage == null) yield break;

            FlashEffect.SetActive(true);

            // Fade in (0 → 1)
            float elapsed = 0f;
            while (elapsed < flashFadeInDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                SetFlashAlpha(Mathf.Clamp01(elapsed / flashFadeInDuration));
                yield return null;
            }
            SetFlashAlpha(1f);

            // Gọi logic load/chuyển màn giữa flash
            onMidFlash?.Invoke();

            // Hold
            yield return new WaitForSecondsRealtime(flashHoldDuration);

            // Fade out (1 → 0)
            elapsed = 0f;
            while (elapsed < flashFadeOutDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                SetFlashAlpha(Mathf.Clamp01(1f - elapsed / flashFadeOutDuration));
                yield return null;
            }
            SetFlashAlpha(0f);
            FlashEffect.SetActive(false);
        }

        private void SetFlashAlpha(float alpha)
        {
            Color c = flashImage.color;
            c.a = alpha;
            flashImage.color = c;
        }
    }
}

