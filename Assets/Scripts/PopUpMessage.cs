using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZyroX
{
    public class PopUpMessage : MonoBehaviour
    {
        public GameObject[] Messages;
        public TextMeshProUGUI BonusText;
        public TextMeshProUGUI EffectText;
        public Image EffectTimerFill;

        [SerializeField] private float showDuration = 0.2f;
        [SerializeField] private float hideDuration = 0.15f;
        [SerializeField] private float bounceScale = 1.15f;

        private Coroutine[] animCoroutines;

        void Awake()
        {
            animCoroutines = new Coroutine[Messages.Length];     
            HideAll();       
        }

        void Update()
        {
            if(Messages[MessageType.Effects.GetHashCode()].activeSelf)
            {
                EffectTimerFill.fillAmount = EffectController.Instance.GetCurrentEffectTimer();
                if(EffectTimerFill.fillAmount <= 0)
                {
                    HideMessage(MessageType.Effects);
                }
            }
        }

        public void ShowMessage(MessageType type)
        {
            gameObject.SetActive(true);
            int index = (int)type;
            if (index < 0 || index >= Messages.Length)
            {
                Debug.LogWarning("Invalid MessageType index: " + index);
                return;
            }

            if(type == MessageType.Effects)
            {
                switch (EffectController.Instance.CurrentEffect.Type)
                {
                    case EffectType.SpeedBoost:
                        EffectText.text = "SPEED BOOST!";
                        break;
                    case EffectType.Shield:
                        EffectText.text = "GOT SHIELD!";
                        break;
                    case EffectType.Slow:
                        EffectText.text = "IT'S SLOW TIME!";
                        break;
                }
            }

            if (animCoroutines[index] != null) StopCoroutine(animCoroutines[index]);
            animCoroutines[index] = StartCoroutine(AnimShow(Messages[index]));
        }

        public void HideMessage(MessageType type)
        {
            int index = (int)type;
            if (index < 0 || index >= Messages.Length) return;
            if (animCoroutines[index] != null) StopCoroutine(animCoroutines[index]);
            animCoroutines[index] = StartCoroutine(AnimHide(Messages[index]));
        }

        private IEnumerator AnimShow(GameObject msg)
        {
            msg.SetActive(true);
            Transform t = msg.transform;
            float elapsed = 0f;
            // scale 0 → bounceScale
            while (elapsed < showDuration * 0.6f)
            {
                elapsed += Time.unscaledDeltaTime;
                float s = Mathf.SmoothStep(0f, bounceScale, elapsed / (showDuration * 0.6f));
                t.localScale = Vector3.one * s;
                yield return null;
            }
            elapsed = 0f;
            // bounce back bounceScale → 1
            while (elapsed < showDuration * 0.4f)
            {
                elapsed += Time.unscaledDeltaTime;
                float s = Mathf.Lerp(bounceScale, 1f, elapsed / (showDuration * 0.4f));
                t.localScale = Vector3.one * s;
                yield return null;
            }
            t.localScale = Vector3.one;
        }

        private IEnumerator AnimHide(GameObject msg)
        {
            Transform t = msg.transform;
            float elapsed = 0f;
            while (elapsed < hideDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float s = Mathf.Lerp(1f, 0f, elapsed / hideDuration);
                t.localScale = Vector3.one * s;
                yield return null;
            }
            t.localScale = Vector3.one;
            msg.SetActive(false);
            gameObject.SetActive(false);
        }

        public void HideAll()
        {
            for (int i = 0; i < Messages.Length; i++)
            {
                if (animCoroutines[i] != null) StopCoroutine(animCoroutines[i]);
                Messages[i].SetActive(false);
            }
            gameObject.SetActive(false);
        }
    }

    public enum MessageType
    {
        LookOut,
        Bonus,
        Effects
    }

}

