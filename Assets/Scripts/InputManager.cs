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
                ProcessKeyPress(lane);
            }
        }
    }
    
    private void ProcessKeyPress(int lane)
    {
        GameObject closestNote = FindClosestNonJudgedNote(lane);
        
        if (closestNote != null)
        {
            Note noteScript = closestNote.GetComponent<Note>();
            if (noteScript != null)
            {
                noteScript.TryJudge();
            }
        }
    }
    
    private GameObject FindClosestNonJudgedNote(int lane)
    {
        List<GameObject> notes = noteSpawner.GetNotesInLane(lane);
        
        GameObject closest = null;
        float closestDistance = float.MaxValue;
        
        foreach (GameObject noteObj in notes)
        {
            if (noteObj == null) continue;
            
            Note note = noteObj.GetComponent<Note>();
            if (note == null || note.judged) continue;
            
            float distance = Mathf.Abs(noteObj.transform.position.x - 2.932941f);
            
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = noteObj;
            }
        }
        
        return closest;
    }
}