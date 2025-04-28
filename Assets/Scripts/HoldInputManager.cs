using UnityEngine;
using System.Collections.Generic;

public class HoldInputManager : MonoBehaviour
{
    public HoldNoteSpawner holdSpawner;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            TryPressHold(0);

        if (Input.GetKeyUp(KeyCode.A))
            TryReleaseHold(0);

        if (Input.GetKeyDown(KeyCode.S))
            TryPressHold(1);

        if (Input.GetKeyUp(KeyCode.S))
            TryReleaseHold(1);

        if (Input.GetKeyDown(KeyCode.D))
            TryPressHold(2);

        if (Input.GetKeyUp(KeyCode.D))
            TryReleaseHold(2);

        if (Input.GetKeyDown(KeyCode.F))
            TryPressHold(3);

        if (Input.GetKeyUp(KeyCode.F))
            TryReleaseHold(3);
    }

    private void TryPressHold(int lane)
    {
        GameObject nearestHoldNote = FindNearestHoldNote(lane);
        if (nearestHoldNote == null) return;

        HoldNote holdNote = nearestHoldNote.GetComponent<HoldNote>();
        holdNote.PlayerPress();
    }

    private void TryReleaseHold(int lane)
    {
        GameObject nearestHoldNote = FindNearestHoldNote(lane);
        if (nearestHoldNote == null) return;

        HoldNote holdNote = nearestHoldNote.GetComponent<HoldNote>();
        holdNote.PlayerRelease();
    }

    private GameObject FindNearestHoldNote(int lane)
    {
        List<GameObject> notes = holdSpawner.GetActiveHoldNotes(lane);
        if (notes.Count == 0) return null;

        GameObject nearest = null;
        float minDistance = float.MaxValue;

        foreach (GameObject note in notes)
        {
            if (note == null) continue;
            HoldNote holdNote = note.GetComponent<HoldNote>();

            if (holdNote != null && holdNote.CanBePressed())
            {
                float distance = Mathf.Abs(note.transform.position.x - 2.932941f);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = note;
                }
            }
        }

        return nearest;
    }
}
