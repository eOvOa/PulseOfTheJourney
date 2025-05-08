using UnityEngine;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public NoteSpawner noteSpawner;
    private KeyCode[] laneKeys = { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F };
    
    // For debugging
    private bool[] keyPressedThisFrame = new bool[4];

    void Update()
    {
        // Reset key pressed tracking
        for (int i = 0; i < keyPressedThisFrame.Length; i++)
        {
            keyPressedThisFrame[i] = false;
        }
        
        // Process key presses
        for (int lane = 0; lane < laneKeys.Length; lane++)
        {
            if (Input.GetKeyDown(laneKeys[lane]))
            {
                keyPressedThisFrame[lane] = true;
                ProcessKeyPress(lane);
            }
        }
    }
    
    private void ProcessKeyPress(int lane)
    {
        // Debug log
        Debug.Log($"Key pressed for lane {lane}");
        
        // Find the closest non-judged note
        GameObject closestNote = FindClosestNonJudgedNote(lane);
        
        if (closestNote != null)
        {
            Note noteScript = closestNote.GetComponent<Note>();
            if (noteScript != null)
            {
                // Let the note handle its own judgment
                noteScript.TryJudge();
            }
        }
        else
        {
            Debug.Log($"No valid notes found in lane {lane}");
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
            
            // Calculate distance to judgment line
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