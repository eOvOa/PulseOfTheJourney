using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SoundtrackManager : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private float defaultFadeOutDuration = 1.0f;
    [SerializeField] private float animationDuration = 2.0f; // 设置您的动画时长
    private Coroutine fadeCoroutine;
    
    public SpriteRenderer backgroundRenderer;
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
    }

    public void FadeOutAndPause()
    {
        FadeOutAndPause(defaultFadeOutDuration);
    }

    public void FadeOutAndPause(float duration)
    {
        // 淡出音频
        if (audioSource != null)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            
            fadeCoroutine = StartCoroutine(FadeOutAudio(duration));
        }
        
        // 等待动画完成后淡出Sprite
        StartCoroutine(WaitAndFadeOutSprite(duration));
    }

    private IEnumerator WaitAndFadeOutSprite(float duration)
    {
        // 等待动画完成
        yield return new WaitForSeconds(animationDuration);
        
        // 现在淡出Sprite
        if (spriteFadeCoroutine != null)
        {
            StopCoroutine(spriteFadeCoroutine);
        }
        
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
        // 淡入音频
        if (audioSource != null)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = StartCoroutine(FadeInAudio(duration));
        }
        
        // 淡入Sprite
        if (backgroundRenderer != null)
        {
            if (spriteFadeCoroutine != null)
            {
                StopCoroutine(spriteFadeCoroutine);
            }
            spriteFadeCoroutine = StartCoroutine(FadeInSprite(duration));
        }
    }
    
    private IEnumerator FadeInAudio(float duration)
    {
        audioSource.volume = 0f;
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
        
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