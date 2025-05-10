using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class SoundtrackManager : MonoBehaviour
{
    public static SoundtrackManager Instance { get; private set; }

    private AudioSource audioSource;

    [SerializeField] private float defaultFadeOutDuration = 1.0f;
    [SerializeField] private float animationDuration = 2.0f;

    public SpriteRenderer backgroundRenderer;
    public TextFader textFader;

    public AudioSource AudioSource
    {
        get
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
            return audioSource;
        }
    }

    private Coroutine fadeCoroutine;
    private Coroutine spriteFadeCoroutine;
    private bool isTransitioning = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isTransitioning = false;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        if (AudioSource != null)
        {
            AudioSource.loop = true;
            if (!AudioSource.isPlaying && AudioSource.clip != null)
            {
                AudioSource.Play();
            }
        }
    }

    void OnEnable()
    {
        if (AudioSource != null && !AudioSource.isPlaying && AudioSource.clip != null)
        {
            AudioSource.volume = 1.0f;
            AudioSource.Play();
        }

        UpdateBackgroundRenderer();
        UpdateTextFader();
    }

    private void UpdateBackgroundRenderer()
    {
        if (backgroundRenderer != null)
        {
            Color color = backgroundRenderer.color;
            backgroundRenderer.color = new Color(color.r, color.g, color.b, 1f);
        }
    }

    private void UpdateTextFader()
    {
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
        isTransitioning = true;

        if (AudioSource != null)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeOutAudio(duration));
        }

        if (backgroundRenderer != null)
        {
            StartCoroutine(WaitAndFadeOutSprite(duration));
        }
    }

    private IEnumerator WaitAndFadeOutSprite(float duration)
    {
        yield return new WaitForSeconds(animationDuration);

        if (spriteFadeCoroutine != null) StopCoroutine(spriteFadeCoroutine);
        spriteFadeCoroutine = StartCoroutine(FadeOutSprite(duration));
    }

    private IEnumerator FadeOutAudio(float duration)
    {
        if (AudioSource == null) yield break;

        float startVolume = AudioSource.volume;
        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            AudioSource.volume = Mathf.Lerp(startVolume, 0, timer / duration);
            yield return null;
        }

        AudioSource.Pause();
        fadeCoroutine = null;
    }

    private IEnumerator FadeOutSprite(float duration)
    {
        if (backgroundRenderer == null) yield break;

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
        isTransitioning = false;

        if (AudioSource != null)
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
        if (AudioSource == null) yield break;

        AudioSource.volume = 0f;
        if (!AudioSource.isPlaying && AudioSource.clip != null) AudioSource.Play();

        float timer = 0;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            AudioSource.volume = Mathf.Lerp(0, 1, timer / duration);
            yield return null;
        }

        AudioSource.volume = 1f;
        fadeCoroutine = null;
    }

    private IEnumerator FadeInSprite(float duration)
    {
        if (backgroundRenderer == null) yield break;

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

    public void PrepareForNewScene()
    {
        isTransitioning = true;

        if (AudioSource != null)
        {
            if (AudioSource.isPlaying)
            {
                AudioSource.Pause();
            }
        }

        backgroundRenderer = null;
        textFader = null;
    }

    public void SetSceneReferences(SpriteRenderer newBgRenderer, TextFader newTextFader)
    {
        backgroundRenderer = newBgRenderer;
        textFader = newTextFader;

        UpdateBackgroundRenderer();
        UpdateTextFader();
        RestartAudioFresh();
    }

    public void RestartAudioFresh()
    {
        if (AudioSource == null || AudioSource.clip == null) return;

        StopAllCoroutines();

        AudioSource.enabled = true;
        AudioSource.gameObject.SetActive(true);

        AudioSource.Stop();
        AudioSource.volume = 1.0f;
        AudioSource.time = 0f;
        AudioSource.loop = true;
        AudioSource.Play();
    }

    public void StopAudio()
    {
        if (AudioSource != null && AudioSource.isPlaying)
        {
            AudioSource.Stop();
        }
    }

    public void ResumeAudio()
    {
        if (AudioSource != null)
        {
            AudioSource.enabled = true;
            AudioSource.gameObject.SetActive(true);

            if (!AudioSource.isPlaying && AudioSource.clip != null)
            {
                AudioSource.Play();
            }
        }
    }

    public bool IsAudioPlaying()
    {
        return AudioSource != null && AudioSource.isPlaying;
    }

    public void SetAudioClip(AudioClip clip)
    {
        if (AudioSource != null)
        {
            bool wasPlaying = AudioSource.isPlaying;
            AudioSource.clip = clip;

            if (wasPlaying && clip != null)
            {
                AudioSource.Play();
            }
        }
    }

    public bool IsTransitioning()
    {
        return isTransitioning;
    }
}
