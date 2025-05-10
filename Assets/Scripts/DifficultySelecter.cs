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
    public GameObject[] difficultySprites; // 三个 GameObject，挂的是 easy/medium/hard 图
    private int currentDifficultyIndex = 0;

    [Header("动画设置")]
    public Animator introAnimator; // 播放动画
    public string animationTriggerName = "Play";

    [Header("场景切换")]
    public string targetSceneName = "Game";
    public float longPressTime = 2.0f; // 长按时间确认

    [Header("确认进度条")]
    public Slider holdProgressBar; // 拖入 UI > Slider，默认 inactive

    // 状态控制
    private bool isAnimationPlayed = false;
    private bool isSelectingDifficulty = false;
    private float pressStartTime = 0f;
    private bool isButtonPressed = false;

    // 静态难度变量
    public static int selectedDifficulty = 0;

    void Start()
    {
        if (soundtrackManager == null)
            soundtrackManager = FindObjectOfType<SoundtrackManager>();

        // 初始化所有难度图标为隐藏
        foreach (GameObject sprite in difficultySprites)
        {
            if (sprite != null) sprite.SetActive(false);
        }

        // 隐藏进度条
        if (holdProgressBar != null)
        {
            holdProgressBar.gameObject.SetActive(false);
            holdProgressBar.value = 0f;
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
            // 任意按键按下，切换难度 + 开始计时
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.anyKeyDown)
            {
                pressStartTime = Time.time;
                isButtonPressed = true;
                CycleDifficulty();
            }

            // 正在长按：更新进度条
            if (isButtonPressed)
            {
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
                        holdProgressBar.gameObject.SetActive(false);
                        holdProgressBar.value = 0f;
                    }
                }
            }

            // 松手取消
            if (isButtonPressed && (Input.GetKeyUp(KeyCode.Space) || Input.GetMouseButtonUp(0) || !Input.anyKey))
            {
                isButtonPressed = false;

                if (holdProgressBar != null)
                {
                    holdProgressBar.value = 0f;
                    holdProgressBar.gameObject.SetActive(false);
                }
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
        SceneManager.LoadScene(targetSceneName);
    }

    public static int GetSelectedDifficulty()
    {
        return selectedDifficulty;
    }
}
