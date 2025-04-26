using UnityEngine;

public class HoldInputManager : MonoBehaviour
{
    public HoldNoteSpawner holdSpawner;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) 
        {
            TryJudgeHold();
        }
    }

    private void TryJudgeHold()
    {
        GameObject holdObj = holdSpawner.GetCurrentHoldNote();
        if (holdObj == null) return;

        HoldNote holdNote = holdObj.GetComponent<HoldNote>();
        holdNote.TryJudge();

        holdSpawner.ClearHoldNote();
    }
}