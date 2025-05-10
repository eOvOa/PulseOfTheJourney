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
    
    // 用于存储当前场景是否是游戏场景
    private bool isGameScene = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 在Awake中检查当前场景并加载谱面
            CheckSceneAndLoadBeatmap();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 如果当前不是游戏场景，则不需要执行后续操作
        if (!isGameScene) return;
        
        // 找到音频源组件
        FindAudioSource();

        CalculateMusicDelay();
        StartCoroutine(DelayedMusicStart());
    }

    private void FindAudioSource()
    {
        // 如果已经在Inspector中分配了audioSource，就不需要查找
        if (audioSource != null)
        {
            return;
        }
        
        // 尝试查找场景中的音频源
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        if (audioSources.Length > 0)
        {
            audioSource = audioSources[0];
            Debug.Log($"✅ 找到音频源在物体: {audioSource.gameObject.name}");
        }
        else
        {
            Debug.LogError("❌ 场景中没有找到任何音频源！");
        }
    }

    private void CheckSceneAndLoadBeatmap()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        // 检查当前场景是否是游戏场景
        if (currentSceneName == "Game" || currentSceneName == "Medium" || currentSceneName == "Easy")
        {
            isGameScene = true;
            LoadBeatmap();
        }
        else
        {
            // 如果不是游戏场景（例如Start、Menu等），跳过加载谱面
            isGameScene = false;
            Debug.Log($"当前场景 '{currentSceneName}' 不是游戏场景，跳过谱面加载。");
        }
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
            // 对于非游戏场景，我们可以选择不加载谱面或加载默认谱面
            Debug.Log($"场景 '{currentSceneName}' 不需要加载谱面。");
            notes.Clear(); // 清空之前的谱面数据
            return;
        }

        string path = Path.Combine(Application.streamingAssetsPath, beatmapFileName);
        Debug.Log($"正在尝试加载: {path}");

        if (File.Exists(path))
        {
            string jsonContent = File.ReadAllText(path);
            notes = ParseJson(jsonContent);
            Debug.Log($"✅ 从 {beatmapFileName} 加载了 {notes.Count} 个音符");
        }
        else
        {
            Debug.LogError($"❌ 在路径找不到谱面文件: {path}");
            notes.Clear(); // 清空之前的谱面数据
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
            Debug.LogWarning("⚠️ 没有加载音符，跳过音乐延迟计算。");
            musicDelay = 0f;
            return;
        }

        float firstNoteTime = notes[0].time;
        musicDelay = firstNoteTime - approachTime;

        if (musicDelay < 0f)
        {
            Debug.LogWarning($"⚠️ 第一个音符太早了！设置 musicDelay = 0");
            musicDelay = 0f;
        }

        Debug.Log($"🎵 自动计算的音乐延迟: {musicDelay:F3} 秒 (第一个音符在 {firstNoteTime:F3}秒, ApproachTime {approachTime}秒)");
    }

    private IEnumerator DelayedMusicStart()
    {
        yield return new WaitForSeconds(musicDelay);

        if (audioSource != null)
        {
            audioSource.Play();
            musicStarted = true;
            Debug.Log("🎵 音乐在延迟后开始播放: " + musicDelay.ToString("F3") + "秒");
        }
        else
        {
            Debug.LogError("❌ AudioSource 为空，无法播放音乐！请确保场景中有音频源组件。");
        }
    }

    // 当场景改变时可能需要重新加载谱面
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
        // 检查新加载的场景并相应地加载谱面
        CheckSceneAndLoadBeatmap();
        
        // 如果不是游戏场景，则不需要后续操作
        if (!isGameScene) return;
        
        // 重新查找音频源
        FindAudioSource();
        
        // 重新计算并启动音乐
        CalculateMusicDelay();
        StartCoroutine(DelayedMusicStart());
    }
}