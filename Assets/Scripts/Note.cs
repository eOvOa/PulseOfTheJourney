using UnityEngine;

public class Note : MonoBehaviour
{
    public float moveSpeed;
    private bool judged = false;
    private bool missed = false;

    private static float judgementLineX = 3f; 
    private static float hitWindow = 0.5f;    

    private SpriteRenderer sr;
    private float missTimer = 0f;
    private bool scheduledDestroy = false;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 不管有没有判定，都要继续走
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        if (!judged)
        {
            // 还没判定过，检查是不是Miss
            if (transform.position.x > judgementLineX + 2f)
            {
                Miss();
            }
        }

        if (missed)
        {
            missTimer += Time.deltaTime;
            if (missTimer >= 2f && !scheduledDestroy)
            {
                Destroy(gameObject);
                scheduledDestroy = true;
            }
        }
    }

    public void TryJudge()
    {
        if (judged) return;

        float distance = Mathf.Abs(transform.position.x - judgementLineX);
        if (distance <= hitWindow)
        {
            Hit();
        }
        else
        {
            Miss();
        }
    }

    private void Hit()
    {
        judged = true;
        Destroy(gameObject);
    }

    private void Miss()
    {
        judged = true;
        missed = true;

        if (sr != null)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.25f); // 变灰
        }
    }
}