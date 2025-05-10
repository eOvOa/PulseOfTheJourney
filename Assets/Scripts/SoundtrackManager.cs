using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundtrackManager : MonoBehaviour
{
    public AudioClip soundtrack;
    private AudioSource audioSource;

    private static bool hasPlayed = false; // 防止重复播放

    void Awake()
    {
        if (!hasPlayed)
        {
            DontDestroyOnLoad(gameObject); // 切场景不销毁
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = soundtrack;
            audioSource.loop = true;
            audioSource.Play();
            hasPlayed = true;
        }
        else
        {
            Destroy(gameObject); // 防止有多个 soundtrack manager
        }

        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    void OnSceneUnloaded(Scene current)
    {
        // 这里你可以用更精细的条件控制是否停止播放
        StopSoundtrack();
    }

    public void StopSoundtrack()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
            Destroy(gameObject); // 移除这个音乐管理器
            hasPlayed = false;
        }
    }
}