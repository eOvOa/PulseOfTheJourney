using UnityEngine;

public class HoldNoteSpawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject[] holdPrefabs;
    public float startX = -11f;

    [SerializeField] private AudioSource audioSource;

    public void SpawnHoldNote(int lane, int holdLength)
    {
        int prefabIndex = lane * 4 + (holdLength - 1);

        if (prefabIndex >= 0 && prefabIndex < holdPrefabs.Length)
        {
            GameObject prefab = holdPrefabs[prefabIndex];
            GameObject holdNoteObj = Instantiate(prefab, spawnPoints[lane].position, Quaternion.identity);

            HoldNote holdNote = holdNoteObj.GetComponent<HoldNote>();
            holdNote.moveSpeed = (2.932941f - startX) / BeatmapLoader.Instance.approachTime;
        }
        else
        {
            Debug.LogError("Hold prefab index out of range!");
        }
    }
}