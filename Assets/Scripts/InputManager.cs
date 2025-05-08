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
        // Get the closest note in this lane that hasn't been judged yet
        GameObject closestNoteObj = noteSpawner.GetClosestNoteInLane(lane);
        
        if (closestNoteObj != null)
        {
            Note closestNote = closestNoteObj.GetComponent<Note>();
            if (closestNote != null && !closestNote.IsJudged)
            {
                // Let this note judge itself independently
                closestNote.TryJudge();
            }
        }
    }
}