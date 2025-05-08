using UnityEngine;
using System.Collections.Generic;

public class NoteSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject[] notePrefabs;
    public float startX = -11f;
    public AudioSource audioSource;

    // Judgment line position for reference
    public float judgementLineX = 2.932941f;
    
    private List<NoteData> tapNotes = new List<NoteData>();
    private int noteIndex = 0;
    private List<GameObject>[] currentNotes;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        }

        // Initialize note lists
        currentNotes = new List<GameObject>[4];
        for (int i = 0; i < 4; i++)
        {
            currentNotes[i] = new List<GameObject>();
        }

        // Load notes from beatmap
        if (BeatmapLoader.Instance != null)
        {
            foreach (var note in BeatmapLoader.Instance.notes)
            {
                if (note.type == "Tap")
                {
                    tapNotes.Add(note);
                }
            }
            
            Debug.Log($"Loaded {tapNotes.Count} tap notes from beatmap");
        }
    }

    void Update()
    {
        // Only spawn notes if music is playing
        if (BeatmapLoader.Instance == null || !BeatmapLoader.Instance.musicStarted) return;

        // Check if we need to spawn a new note
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
        
        // Clean lists (only remove null entries here)
        CleanNoteLists();
    }

    private void SpawnNote(NoteData noteData)
    {
        int lane = noteData.lane;
        if (lane < 0 || lane >= spawnPoints.Length)
        {
            Debug.LogWarning($"Attempted to spawn note in invalid lane: {lane}");
            return;
        }

        // Create the note
        GameObject note = Instantiate(notePrefabs[lane], spawnPoints[lane].position, Quaternion.identity);
        Note noteScript = note.GetComponent<Note>();
        
        if (noteScript != null)
        {
            noteScript.moveSpeed = (judgementLineX - startX) / BeatmapLoader.Instance.approachTime;
            noteScript.lane = lane;
            noteScript.judged = false; // Ensure it starts as not judged
        }

        // Add to tracking list
        currentNotes[lane].Add(note);
        
        Debug.Log($"Spawned note in lane {lane}, time: {noteData.time}");
    }

    // Get all notes in a lane (including judged ones - filtering happens in InputManager)
    public List<GameObject> GetNotesInLane(int lane)
    {
        if (lane < 0 || lane >= currentNotes.Length)
            return new List<GameObject>();
            
        // Return a copy to prevent modification issues
        return new List<GameObject>(currentNotes[lane]);
    }

    // Remove a note from tracking
    public void RemoveTapNote(int lane, GameObject note)
    {
        if (lane < 0 || lane >= currentNotes.Length) return;
        
        if (currentNotes[lane].Contains(note))
        {
            currentNotes[lane].Remove(note);
            Debug.Log($"Removed note from lane {lane} tracking");
        }
    }
    
    // Clean up null entries
    private void CleanNoteLists()
    {
        for (int i = 0; i < currentNotes.Length; i++)
        {
            int beforeCount = currentNotes[i].Count;
            currentNotes[i].RemoveAll(note => note == null);
            int afterCount = currentNotes[i].Count;
            
            if (beforeCount != afterCount)
            {
                Debug.Log($"Cleaned {beforeCount - afterCount} null entries from lane {i}");
            }
        }
    }
    
    // Debug method - call from console
    public void DebugPrintNoteCounts()
    {
        string counts = "Note counts: ";
        for (int i = 0; i < currentNotes.Length; i++)
        {
            int judgedCount = 0;
            foreach (var note in currentNotes[i])
            {
                if (note != null && note.GetComponent<Note>() != null && note.GetComponent<Note>().judged)
                {
                    judgedCount++;
                }
            }
            counts += $"Lane {i}: {currentNotes[i].Count} total, {judgedCount} judged | ";
        }
        Debug.Log(counts);
    }
}