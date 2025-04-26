using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public Transform[] spawnPoints; // 四条轨道生成点
    public GameObject[] tapPrefabs;  // 红蓝白绿四个Tap音符Prefab
    public float startX = -5f;       // 音符出生的X位置
    public float approachTime = 2.5f;  // 预计从出生点到判定线的时间（秒）

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnNote(0); // 红轨道
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SpawnNote(1); // 蓝轨道
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SpawnNote(2); // 白轨道
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SpawnNote(3); // 绿轨道
        }
    }

    void SpawnNote(int lane)
    {
        GameObject prefab = tapPrefabs[lane];

        Vector3 spawnPos = new Vector3(startX, spawnPoints[lane].position.y, 0);
        GameObject noteObj = Instantiate(prefab, spawnPos, Quaternion.identity);

        Note note = noteObj.GetComponent<Note>();
        note.moveSpeed = (3f - startX) / approachTime;

        FindFirstObjectByType<InputManager>().RegisterNote(lane, note);
    }
}