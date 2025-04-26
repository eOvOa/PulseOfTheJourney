using UnityEngine;

public class HoldNoteSpawner : MonoBehaviour
{
    public Transform spawnPoint; // 只有蓝轨一个spawn点
    public GameObject holdPrefab; // 只有蓝Hold一个Prefab
    public float startX = -11f;
    public float approachTime = 3f;

    private GameObject currentHoldNote;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SpawnHoldNote();
        }
    }

    public void SpawnHoldNote()
    {
        currentHoldNote = Instantiate(holdPrefab, spawnPoint.position, Quaternion.identity);
        HoldNote holdNote = currentHoldNote.GetComponent<HoldNote>();
        holdNote.moveSpeed = (3f - startX) / approachTime;
    }

    public GameObject GetCurrentHoldNote()
    {
        return currentHoldNote;
    }

    public void ClearHoldNote()
    {
        currentHoldNote = null;
    }
}