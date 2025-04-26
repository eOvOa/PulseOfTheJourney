using UnityEngine;

public class InputManager : MonoBehaviour
{
    public NoteSpawner noteSpawner;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            TryJudgeNote(0); // 红轨
        if (Input.GetKeyDown(KeyCode.S))
            TryJudgeNote(1); // 蓝轨
        if (Input.GetKeyDown(KeyCode.W))
            TryJudgeNote(2); // 白轨
        if (Input.GetKeyDown(KeyCode.F))
            TryJudgeNote(3); // 绿轨
    }

    private void TryJudgeNote(int lane)
    {
        GameObject noteObj = noteSpawner.GetCurrentNote(lane);
        if (noteObj == null) return;

        Note note = noteObj.GetComponent<Note>();
        note.TryJudge();

        noteSpawner.ClearNote(lane);
    }
}