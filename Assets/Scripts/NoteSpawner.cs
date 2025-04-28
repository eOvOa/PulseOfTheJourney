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
        return currentNotes[lane];
    }

    public void RemoveTapNote(int lane, GameObject note)
    {
        if (currentNotes[lane].Contains(note))
        {
            currentNotes[lane].Remove(note);
        }
    }
}
