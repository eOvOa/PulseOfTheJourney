using UnityEngine;

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
        GameObject noteObj = noteSpawner.GetCurrentNote(lane);
        if (noteObj == null) return;

        Note note = noteObj.GetComponent<Note>();
        if (note == null) return;

        note.TryJudge();
    }
}