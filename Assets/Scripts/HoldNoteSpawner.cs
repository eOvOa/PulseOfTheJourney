using UnityEngine;
using System.Collections.Generic;

public class HoldNoteSpawner : MonoBehaviour
{
    public Transform[] spawnPoints; // 红蓝白绿轨道 spawn points
    public GameObject[] holdPrefabs; // 16个Prefab：红蓝白绿 * 1-4拍
    public float startX = -11f;
    public float approachTime = 3f;

    private List<GameObject>[] activeHoldNotes = new List<GameObject>[4];

    [Header("Auto Test Settings")]
    public bool autoTest = false;  // 是否启用自动测试
    private int currentTestIndex = 0;
    private float testTimer = 0f;
    public float testInterval = 3f; // 每隔几秒生成一个

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            activeHoldNotes[i] = new List<GameObject>();
        }
    }

    void Update()
    {
        if (autoTest)
        {
            testTimer += Time.deltaTime;
            if (testTimer >= testInterval)
            {
                testTimer = 0f;
                SpawnTestHold();
            }
        }
    }

    public void SpawnHoldNote(int lane, int holdLength)
    {
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

    private void SpawnTestHold()
    {
        if (currentTestIndex >= holdPrefabs.Length)
        {
            Debug.Log("测试完成，所有HoldNote已生成！");
            autoTest = false; // 测完自动停止
            return;
        }

        int lane = currentTestIndex / 4; // 每4个属于一个轨道
        int holdLength = (currentTestIndex % 4) + 1; // 1-4拍

        SpawnHoldNote(lane, holdLength);

        Debug.Log($"生成测试：轨道 {lane}，长度 {holdLength}拍");

        currentTestIndex++;
    }
}
