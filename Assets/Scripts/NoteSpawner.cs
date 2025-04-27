using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject[] notePrefabs;
    public float startX = -11f;

    [SerializeField] private AudioSource audioSource;

    public void SpawnNote(int lane)
    {
        GameObject note = Instantiate(notePrefabs[lane], spawnPoints[lane].position, Quaternion.identity);
        Note noteScript = note.GetComponent<Note>();
        noteScript.moveSpeed = (2.932941f - startX) / BeatmapLoader.Instance.approachTime;
    }
}