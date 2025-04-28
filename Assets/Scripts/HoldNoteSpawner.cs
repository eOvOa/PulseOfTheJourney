using UnityEngine;
using System.Collections.Generic;

public class HoldNoteSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject[] holdPrefabs;
    public float startX = -11f;
    public AudioSource audioSource;

    private List<NoteData> holdNotes = new List<NoteData>();
    private int noteIndex = 0;
    private List<GameObject>[] activeHoldNotes = new List<GameObject>[4];

    void Start()
    {
        if (audioSource == null)
        {
            audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        }

        for (int i = 0; i < 4; i++)
        {
            activeHoldNotes[i] = new List<GameObject>();
        }

        if (BeatmapLoader.Instance != null)
        {
            foreach (var note in BeatmapLoader.Instance.notes)
            {
                if (note.type == "Hold")
                {
                    holdNotes.Add(note);
                }
            }
        }
    }

    void Update()
    {
        if (!BeatmapLoader.Instance.musicStarted) return;

        if (noteIndex < holdNotes.Count)
        {
            float musicTime = audioSource.time;
            float approachTime = BeatmapLoader.Instance.approachTime;

            if (musicTime + approachTime >= holdNotes[noteIndex].time)
            {
                SpawnHoldNote(holdNotes[noteIndex]);
                noteIndex++;
            }
        }
    }

    private void SpawnHoldNote(NoteData noteData)
    {
        int lane = noteData.lane;
        int holdLength = noteData.holdLength;

        int prefabIndex = lane * 4 + (holdLength - 1);

        if (prefabIndex >= 0 && prefabIndex < holdPrefabs.Length)
        {
            GameObject prefab = holdPrefabs[prefabIndex];
            GameObject holdNoteObj = Instantiate(prefab, spawnPoints[lane].position, Quaternion.identity);

            HoldNote holdNote = holdNoteObj.GetComponent<HoldNote>();
            holdNote.moveSpeed = (2.932941f - startX) / BeatmapLoader.Instance.approachTime;
            holdNote.lane = lane; 

            activeHoldNotes[lane].Add(holdNoteObj);
        }
    }

    public List<GameObject> GetActiveHoldNotes(int lane)
    {
        return activeHoldNotes[lane];
    }

    public void RemoveHoldNote(int lane, GameObject note)
    {
        if (activeHoldNotes[lane].Contains(note))
        {
            activeHoldNotes[lane].Remove(note);
        }
    }
}
