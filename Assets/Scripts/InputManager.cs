using UnityEngine;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public List<Note>[] laneNotes = new List<Note>[4]; // 四个轨道，每条轨道上的活跃音符列表

    void Start()
    {
        // 初始化四个轨道的音符表
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

        // 找到最靠近判定线的音符
        Note closestNote = null;
        float closestDist = Mathf.Infinity;

        foreach (var note in laneNotes[lane])
        {
            float dist = Mathf.Abs(note.transform.position.x - 3f); // 判定线X=3
            if (dist < closestDist)
            {
                closestDist = dist;
                closestNote = note;
            }
        }

        if (closestNote != null && closestNote.CanBeHit())
        {
            closestNote.Hit();
            laneNotes[lane].Remove(closestNote);
            // 这里可以加分数、Combo系统
        }
    }

    // 给Spawner调用：生成音符时登记到对应轨道
    public void RegisterNote(int lane, Note note)
    {
        laneNotes[lane].Add(note);
    }
}