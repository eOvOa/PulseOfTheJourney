using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

[System.Serializable]
public class NoteData
{
    public float time;
    public int lane;
    public string type;
    public int holdLength;
}

[System.Serializable]
public class NoteDataList
{
    public List<NoteData> list;
}

public class BeatmapLoader : MonoBehaviour
{
    public static BeatmapLoader Instance { get; private set; }

    public List<NoteData> notes = new List<NoteData>();
    public AudioSource audioSource;
    public bool musicStarted = false;

    public float approachTime = 2f;
    private float musicDelay = 0f;
    private bool isGameScene = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CheckSceneAndLoadBeatmap();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (!isGameScene) return;
        FindAudioSource();
        CalculateMusicDelay();
        StartCoroutine(DelayedMusicStart());
    }

    private void FindAudioSource()
    {
        if (audioSource != null) return;

        if (SoundtrackManager.Instance != null)
        {
            audioSource = SoundtrackManager.Instance.AudioSource;
            if (audioSource != null) return;
        }

        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        if (audioSources.Length > 0)
        {
            audioSource = audioSources[0];
        }
    }

    private void CheckSceneAndLoadBeatmap()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        string beatmapFileName = currentSceneName.ToLower() + ".json";
        string path = Path.Combine(Application.streamingAssetsPath, beatmapFileName);

        if (File.Exists(path))
        {
            isGameScene = true;
            LoadBeatmap();
        }
        else
        {
            isGameScene = false;
            notes.Clear();
        }
    }


    private void LoadBeatmap()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        string beatmapFileName = currentSceneName.ToLower() + ".json";
        string path = Path.Combine(Application.streamingAssetsPath, beatmapFileName);

        if (File.Exists(path))
        {
            string jsonContent = File.ReadAllText(path);
            notes = ParseJson(jsonContent);
        }
        else
        {
            notes.Clear();
        }
    }

    private List<NoteData> ParseJson(string json)
    {
        string wrappedJson = "{\"list\":" + json + "}";
        NoteDataList dataList = JsonUtility.FromJson<NoteDataList>(wrappedJson);
        return dataList.list;
    }

    private void CalculateMusicDelay()
    {
        if (notes.Count == 0)
        {
            musicDelay = 0f;
            return;
        }

        float firstNoteTime = notes[0].time;
        musicDelay = firstNoteTime - approachTime;

        if (musicDelay < 0f)
        {
            musicDelay = 0f;
        }
    }

    private IEnumerator DelayedMusicStart()
    {
        yield return new WaitForSeconds(musicDelay);

        if (audioSource != null)
        {
            audioSource.Play();
            musicStarted = true;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckSceneAndLoadBeatmap();
        if (!isGameScene) return;
        FindAudioSource();
        CalculateMusicDelay();
        StartCoroutine(DelayedMusicStart());
    }
}
