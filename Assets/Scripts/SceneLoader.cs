using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private bool isAnimationPlaying = false;
    public Animator openAnimator; // Reference to the three-needle animator
    
    void Update()
    {
        // Check if space key is pressed and animation is not already playing
        if (!isAnimationPlaying && Input.GetKeyDown(KeyCode.Space))
        {
            isAnimationPlaying = true;
            PlayThreeNeedleAnimation();
        }
    }
    
    void PlayThreeNeedleAnimation()
    {
        // Play the animation
        openAnimator.SetTrigger("Play");
        
        // Get the length of the animation
        AnimationClip[] clips = openAnimator.runtimeAnimatorController.animationClips;
        float animationLength = 0f;
        
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == "ThreeNeedleAnimation") // Replace with your actual animation name
            {
                animationLength = clip.length;
                break;
            }
        }
        
        // Load the Game scene after the animation completes
        Invoke("LoadGameScene", animationLength);
    }
    
    void LoadGameScene()
    {
        SceneManager.LoadScene("Game");
    }
}