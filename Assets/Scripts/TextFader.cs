using UnityEngine;
using TMPro;
using System.Collections;

public class TextFader : MonoBehaviour
{
    public TextMeshProUGUI hintText;
    public TextMeshProUGUI allButtonText;

    private Coroutine fadeCoroutine;

    public void Show(string message, bool isHint)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

        if (isHint && hintText != null)
        {
            hintText.text = message;
            fadeCoroutine = StartCoroutine(FadeTextAlpha(hintText, 0f, 1f, 0.5f));
        }
        else if (!isHint && allButtonText != null)
        {
            allButtonText.text = message;
            fadeCoroutine = StartCoroutine(FadeTextAlpha(allButtonText, 0f, 1f, 0.5f));
        }
    }

    public void Hide()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        if (hintText != null) fadeCoroutine = StartCoroutine(FadeTextAlpha(hintText, hintText.color.a, 0f, 0.5f));
        if (allButtonText != null) fadeCoroutine = StartCoroutine(FadeTextAlpha(allButtonText, allButtonText.color.a, 0f, 0.5f));
    }

    public void SetHintAlpha(float alpha)
    {
        if (hintText != null)
        {
            Color c = hintText.color;
            hintText.color = new Color(c.r, c.g, c.b, alpha);
        }
    }

    public void SetAllButtonAlpha(float alpha)
    {
        if (allButtonText != null)
        {
            Color c = allButtonText.color;
            allButtonText.color = new Color(c.r, c.g, c.b, alpha);
        }
    }

    private IEnumerator FadeTextAlpha(TextMeshProUGUI text, float from, float to, float duration)
    {
        float timer = 0;
        Color c = text.color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(from, to, timer / duration);
            text.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }

        text.color = new Color(c.r, c.g, c.b, to);
    }
}