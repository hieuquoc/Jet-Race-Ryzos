using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZyroX
{
    public class UIPopUpBase : MonoBehaviour
    {
        [SerializeField] private float animDuration = 0.25f;

        private Coroutine animCoroutine;

        public virtual void Show()
        {
            gameObject.SetActive(true);
            PlayAnim(Vector3.zero, Vector3.one);
        }

        public virtual void Hide()
        {
            PlayAnim(Vector3.one, Vector3.zero, onDone: () => gameObject.SetActive(false));
        }

        private void PlayAnim(Vector3 from, Vector3 to, System.Action onDone = null)
        {
            if (animCoroutine != null) StopCoroutine(animCoroutine);
            animCoroutine = StartCoroutine(ScaleAnim(from, to, onDone));
        }

        private IEnumerator ScaleAnim(Vector3 from, Vector3 to, System.Action onDone)
        {
            transform.localScale = from;
            float elapsed = 0f;
            while (elapsed < animDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / animDuration);
                transform.localScale = Vector3.LerpUnclamped(from, to, t);
                yield return null;
            }
            transform.localScale = to;
            onDone?.Invoke();
        }
    }


}
