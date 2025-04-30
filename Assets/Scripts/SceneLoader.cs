using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private bool hasLoaded = false;

    void Update()
    {
        if (!hasLoaded && Input.anyKeyDown)
        {
            hasLoaded = true;
            SceneManager.LoadScene("Game");
        }
    }
}