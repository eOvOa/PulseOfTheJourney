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

    // 移除难度切换特定按键设置，使用任意键切换

    private bool isAnimationPlayed = false;
    private bool isSelectingDifficulty = false;
    private float pressStartTime = 0f;
    private bool isButtonPressed = false;
    private bool allKeysHeld = false;
    
    // 用于确认的ASDF键
    private KeyCode[] confirmKeys = new KeyCode[] { 
        KeyCode.A, 
        KeyCode.S, 
        KeyCode.D, 
        KeyCode.F 
    };

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
            textFader.Show("Press ANY KEY to start", true);
        }
    }

    void Update()
    {
        if (!isAnimationPlayed)
        {
            // 检测任意按键而不只是空格键
            if (Input.anyKeyDown)
            {
                PlayIntroAnimation();
            }
        }
        else if (isSelectingDifficulty)
        {
            // 检查是否按下了任意键来切换难度
            if (Input.anyKeyDown && !CheckAnyConfirmKeyPressed())
            {
                CycleDifficulty();
            }

            // 检查是否同时按下了ASDF键
            allKeysHeld = CheckAllConfirmKeysHeld();

            if (allKeysHeld)
            {
                if (!isButtonPressed)
                {
                    pressStartTime = Time.time;
                    isButtonPressed = true;

                    if (textFader != null)
                        textFader.Show("Hold ASDF to enter level", true);
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
                    textFader.Show("Hold ASDF to enter difficulty", true);
            }
        }
    }

    // 检查是否所有确认键（ASDF）都被按住
    private bool CheckAllConfirmKeysHeld()
    {
        foreach (KeyCode key in confirmKeys)
        {
            if (!Input.GetKey(key))
            {
                return false;
            }
        }
        return true;
    }
    
    // 检查是否有任何确认键被按下（用于防止确认键被用作切换难度键）
    private bool CheckAnyConfirmKeyPressed()
    {
        foreach (KeyCode key in confirmKeys)
        {
            if (Input.GetKeyDown(key))
            {
                return true;
            }
        }
        return false;
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
            textFader.Show("Press ANY KEY to change difficulty\nHold ASDF to enter", true);
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