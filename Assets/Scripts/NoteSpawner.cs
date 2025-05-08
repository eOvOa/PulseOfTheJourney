using UnityEngine;
using System.Collections.Generic;

public class NoteSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject[] notePrefabs;
    public float startX = -11f;
    public AudioSource audioSource;

    private List<NoteData> tapNotes = new List<NoteData>();
    private int noteIndex = 0;
    private List<GameObject>[] currentNotes = new List<GameObject>[4];

    private float judgementLineX = 2.932941f;
    private float goodWindowValue = 0.3f;

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        }

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
        if (!BeatmapLoader.Instance.musicStarted) return;

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
        
        // Clean up any destroyed notes that might still be in our lists
        for (int i = 0; i < 4; i++)
        {
            currentNotes[i].RemoveAll(note => note == null);
        }
    }

    private void SpawnNote(NoteData noteData)
    {
        int lane = noteData.lane;
        if (lane < 0 || lane >= spawnPoints.Length) return;

        GameObject note = Instantiate(notePrefabs[lane], spawnPoints[lane].position, Quaternion.identity);
        Note noteScript = note.GetComponent<Note>();
        noteScript.moveSpeed = (judgementLineX - startX) / BeatmapLoader.Instance.approachTime;
        noteScript.lane = lane;

        currentNotes[lane].Add(note);
    }

    public List<GameObject> GetActiveTapNotes(int lane)
    {
        if (lane < 0 || lane >= currentNotes.Length)
            return new List<GameObject>();
            
      
        currentNotes[lane].RemoveAll(note => note == null);
        
        return currentNotes[lane];
    }

    public void RemoveTapNote(int lane, GameObject note)
    {
        if (lane >= 0 && lane < currentNotes.Length && currentNotes[lane].Contains(note))
        {
            currentNotes[lane].Remove(note);
        }
    }
    
    public GameObject GetClosestNoteInLane(int lane)
    {
        if (lane < 0 || lane >= currentNotes.Length)
            return null;
            

        List<GameObject> notes = GetActiveTapNotes(lane);
        
        if (notes.Count == 0)
            return null;
            
        GameObject closest = null;
        float closestDist = float.MaxValue;
        
        foreach (var note in notes)
        {
            if (note == null) continue;
            
            Note noteScript = note.GetComponent<Note>();
            
   
            if (noteScript == null || noteScript.IsJudged) continue;
            
            float dist = Mathf.Abs(note.transform.position.x - judgementLineX);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = note;
            }
        }
        
        return closest;
    }
}