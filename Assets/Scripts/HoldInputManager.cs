using UnityEngine;
using System.Collections.Generic;

public class HoldInputManager : MonoBehaviour
{
    public HoldNoteSpawner holdSpawner;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TryPressHold();
        }

        if (Input.GetKeyUp(KeyCode.K))
        {
            TryReleaseHold();
        }
    }

    private void TryPressHold()
    {
        GameObject nearestHoldNote = FindNearestHoldNote();
        if (nearestHoldNote == null) return;

        HoldNote holdNote = nearestHoldNote.GetComponent<HoldNote>();
        holdNote.PlayerPress();
    }

    private void TryReleaseHold()
    {
        GameObject nearestHoldNote = FindNearestHoldNote();
        if (nearestHoldNote == null) return;

        HoldNote holdNote = nearestHoldNote.GetComponent<HoldNote>();
        holdNote.PlayerRelease();
    }

    private GameObject FindNearestHoldNote()
    {
        List<GameObject> notes = holdSpawner.GetActiveHoldNotes();
        if (notes.Count == 0) return null;

        GameObject nearest = null;
        float minDistance = float.MaxValue;

        foreach (GameObject note in notes)
        {
            if (note == null) continue;
            float distance = Mathf.Abs(note.transform.position.x - 2.932941f);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = note;
            }
        }

        return nearest;
    }
}