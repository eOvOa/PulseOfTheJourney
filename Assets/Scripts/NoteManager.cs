using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public NoteSpawner noteSpawner;
    public HoldNoteSpawner holdNoteSpawner;

    private int noteIndex = 0;

    void Update()
    {
        if (BeatmapLoader.Instance == null || BeatmapLoader.Instance.notes == null)
            return;

        if (!BeatmapLoader.Instance.musicStarted)
            return; // ✅音乐没开始就啥也不做！

        if (noteIndex >= BeatmapLoader.Instance.notes.Count)
            return; // 所有note都生成完了

        float musicTime = BeatmapLoader.Instance.audioSource.time;
        NoteData noteData = BeatmapLoader.Instance.notes[noteIndex];

        if (musicTime >= noteData.time - BeatmapLoader.Instance.approachTime)
        {
            if (noteData.type == "Tap")
            {
                noteSpawner.SpawnNote(noteData.lane);
            }
            else if (noteData.type == "Hold")
            {
                holdNoteSpawner.SpawnHoldNote(noteData.lane, noteData.holdLength);
            }

            noteIndex++;
        }
    }
}