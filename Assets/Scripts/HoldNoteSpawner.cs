using UnityEngine;
using System.Collections.Generic;

public class HoldNoteSpawner : MonoBehaviour
{
    public Transform[] spawnPoints; // 红蓝白绿，按顺序
    public GameObject[] holdPrefabs; // 16个Prefab，按颜色分段排列
    public float startX = -11f;
    public float approachTime = 3f;

    // 每条轨道一个活跃Hold列表
    private List<GameObject>[] activeHoldNotes = new List<GameObject>[4];

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            activeHoldNotes[i] = new List<GameObject>();
        }
    }

    public void SpawnHoldNote(int lane, int holdLength) 
    {
        // lane = 0红, 1蓝, 2白, 3绿
        // holdLength = 1, 2, 3, 4拍

        int prefabIndex = lane * 4 + (holdLength - 1);

        if (prefabIndex >= 0 && prefabIndex < holdPrefabs.Length)
        {
            GameObject prefab = holdPrefabs[prefabIndex];
            GameObject holdNoteObj = Instantiate(prefab, spawnPoints[lane].position, Quaternion.identity);

            HoldNote holdNote = holdNoteObj.GetComponent<HoldNote>();
            holdNote.moveSpeed = (2.932941f - startX) / approachTime;

            activeHoldNotes[lane].Add(holdNoteObj);
        }
        else
        {
            Debug.LogError("Prefab index out of range!");
        }
    }

    public List<GameObject> GetActiveHoldNotes(int lane)
    {
        return activeHoldNotes[lane];
    }

    public void RemoveHoldNote(int lane, GameObject note)
    {
        if (activeHoldNotes[lane].Contains(note))
        {
            activeHoldNotes[lane].Remove(note);
        }
    }
}