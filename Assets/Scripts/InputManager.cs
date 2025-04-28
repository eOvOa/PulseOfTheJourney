using UnityEngine;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public NoteSpawner noteSpawner;

    private KeyCode[] laneKeys = { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F };

    void Update()
    {
        for (int lane = 0; lane < laneKeys.Length; lane++)
        {
            if (Input.GetKeyDown(laneKeys[lane]))
            {
                TryJudgeTap(lane);
            }
        }
    }

    private void TryJudgeTap(int lane)
    {
        List<GameObject> notes = noteSpawner.GetActiveTapNotes(lane);
        if (notes == null || notes.Count == 0) return;

        float bestDistance = float.MaxValue;
        Note bestNote = null;

        foreach (var obj in notes)
        {
            if (obj == null) continue;

            Note note = obj.GetComponent<Note>();
            if (note == null) continue;

            float distance = Mathf.Abs(note.transform.position.x - 2.932941f);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestNote = note;
            }
        }

        if (bestNote != null)
        {
            bestNote.TryJudge();
            noteSpawner.RemoveTapNote(lane, bestNote.gameObject); // 判定后移除
        }
    }
}