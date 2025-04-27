using UnityEngine;
using System.Collections.Generic;

public class HoldInputManager : MonoBehaviour
{
    public HoldNoteSpawner holdNoteSpawner;

    private KeyCode[] laneKeys = { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F };

    private bool[] holdingKeys = new bool[4];

    void Update()
    {
        for (int lane = 0; lane < laneKeys.Length; lane++)
        {
            if (Input.GetKeyDown(laneKeys[lane]))
            {
                holdingKeys[lane] = true;
            }
            if (Input.GetKeyUp(laneKeys[lane]))
            {
                holdingKeys[lane] = false;
            }
        }

        JudgeHoldNotes();
    }

    private void JudgeHoldNotes()
    {
        for (int lane = 0; lane < 4; lane++)
        {
            List<GameObject> notes = holdNoteSpawner.GetActiveHoldNotes(lane);
            if (notes == null) continue;

            foreach (var obj in notes.ToArray())
            {
                if (obj == null) continue;

                HoldNote holdNote = obj.GetComponent<HoldNote>();
                if (holdNote == null) continue;

                if (holdingKeys[lane])
                {
                    holdNote.OnHold(lane);
                }
                else
                {
                    holdNote.OnRelease(lane);
                }
            }
        }
    }
}