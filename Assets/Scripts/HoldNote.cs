using UnityEngine;

public class HoldNote : MonoBehaviour
{
    public float moveSpeed;
    public int lane;
    private bool judged = false;

    private static float judgementLineX = 2.932941f;
    private static float perfectWindow = 0.05f;
    private static float goodWindow = 0.15f;

    void Update()
    {
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        if (!judged && transform.position.x > judgementLineX + goodWindow)
        {
            Miss();
        }
    }

    public void TryJudge()
    {
        if (judged) return;

        float distance = Mathf.Abs(transform.position.x - judgementLineX);

        if (distance <= perfectWindow)
        {
            Debug.Log("Perfect Hold!");
            ScoreManager.Instance.AddScore(500);
            judged = true;
            Destroy(gameObject);
        }
        else if (distance <= goodWindow)
        {
            Debug.Log("Good Hold!");
            ScoreManager.Instance.AddScore(200);
            judged = true;
            Destroy(gameObject);
        }
        else
        {
            Miss();
        }
    }

    private void Miss()
    {
        if (!judged)
        {
            Debug.Log("Miss Hold");
            ScoreManager.Instance.SubtractScore(200);
            judged = true;
            Destroy(gameObject);
        }
    }
}