using UnityEngine;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public List<Note>[] laneNotes = new List<Note>[4]; // 四条轨道上的音符列表

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            laneNotes[i] = new List<Note>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            TryHit(0); // 红轨道
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            TryHit(1); // 蓝轨道
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            TryHit(2); // 白轨道
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            TryHit(3); // 绿轨道
        }
    }

    void TryHit(int lane)
    {
        if (laneNotes[lane].Count == 0) return;

        Note closestNote = null;
        float closestDist = Mathf.Infinity;

        // 遍历轨道上的所有Note，找最近的可以打的
        for (int i = 0; i < laneNotes[lane].Count; i++)
        {
            var note = laneNotes[lane][i];

            // 🛠 关键：如果这个Note已经被销毁了，就跳过
            if (note == null) continue;

            float dist = Mathf.Abs(note.transform.position.x - 3f);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestNote = note;
            }
        }

        // 打击最近的
        if (closestNote != null && closestNote.CanBeHit())
        {
            closestNote.Hit();
            laneNotes[lane].Remove(closestNote);
        }
    }


    public void RegisterNote(int lane, Note note)
    {
        laneNotes[lane].Add(note);
    }
}