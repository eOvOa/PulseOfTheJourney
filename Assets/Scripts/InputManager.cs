using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public NoteSpawner noteSpawner;
    private Dictionary<int, List<GameObject>> pressableNotes = new Dictionary<int, List<GameObject>>();
    
    // 音效相关
    public AudioClip tapHitSound;
    private AudioSource audioSource;
    
    void Awake()
    {
        // 初始化音频源
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        for (int i = 0; i < 4; i++)
        {
            pressableNotes[i] = new List<GameObject>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            TryJudgeNote(0);

        if (Input.GetKeyDown(KeyCode.S))
            TryJudgeNote(1);

        if (Input.GetKeyDown(KeyCode.D))
            TryJudgeNote(2);

        if (Input.GetKeyDown(KeyCode.F))
            TryJudgeNote(3);
    }

    private void TryJudgeNote(int lane)
    {
        if (pressableNotes[lane].Count > 0)
        {
            foreach (GameObject noteObj in pressableNotes[lane])
            {
                if (noteObj == null) continue;
                
                Note note = noteObj.GetComponent<Note>();
                if (note != null && !note.judged)
                {
                    note.TryJudge();
                    
                    // 如果音符被成功击中，播放音效
                    if (note.judged && !note.missed)
                    {
                        PlaySound(tapHitSound);
                    }
                    
                    break;
                }
            }
        }
    }
    
    // 播放音效的方法
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    public void AddPressableNote(int lane, GameObject note)
    {
        if (lane >= 0 && lane < 4 && !pressableNotes[lane].Contains(note))
        {
            pressableNotes[lane].Add(note);
        }
    }

    public void RemovePressableNote(int lane, GameObject note)
    {
        if (lane >= 0 && lane < 4 && pressableNotes[lane].Contains(note))
        {
            pressableNotes[lane].Remove(note);
        }
    }
}