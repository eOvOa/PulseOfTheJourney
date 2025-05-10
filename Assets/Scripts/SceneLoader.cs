using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    private bool isAnimationPlaying = false;
    public Animator openAnimator; // 在 Inspector 中挂带 Animator 的对象

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
        openAnimator.SetTrigger("Play");

        float animationLength = 1.5f; // 默认时长，防止没找到动画

        // 获取动画实际时长
        foreach (AnimationClip clip in openAnimator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "start")
            {
                animationLength = clip.length;
                break;
            }
        }

        yield return new WaitForSeconds(animationLength); // 等动画播放完

        SceneManager.LoadScene("Game"); // 切换场景
    }
}