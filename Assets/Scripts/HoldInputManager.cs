using UnityEngine;
using System.Collections.Generic;

public class HoldInputManager : MonoBehaviour
{
    public HoldNoteSpawner holdSpawner;
    private Dictionary<int, GameObject> activeHoldNotes = new Dictionary<int, GameObject>();
    
    // 音效相关
    public AudioClip holdStartSound;
    public AudioClip holdReleaseSound;
    private AudioSource audioSource;

    void Awake()
    {
        // 初始化音频源
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            TryPressHold(0);

        if (Input.GetKeyUp(KeyCode.A))
            TryReleaseHold(0);

        if (Input.GetKeyDown(KeyCode.S))
            TryPressHold(1);

        if (Input.GetKeyUp(KeyCode.S))
            TryReleaseHold(1);

        if (Input.GetKeyDown(KeyCode.D))
            TryPressHold(2);

        if (Input.GetKeyUp(KeyCode.D))
            TryReleaseHold(2);

        if (Input.GetKeyDown(KeyCode.F))
            TryPressHold(3);

        if (Input.GetKeyUp(KeyCode.F))
            TryReleaseHold(3);
    }

    private void TryPressHold(int lane)
    {
        GameObject nearestHoldNote = FindNearestHoldNote(lane);
        if (nearestHoldNote == null) return;

        HoldNote holdNote = nearestHoldNote.GetComponent<HoldNote>();
        holdNote.PlayerPress();
        
        // 播放Hold开始音效
        PlaySound(holdStartSound);
        
        // Store the active hold note for this lane
        activeHoldNotes[lane] = nearestHoldNote;
    }

    private void TryReleaseHold(int lane)
    {
        // Use the stored hold note instead of finding it again
        if (activeHoldNotes.TryGetValue(lane, out GameObject holdNoteObj))
        {
            if (holdNoteObj != null)
            {
                HoldNote holdNote = holdNoteObj.GetComponent<HoldNote>();
                if (holdNote != null)
                {
                    bool wasAllowedRelease = holdNote.CheckAllowedRelease();
                    holdNote.PlayerRelease();
                    
                    // 只有成功释放时才播放音效
                    if (wasAllowedRelease)
                    {
                        PlaySound(holdReleaseSound);
                    }
                }
            }
            // Remove from active notes
            activeHoldNotes.Remove(lane);
        }
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
    
    // 播放音效的方法
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}