using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public Transform[] spawnPoints; // 四条轨道的生成位置（红蓝白绿）
    public GameObject[] notePrefabs; // 四种颜色的音符Prefab
    public float startX = -11f;
    public float approachTime = 3f;

    private GameObject[] currentNotes = new GameObject[4]; // 当前四轨的音符引用

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SpawnNote(0); // 红轨
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SpawnNote(1); // 蓝轨
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SpawnNote(2); // 白轨
        if (Input.GetKeyDown(KeyCode.Alpha4))
            SpawnNote(3); // 绿轨
    }

    public void SpawnNote(int lane)
    {
        GameObject note = Instantiate(notePrefabs[lane], spawnPoints[lane].position, Quaternion.identity);
        Note noteScript = note.GetComponent<Note>();
        noteScript.moveSpeed = (3f - startX) / approachTime;
        currentNotes[lane] = note;
    }

    public GameObject GetCurrentNote(int lane)
    {
        return currentNotes[lane];
    }

    public void ClearNote(int lane)
    {
        currentNotes[lane] = null;
    }
}