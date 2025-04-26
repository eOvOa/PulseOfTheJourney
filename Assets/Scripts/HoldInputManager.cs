using UnityEngine;
using System.Collections.Generic;

public class HoldInputManager : MonoBehaviour
{
    public HoldNoteSpawner holdSpawner;

    void Update()
    {
        // 四条轨道，Q W E R分别控制
        if (Input.GetKeyDown(KeyCode.Q))
            TryPressHold(0); // 红

        if (Input.GetKeyUp(KeyCode.Q))
            TryReleaseHold(0);

        if (Input.GetKeyDown(KeyCode.W))
            TryPressHold(1); // 蓝

        if (Input.GetKeyUp(KeyCode.W))
            TryReleaseHold(1);

        if (Input.GetKeyDown(KeyCode.E))
            TryPressHold(2); // 白

        if (Input.GetKeyUp(KeyCode.E))
            TryReleaseHold(2);

        if (Input.GetKeyDown(KeyCode.R))
            TryPressHold(3); // 绿

        if (Input.GetKeyUp(KeyCode.R))
            TryReleaseHold(3);
    }

    private void TryPressHold(int lane)
    {
        GameObject nearestHoldNote = FindNearestHoldNote(lane);
        if (nearestHoldNote == null)
        {
            Debug.Log($"❌ 没找到 {lane} 轨道上可以按的HoldNote！");
            return;
        }

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

            // 只找可以按的！
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
