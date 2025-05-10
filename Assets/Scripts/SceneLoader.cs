using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    private bool isAnimationPlaying = false;
    public Animator openAnimator;
    public SoundtrackManager soundtrackManager;
    public float fadeDuration = 1.5f;

    void Start()
    {
        if (soundtrackManager == null)
        {
            soundtrackManager = FindObjectOfType<SoundtrackManager>();
        }
    }

    void Update()
    {
        if (!isAnimationPlaying && Input.anyKeyDown)
        {
            isAnimationPlaying = true;
            StartCoroutine(PlayAndLoad());
        }
    }

    IEnumerator PlayAndLoad()
    {
        if (openAnimator != null)
        {
            openAnimator.SetTrigger("Play");
        }

        float animationLength = 1.5f;
        if (openAnimator != null)
        {
            foreach (AnimationClip clip in openAnimator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == "start")
                {
                    animationLength = clip.length;
                    break;
                }
            }
        }

        if (soundtrackManager != null)
        {
            soundtrackManager.FadeOutAndPause(fadeDuration);
        }

        yield return new WaitForSeconds(animationLength);
        SceneManager.LoadScene("Game");
    }
}