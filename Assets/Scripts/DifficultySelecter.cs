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
    private bool wasHoldingAllKeys = false;
    
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
            // 首先检查当前帧是否同时按下了所有ASDF键
            bool currentlyHoldingAllKeys = CheckAllConfirmKeysHeld();
            
            // 预先检查：如果有任何ASDF键在这一帧被按下
            bool anyConfirmKeyDownThisFrame = CheckAnyConfirmKeyDown();
            
            // 确认键逻辑（同时按住ASDF）- 优先处理
            if (currentlyHoldingAllKeys)
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
                    textFader.Show("Press A/S/D/F to change difficulty\nHold ASDF to enter", true);
            }
            // 只有在以下情况下才检测难度切换操作：
            // 1. 当前没有同时按住所有ASDF键
            // 2. 上一帧也没有同时按住所有ASDF键
            // 3. 不是刚从"全部按住"状态释放键
            else if (!wasHoldingAllKeys)
            {
                // 检查是否单独按下了ASDF中的任意一个键来切换难度
                if (anyConfirmKeyDownThisFrame)
                {
                    CycleDifficulty();
                }
                // 检查是否按下了除ASDF之外的其他键来切换难度
                else if (Input.anyKeyDown)
                {
                    CycleDifficulty();
                }
            }
            
            // 保存当前的全键按住状态用于下一帧比较
            wasHoldingAllKeys = currentlyHoldingAllKeys;
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
    
    // 检查是否有任何确认键被按下
    private bool CheckAnyConfirmKeyDown()
    {
        // 首先检查是否已经有多个确认键被按住
        int keysCurrentlyHeld = 0;
        foreach (KeyCode key in confirmKeys)
        {
            if (Input.GetKey(key))
            {
                keysCurrentlyHeld++;
            }
        }
        
        // 如果已经有2个或更多确认键被按住，则不处理单键按下事件
        // 这可以防止在开始同时按下ASDF的过程中触发难度切换
        if (keysCurrentlyHeld >= 2)
        {
            return false;
        }
        
        // 只有当不是在尝试按住多个键时，才检查单个按键按下事件
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
            textFader.Show("Press A/S/D/F to change difficulty\nHold ASDF to enter", true);
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