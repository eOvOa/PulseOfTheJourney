using System.Collections.Generic;
using UnityEngine;
using System.IO;

// 单个Note数据结构
[System.Serializable]
public class NoteData
{
    public float time;
    public int lane;
    public string type;
    public int holdLength;
}

// Json数组包装
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
    public float approachTime = 3f;
    public bool musicStarted = false;
    public float manualDelay = 1.785f; // 延迟播放秒数

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

    private void LoadBeatmap()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "beatmap.json");

        if (File.Exists(path))
        {
            string rawJson = File.ReadAllText(path).Trim();

            // 检查是否是裸数组 []
            if (rawJson.StartsWith("["))
            {
                rawJson = "{\"list\":" + rawJson + "}";
            }

            notes = ParseJson(rawJson);
            Debug.Log($"✅ Loaded {notes.Count} notes from beatmap.json");
        }
        else
        {
            Debug.LogError("❌ Beatmap file not found at: " + path);
        }
    }

    private List<NoteData> ParseJson(string json)
    {
        NoteDataList dataList = JsonUtility.FromJson<NoteDataList>(json);
        return dataList.list;
    }

    private void Start()
    {
        if (notes.Count == 0)
        {
            Debug.LogError("❌ No notes loaded!");
            return;
        }

        StartCoroutine(DelayedMusicStart());
    }

    private System.Collections.IEnumerator DelayedMusicStart()
    {
        yield return new WaitForSeconds(manualDelay);

        audioSource.Play();
        musicStarted = true;
        Debug.Log("🎵 Music started after delay: " + manualDelay + "s");
    }
}
