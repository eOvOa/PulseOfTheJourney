using UnityEngine;
using System.Collections;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class SoundtrackManager : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private float defaultFadeOutDuration = 1.0f;
    [SerializeField] private float animationDuration = 2.0f;

    public SpriteRenderer backgroundRenderer;
    public TextFader textFader; // 新增：统一管理 hintText 和 allButtonText

    private Coroutine fadeCoroutine;
    private Coroutine spriteFadeCoroutine;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.Play();
    }

    void OnEnable()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.volume = 1.0f;
            audioSource.Play();
        }

        if (backgroundRenderer != null)
        {
            Color color = backgroundRenderer.color;
            backgroundRenderer.color = new Color(color.r, color.g, color.b, 1f);
        }

        if (textFader != null)
        {
            textFader.SetHintAlpha(1f);
            textFader.SetAllButtonAlpha(0f);
        }
    }

    public void FadeOutAndPause()
    {
        FadeOutAndPause(defaultFadeOutDuration);
    }

    public void FadeOutAndPause(float duration)
    {
        if (audioSource != null)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeOutAudio(duration));
        }

        StartCoroutine(WaitAndFadeOutSprite(duration));
    }

    private IEnumerator WaitAndFadeOutSprite(float duration)
    {
        yield return new WaitForSeconds(animationDuration);

        if (spriteFadeCoroutine != null) StopCoroutine(spriteFadeCoroutine);
        spriteFadeCoroutine = StartCoroutine(FadeOutSprite(duration));
    }

    private IEnumerator FadeOutAudio(float duration)
    {
        float startVolume = audioSource.volume;
        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0, timer / duration);
            yield return null;
        }

        audioSource.Pause();
        fadeCoroutine = null;
    }

    private IEnumerator FadeOutSprite(float duration)
    {
        Color startColor = backgroundRenderer.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            backgroundRenderer.color = Color.Lerp(startColor, targetColor, timer / duration);
            yield return null;
        }

        backgroundRenderer.color = targetColor;
        spriteFadeCoroutine = null;
    }

    public void FadeIn(float duration)
    {
        if (audioSource != null)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeInAudio(duration));
        }

        if (backgroundRenderer != null)
        {
            if (spriteFadeCoroutine != null) StopCoroutine(spriteFadeCoroutine);
            spriteFadeCoroutine = StartCoroutine(FadeInSprite(duration));
        }
    }

    private IEnumerator FadeInAudio(float duration)
    {
        audioSource.volume = 0f;
        if (!audioSource.isPlaying) audioSource.Play();

        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, 1, timer / duration);
            yield return null;
        }

        audioSource.volume = 1f;
        fadeCoroutine = null;
    }

    private IEnumerator FadeInSprite(float duration)
    {
        Color startColor = backgroundRenderer.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 1f);
        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            backgroundRenderer.color = Color.Lerp(startColor, targetColor, timer / duration);
            yield return null;
        }

        backgroundRenderer.color = targetColor;
        spriteFadeCoroutine = null;
    }
}
