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

    void Start()
    {
        Time.timeScale = 0f;
        musicSource.Pause(); 

        startButton.onClick.AddListener(OnStartClicked);
        pauseButton.onClick.AddListener(OnPauseClicked);
        stopButton.onClick.AddListener(OnStopClicked);
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
    }

    void OnPauseClicked()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
            Time.timeScale = 0f;
            musicPaused = true;
        }
    }

    void OnStopClicked()
    {
        musicSource.Stop();
        Time.timeScale = 1f; 
        musicStarted = false;
        musicPaused = false;

        SceneManager.LoadScene("Start");
    }
}