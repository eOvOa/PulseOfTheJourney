using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class DifficultySelector : MonoBehaviour
{
    [Header("音频设置")]
    public SoundtrackManager soundtrackManager;
    public float fadeOutDuration = 1.0f;

    [Header("难度选择")]
    public GameObject[] difficultySprites;
    private int currentDifficultyIndex = 0;

    [Header("动画设置")]
    public Animator introAnimator;
    public string animationTriggerName = "Play";

    [Header("场景切换")]
    public float longPressTime = 2.0f;

    [Header("确认进度条")]
    public Slider holdProgressBar;

    [Header("提示文字管理")]
    public TextFader textFader;

    private bool isAnimationPlayed = false;
    private bool isSelectingDifficulty = false;
    private float pressStartTime = 0f;
    private bool isButtonPressed = false;
    private bool allKeysHeld = false;

    public static int selectedDifficulty = 0;

    void Start()
    {
        if (soundtrackManager == null)
            soundtrackManager = FindObjectOfType<SoundtrackManager>();

        foreach (GameObject sprite in difficultySprites)
        {
            if (sprite != null) sprite.SetActive(false);
        }

        if (holdProgressBar != null)
        {
            holdProgressBar.gameObject.SetActive(false);
            holdProgressBar.value = 0f;
        }

        if (textFader != null)
        {
            textFader.Show("Press SPACE to start", true);
        }
    }

    void Update()
    {
        if (!isAnimationPlayed)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PlayIntroAnimation();
            }
        }
        else if (isSelectingDifficulty)
        {
            if (Input.anyKeyDown)
            {
                CycleDifficulty();
            }

            allKeysHeld = Input.GetKey(KeyCode.A) &&
                          Input.GetKey(KeyCode.S) &&
                          Input.GetKey(KeyCode.D) &&
                          Input.GetKey(KeyCode.F);

            if (allKeysHeld)
            {
                if (!isButtonPressed)
                {
                    pressStartTime = Time.time;
                    isButtonPressed = true;

                    if (textFader != null)
                        textFader.Show("Press all buttons to enter level", true);
                }

                float heldTime = Time.time - pressStartTime;

                if (holdProgressBar != null)
                {
                    if (!holdProgressBar.gameObject.activeSelf)
                        holdProgressBar.gameObject.SetActive(true);

                    holdProgressBar.value = heldTime / longPressTime;
                }

                if (heldTime >= longPressTime)
                {
                    selectedDifficulty = currentDifficultyIndex;
                    StartLoadingScene();
                    isButtonPressed = false;

                    if (holdProgressBar != null)
                    {
                        holdProgressBar.value = 0f;
                        holdProgressBar.gameObject.SetActive(false);
                    }

                    if (textFader != null) textFader.Hide();
                }
            }
            else if (isButtonPressed)
            {
                isButtonPressed = false;

                if (holdProgressBar != null)
                {
                    holdProgressBar.value = 0f;
                    holdProgressBar.gameObject.SetActive(false);
                }

                if (textFader != null)
                    textFader.Show("Hold to enter difficulty", true);
            }
        }
    }

    private void PlayIntroAnimation()
    {
        if (introAnimator != null)
        {
            introAnimator.SetTrigger(animationTriggerName);

            float animationLength = 1.5f;
            foreach (AnimationClip clip in introAnimator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == "start")
                {
                    animationLength = clip.length;
                    break;
                }
            }

            StartCoroutine(ShowDifficultySelectAfterAnimation(animationLength));
        }
        else
        {
            ShowDifficultySelect();
        }

        isAnimationPlayed = true;
    }

    private IEnumerator ShowDifficultySelectAfterAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowDifficultySelect();
    }

    private void ShowDifficultySelect()
    {
        isSelectingDifficulty = true;
        UpdateDifficultyDisplay();

        if (textFader != null)
        {
            textFader.Show("Hold to enter difficulty", true);
        }
    }

    private void CycleDifficulty()
    {
        currentDifficultyIndex = (currentDifficultyIndex + 1) % difficultySprites.Length;
        UpdateDifficultyDisplay();
    }

    private void UpdateDifficultyDisplay()
    {
        for (int i = 0; i < difficultySprites.Length; i++)
        {
            if (difficultySprites[i] != null)
            {
                difficultySprites[i].SetActive(i == currentDifficultyIndex);
            }
        }
    }

    private void StartLoadingScene()
    {
        isSelectingDifficulty = false;

        if (soundtrackManager != null)
        {
            soundtrackManager.FadeOutAndPause(fadeOutDuration);
        }

        StartCoroutine(LoadSceneAfterFadeOut());
    }

    private IEnumerator LoadSceneAfterFadeOut()
    {
        yield return new WaitForSeconds(fadeOutDuration);

        string sceneToLoad = GetSceneNameForDifficulty(currentDifficultyIndex);
        SceneManager.LoadScene(sceneToLoad);
    }

    private string GetSceneNameForDifficulty(int index)
    {
        switch (index)
        {
            case 0: return "Easy";
            case 1: return "Medium";
            case 2: return "Game";
            default: return "Game";
        }
    }

    public static int GetSelectedDifficulty()
    {
        return selectedDifficulty;
    }
}
