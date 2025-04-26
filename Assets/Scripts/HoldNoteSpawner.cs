using UnityEngine;

public class HoldNoteSpawner : MonoBehaviour
{
    public Transform[] spawnPoints; // 红 蓝 白 绿轨
    public GameObject[] holdPrefabs; // 红Hold 蓝Hold 白Hold 绿Hold
    public float startX = -11f;
    public float approachTime = 3f;

    private GameObject[] currentHoldNotes = new GameObject[4];

    void Update()
    {
        // 测试阶段只开Q轨（蓝色）
        if (Input.GetKeyDown(KeyCode.Q))
            SpawnHold(1); // 蓝Hold轨道
    }

    public void SpawnHold(int lane)
    {
        GameObject note = Instantiate(holdPrefabs[lane], spawnPoints[lane].position, Quaternion.identity);
        HoldNode hold = note.GetComponent<HoldNode>();
        hold.moveSpeed = (3f - startX) / approachTime;
        currentHoldNotes[lane] = note;
    }

    public void TryHoldPress(int lane)
    {
        if (currentHoldNotes[lane] == null) return;
        HoldNode hold = currentHoldNotes[lane].GetComponent<HoldNode>();
        hold.PlayerPress();
    }

    public void TryHoldRelease(int lane)
    {
        if (currentHoldNotes[lane] == null) return;
        HoldNode hold = currentHoldNotes[lane].GetComponent<HoldNode>();
        hold.PlayerRelease();
    }
}