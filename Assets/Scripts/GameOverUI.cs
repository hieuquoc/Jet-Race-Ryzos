using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ZyroX;

public class GameOverUI : UIPopUpBase
{
    public TextMeshProUGUI DistanceText;
    public TextMeshProUGUI TargetDistanceText;
    public TextMeshProUGUI CoinCollectedText;

    [SerializeField] private float countDuration = 1.5f;
    [SerializeField] private float bounceDuration = 0.15f;
    [SerializeField] private float bounceScale = 1.25f;
    [SerializeField] private float bounceDelay = 0.1f;

    public override void Show()
    {
        base.Show();
        TargetDistanceText.SetText($"{PlayerData.RecordDistance:F1}m");
        CoinCollectedText.SetText($"{GameManager.RunCoin}");
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        // đếm distance
        yield return StartCoroutine(CountDistance(MapController.RunDistance));

        // bounce lần lượt 3 text
        yield return StartCoroutine(BounceText(DistanceText));
        yield return new WaitForSeconds(bounceDelay);
        yield return StartCoroutine(BounceText(TargetDistanceText));
        yield return new WaitForSeconds(bounceDelay);
        yield return StartCoroutine(BounceText(CoinCollectedText));
    }

    private IEnumerator CountDistance(float targetValue)
    {
        float elapsed = 0f;
        while (elapsed < countDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float current = Mathf.Lerp(0f, targetValue, elapsed / countDuration);
            DistanceText.SetText($"{current:F1}m");
            yield return null;
        }
        DistanceText.SetText($"{targetValue:F1}m");
    }

    private IEnumerator BounceText(TextMeshProUGUI text)
    {
        Vector3 original = text.transform.localScale;
        Vector3 big = original * bounceScale;
        float half = bounceDuration * 0.5f;

        float elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.unscaledDeltaTime;
            text.transform.localScale = Vector3.LerpUnclamped(original, big, elapsed / half);
            yield return null;
        }
        elapsed = 0f;
        while (elapsed < half)
        {
            elapsed += Time.unscaledDeltaTime;
            text.transform.localScale = Vector3.LerpUnclamped(big, original, elapsed / half);
            yield return null;
        }
        text.transform.localScale = original;
    }

    public void OnRestartButtonClicked()
    {
        Hide();
        GameManager.Instance.StartGame();
        AudioManager.Instance.PlayButtonClick();
    }

    public void OnStoreButtonClicked()
    {
        UIManager.Instance.ShowPopUp(UIManager.Instance.ShopUI);
        AudioManager.Instance.PlayButtonClick();
    }

    public void OnHomeButtonClicked()
    {
        UIManager.Instance.ShowPopUp(UIManager.Instance.HomeUI);
        AudioManager.Instance.PlayButtonClick();
    }
}

