using UnityEngine;
using System.Collections.Generic;

public class NoteSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject[] notePrefabs;
    public float startX = -11f;
    public AudioSource audioSource;
    public float judgementLineX = 2.932941f;
    
    private List<NoteData> tapNotes = new List<NoteData>();
    private int noteIndex = 0;
    private List<GameObject>[] currentNotes;
    
    // 清理计时器，减少每帧清理
    private float cleanupTimer = 0f;
    private const float CLEANUP_INTERVAL = 0.5f;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        }

        currentNotes = new List<GameObject>[4];
        for (int i = 0; i < 4; i++)
        {
            currentNotes[i] = new List<GameObject>();
        }

        if (BeatmapLoader.Instance != null)
        {
            foreach (var note in BeatmapLoader.Instance.notes)
            {
                if (note.type == "Tap")
                {
                    tapNotes.Add(note);
                }
            }
        }
    }

    void Update()
    {
        if (BeatmapLoader.Instance == null || !BeatmapLoader.Instance.musicStarted) return;

        if (noteIndex < tapNotes.Count)
        {
            float musicTime = audioSource.time;
            float approachTime = BeatmapLoader.Instance.approachTime;

            if (musicTime + approachTime >= tapNotes[noteIndex].time)
            {
                SpawnNote(tapNotes[noteIndex]);
                noteIndex++;
            }
        }
        
        // 定时清理而不是每帧清理
        cleanupTimer += Time.deltaTime;
        if (cleanupTimer >= CLEANUP_INTERVAL)
        {
            CleanNoteLists();
            cleanupTimer = 0f;
        }
    }

    private void SpawnNote(NoteData noteData)
    {
        int lane = noteData.lane;
        if (lane < 0 || lane >= spawnPoints.Length) return;

        GameObject note = Instantiate(notePrefabs[lane], spawnPoints[lane].position, Quaternion.identity);
        Note noteScript = note.GetComponent<Note>();
        
        if (noteScript != null)
        {
            noteScript.moveSpeed = (judgementLineX - startX) / BeatmapLoader.Instance.approachTime;
            noteScript.lane = lane;
            noteScript.judged = false;
        }

        currentNotes[lane].Add(note);
    }

    // 返回引用而不是复制，提高性能
    public List<GameObject> GetNotesInLane(int lane)
    {
        if (lane < 0 || lane >= currentNotes.Length)
            return new List<GameObject>();
            
        return currentNotes[lane];
    }

    public void RemoveTapNote(int lane, GameObject note)
    {
        if (lane < 0 || lane >= currentNotes.Length) return;
        
        if (currentNotes[lane].Contains(note))
        {
            currentNotes[lane].Remove(note);
        }
    }
    
    private void CleanNoteLists()
    {
        for (int i = 0; i < currentNotes.Length; i++)
        {
            currentNotes[i].RemoveAll(note => note == null);
        }
    }
}