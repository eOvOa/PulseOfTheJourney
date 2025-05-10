using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DifficultySelector : MonoBehaviour
{
    public SoundtrackManager soundtrackManager;
    public float fadeOutDuration = 1.0f;

    public GameObject[] difficultySprites;
    private int currentDifficultyIndex = 0;

    public Animator introAnimator;
    public string animationTriggerName = "Play";

    public float longPressTime = 2.0f;
    public Slider holdProgressBar;
    public TextFader textFader;

    private bool isAnimationPlayed = false;
    private bool isSelectingDifficulty = false;
    private float pressStartTime = 0f;
    private bool isButtonPressed = false;
    private bool wasHoldingAllKeys = false;

    private KeyCode[] confirmKeys = new KeyCode[] {
        KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F
    };

    private Coroutine simultaneousCheckCoroutine;
    public static int selectedDifficulty = 0;

    void Start()
    {
        if (soundtrackManager == null)
            soundtrackManager = SoundtrackManager.Instance;

        if (soundtrackManager != null)
        {
            SpriteRenderer bgRenderer = FindObjectOfType<SpriteRenderer>();
            soundtrackManager.SetSceneReferences(bgRenderer, textFader);
            soundtrackManager.StopAudio();
            soundtrackManager.ResumeAudio();
        }

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
            if (Input.anyKeyDown)
            {
                PlayIntroAnimation();
            }
        }
        else if (isSelectingDifficulty)
        {
            bool currentlyHoldingAllKeys = CheckAllConfirmKeysHeld();

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

                    if (textFader != null)
                        textFader.Hide();
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
            else if (Input.anyKeyDown)
            {
                if (simultaneousCheckCoroutine == null)
                {
                    simultaneousCheckCoroutine = StartCoroutine(CheckSimultaneousPress(0.15f));
                }
            }

            wasHoldingAllKeys = currentlyHoldingAllKeys;
        }
    }

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

    private IEnumerator CheckSimultaneousPress(float window)
    {
        float startTime = Time.time;

        while (Time.time - startTime < window)
        {
            int keysHeld = 0;
            foreach (KeyCode key in confirmKeys)
            {
                if (Input.GetKey(key))
                    keysHeld++;
            }

            if (keysHeld >= 4)
            {
                yield break;
            }

            yield return null;
        }

        CycleDifficulty();
        simultaneousCheckCoroutine = null;
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

        if (soundtrackManager != null)
            soundtrackManager.PrepareForNewScene();

        string sceneToLoad = GetSceneNameForDifficulty(currentDifficultyIndex);
        SceneManager.LoadScene(sceneToLoad);
    }

    private string GetSceneNameForDifficulty(int index)
    {
        switch (index)
        {
            case 0: return "Easy";
            case 1: return "Medium";
            case 2: return "Hard";
            default: return "Game";
        }
    }

    public static int GetSelectedDifficulty()
    {
        return selectedDifficulty;
    }
}
