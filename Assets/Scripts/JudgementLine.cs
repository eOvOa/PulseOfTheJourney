using UnityEngine;
using System.Collections.Generic;

public class JudgementLine : MonoBehaviour
{
    private List<Note> notesInRange = new List<Note>();
    private List<HoldNote> holdNotesInRange = new List<HoldNote>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Note"))
        {
            Note note = other.GetComponent<Note>();
            if (note != null) notesInRange.Add(note);
        }
        else if (other.CompareTag("HoldNote"))
        {
            HoldNote holdNote = other.GetComponent<HoldNote>();
            if (holdNote != null) holdNotesInRange.Add(holdNote);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Note"))
        {
            Note note = other.GetComponent<Note>();
            if (note != null) notesInRange.Remove(note);
        }
        else if (other.CompareTag("HoldNote"))
        {
            HoldNote holdNote = other.GetComponent<HoldNote>();
            if (holdNote != null) holdNotesInRange.Remove(holdNote);
        }
    }

    public void TryJudge(int lane)
    {
        // 先判Tap
        for (int i = 0; i < notesInRange.Count; i++)
        {
            if (notesInRange[i] != null && notesInRange[i].lane == lane)
            {
                notesInRange[i].TryJudge();
                notesInRange.RemoveAt(i);
                return;
            }
        }
        // 再判Hold
        for (int i = 0; i < holdNotesInRange.Count; i++)
        {
            if (holdNotesInRange[i] != null && holdNotesInRange[i].lane == lane)
            {
                holdNotesInRange[i].TryJudge();
                holdNotesInRange.RemoveAt(i);
                return;
            }
        }
    }
}