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

    public float approachTime = 2f; // 你设定的approach time（比如2秒）
    private float musicDelay = 0f;   // 自动计算得到的延迟时间

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadBeatmap();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        }

        CalculateMusicDelay();
        StartCoroutine(DelayedMusicStart());
    }

    private void LoadBeatmap()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        string beatmapFileName;

        // 根据场景名称选择不同的JSON文件
        if (currentSceneName == "Game")
        {
            beatmapFileName = "hard.json";
        }
        else if (currentSceneName == "Medium")
        {
            beatmapFileName = "medium.json";
        }
        else if (currentSceneName == "Easy")
        {
            beatmapFileName = "easy.json";
        }
        else
        {
            Debug.LogError("❌ Unknown scene name: " + currentSceneName);
            return;
        }

        string path = Path.Combine(Application.streamingAssetsPath, beatmapFileName);

        if (File.Exists(path))
        {
            string jsonContent = File.ReadAllText(path);
            notes = ParseJson(jsonContent);
            Debug.Log($"✅ Loaded {notes.Count} notes from {beatmapFileName}");
        }
        else
        {
            Debug.LogError($"❌ Beatmap file not found at: {path}");
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
            Debug.LogWarning("⚠️ No notes loaded, skipping music delay calculation.");
            musicDelay = 0f;
            return;
        }

        float firstNoteTime = notes[0].time;
        musicDelay = firstNoteTime - approachTime;

        if (musicDelay < 0f)
        {
            Debug.LogWarning($"⚠️ First note is too early! Setting musicDelay = 0");
            musicDelay = 0f;
        }

        Debug.Log($"🎵 Auto-calculated music delay: {musicDelay:F3} seconds (First note at {firstNoteTime:F3}s, ApproachTime {approachTime}s)");
    }

    private IEnumerator DelayedMusicStart()
    {
        yield return new WaitForSeconds(musicDelay);

        audioSource.Play();
        musicStarted = true;
        Debug.Log("🎵 Music started after delay: " + musicDelay.ToString("F3") + "s");
    }
}