using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject[] notePrefabs;
    public float startX = -11f;
    public float approachTime = 3f;

    private GameObject[] currentNotes = new GameObject[4];

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SpawnNote(0);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SpawnNote(1);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SpawnNote(2);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            SpawnNote(3);
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