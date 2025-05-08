using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MusicControlManager : MonoBehaviour
{
    public AudioSource musicSource;
    public Button startButton;
    public Button pauseButton;
    public Button stopButton;

    private bool musicStarted = false;
    private bool musicPaused = false;
    
    // 定义颜色
    private Color activeColor;
    private Color inactiveColor;
    
    // 存储按钮图像组件的引用
    private Image startButtonImage;
    private Image pauseButtonImage;
    private Image stopButtonImage;

    void Start()
    {
        Time.timeScale = 0f;
        musicSource.Pause(); 

        startButton.onClick.AddListener(OnStartClicked);
        pauseButton.onClick.AddListener(OnPauseClicked);
        stopButton.onClick.AddListener(OnStopClicked);
        
        // 直接使用十六进制颜色值转换为Unity颜色
        ColorUtility.TryParseHtmlString("#2D5C85", out activeColor);
        ColorUtility.TryParseHtmlString("#BCD7E7", out inactiveColor);
        
        // 获取按钮的Image组件
        startButtonImage = startButton.GetComponent<Image>();
        pauseButtonImage = pauseButton.GetComponent<Image>();
        stopButtonImage = stopButton.GetComponent<Image>();
        
        // 初始化所有按钮为非活动颜色
        startButtonImage.color = inactiveColor;
        pauseButtonImage.color = inactiveColor;
        stopButtonImage.color = inactiveColor;
    }

    void OnStartClicked()
    {
        if (!musicStarted)
        {
            musicSource.Play();
            musicStarted = true;
            musicPaused = false;
        }
        else if (musicPaused)
        {
            musicSource.UnPause();
        }

        Time.timeScale = 1f;
        musicPaused = false;
        
        // 更新按钮颜色 - 只有开始按钮是活动的
        startButtonImage.color = activeColor;
        pauseButtonImage.color = inactiveColor;
        stopButtonImage.color = inactiveColor;
    }

    void OnPauseClicked()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
            Time.timeScale = 0f;
            musicPaused = true;
            
            // 更新按钮颜色 - 只有暂停按钮是活动的
            startButtonImage.color = inactiveColor;
            pauseButtonImage.color = activeColor;
            stopButtonImage.color = inactiveColor;
        }
    }

    void OnStopClicked()
    {
        musicSource.Stop();
        Time.timeScale = 1f; 
        musicStarted = false;
        musicPaused = false;
        
        // 更新按钮颜色 - 只有停止按钮是活动的
        startButtonImage.color = inactiveColor;
        pauseButtonImage.color = inactiveColor;
        stopButtonImage.color = activeColor;
        
        // 延迟一帧加载场景，确保颜色变化可见
        StartCoroutine(LoadSceneAfterFrame());
    }
    
    System.Collections.IEnumerator LoadSceneAfterFrame()
    {
        // 等待一帧以显示颜色变化
        yield return null;
        SceneManager.LoadScene("Start");
    }
}