using UnityEngine;
using System.Collections.Generic;

public class HoldNoteSpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject holdPrefab;
    public float startX = -11f;
    public float approachTime = 3f;

    private List<GameObject> activeHoldNotes = new List<GameObject>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SpawnHoldNote();
        }
    }

    public void SpawnHoldNote()
    {
        GameObject holdNoteObj = Instantiate(holdPrefab, spawnPoint.position, Quaternion.identity);
        HoldNote holdNote = holdNoteObj.GetComponent<HoldNote>();
        holdNote.moveSpeed = (2.932941f - startX) / approachTime;

        activeHoldNotes.Add(holdNoteObj);
    }

    public List<GameObject> GetActiveHoldNotes()
    {
        return activeHoldNotes;
    }

    public void RemoveHoldNote(GameObject note)
    {
        if (activeHoldNotes.Contains(note))
        {
            activeHoldNotes.Remove(note);
        }
    }
}