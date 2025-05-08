using UnityEngine;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public NoteSpawner noteSpawner;
    private KeyCode[] laneKeys = { KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F };
    
    // 缓存可按的音符列表，避免每次都搜索全部音符
    private Dictionary<int, List<GameObject>> pressableNotes = new Dictionary<int, List<GameObject>>(4);
    
    private void Start()
    {
        // 初始化字典
        for (int i = 0; i < 4; i++)
        {
            pressableNotes[i] = new List<GameObject>();
        }
    }

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
    
    // 优化版本的最近音符查找
    private GameObject FindClosestNonJudgedNote(int lane)
    {
        List<GameObject> notes = noteSpawner.GetNotesInLane(lane);
        
        GameObject closest = null;
        float closestDistance = float.MaxValue;
        
        // 使用更高效的遍历方式
        for (int i = 0; i < notes.Count; i++)
        {
            GameObject noteObj = notes[i];
            if (noteObj == null) continue;
            
            Note note = noteObj.GetComponent<Note>();
            if (note == null || note.judged) continue;
            
            // 使用缓存的判定线位置
            float distance = Mathf.Abs(noteObj.transform.position.x - Note.JudgementLineX);
            
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = noteObj;
            }
        }
        
        return closest;
    }
    
    // 添加可按音符到列表
    public void AddPressableNote(int lane, GameObject note)
    {
        if (pressableNotes.ContainsKey(lane))
        {
            pressableNotes[lane].Add(note);
        }
    }
    
    // 从列表移除音符
    public void RemovePressableNote(int lane, GameObject note)
    {
        if (pressableNotes.ContainsKey(lane))
        {
            pressableNotes[lane].Remove(note);
        }
    }
}