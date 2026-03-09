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
        [SerializeField] private float timeLife = 1.5f;

        private Coroutine[] animCoroutines;
        private EffectAnimation[] activeAnims;
        private int previousIndex = 0;

        void Awake()
        {
            animCoroutines = new Coroutine[Messages.Length];
            activeAnims = new EffectAnimation[Messages.Length];
            HideAll();       
        }

        void Update()
        {
            for (int i = 0; i < activeAnims.Length; i++)
            {
                if (!activeAnims[i].IsAnimtionShowing) continue;

                if (i == (int)MessageType.Bonus)
                {
                    // Bonus: đếm ngược duration rồi tắt
                    var anim = activeAnims[i];
                    anim.Duration -= Time.deltaTime;
                    activeAnims[i] = anim;
                    if (activeAnims[i].Duration <= 0f)
                    {
                        var a = activeAnims[i];
                        a.IsAnimtionShowing = false;
                        activeAnims[i] = a;
                        HideMessage(MessageType.Bonus);
                    }
                }
                else if (i == (int)MessageType.Effects)
                {
                    // Effects: tắt khi fillAmount hết
                    EffectTimerFill.fillAmount = EffectController.Instance.GetCurrentEffectTimer();
                    if (EffectTimerFill.fillAmount <= 0f)
                    {
                        var a = activeAnims[i];
                        a.IsAnimtionShowing = false;
                        activeAnims[i] = a;
                        HideMessage(MessageType.Effects);
                    }
                }
            }
        }

        public void ShowMessage(MessageType type)
        {
            gameObject.SetActive(true);
            if(previousIndex != (int)type)
            {
                Messages[previousIndex].SetActive(false);
            }
            Debug.Log("ShowMessage: " + type + "/" + ((int)type));
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
                        EffectText.text = "SPEED BOOST! +500G";
                        break;
                    case EffectType.Shield:
                        EffectText.text = "GOT SHIELD! +500G";
                        break;
                    case EffectType.Slow:
                        EffectText.text = "IT'S SLOW TIME! +500";
                        break;
                }
            }

            if (animCoroutines[previousIndex] != null) StopCoroutine(animCoroutines[previousIndex]);
            if (animCoroutines[index] != null) StopCoroutine(animCoroutines[index]);

            // Đăng ký EffectAnimation request
            float duration = (type == MessageType.Bonus) ? timeLife : 0f;
            activeAnims[index] = new EffectAnimation(EffectType.None, duration);

            animCoroutines[index] = StartCoroutine(AnimShow(Messages[index]));
            previousIndex = index;
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
            Debug.Log("Start hiding message: " + msg.name);
            float elapsed = 0f;
            while (elapsed < hideDuration)
            {
                elapsed += Time.deltaTime;
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
        LookOut = 0,
        Bonus = 1,
        Effects = 2
    }

    public struct EffectAnimation{
        public EffectType Type;
        public bool IsAnimtionShowing;
        public float Duration;

        public EffectAnimation(EffectType type, float duration)
        {
            Type = type;
            Duration = duration;
            IsAnimtionShowing = true;
        }
    }

}

