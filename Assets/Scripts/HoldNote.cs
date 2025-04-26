using UnityEngine;

public class HoldNote : MonoBehaviour
{
    public float moveSpeed;
    public float holdDuration = 1.0f;

    private bool isHolding = false;
    private bool isFinished = false;
    private float holdTimer = 0f;
    private bool isReleased = false; // 新增，记录有没有松手

    private static float judgementLineX = 3f;

    private SpriteRenderer sr;
    private Vector3 originalScale;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        if (isFinished) return;

        if (isHolding)
        {
            holdTimer += Time.deltaTime;
            float progress = Mathf.Clamp01(holdTimer / holdDuration);

            // 吃掉自己
            transform.localScale = new Vector3(originalScale.x * (1f - progress), originalScale.y, originalScale.z);

            if (progress >= 1f)
            {
                FinishHold();
            }
        }

        // 松手后，剩下的残留慢慢褪色
        if (isReleased && sr != null)
        {
            Color c = sr.color;
            c.a = Mathf.MoveTowards(c.a, 0f, Time.deltaTime * 0.5f); // 每秒减少0.5透明度
            sr.color = c;
        }

        // 过判定线+一段距离销毁
        if (transform.position.x >= (judgementLineX + 2f))
        {
            Destroy(gameObject);
        }
    }

    public void StartHold()
    {
        if (isFinished) return;
        isHolding = true;
    }

    public void ReleaseHold()
    {
        if (isFinished) return;
        isHolding = false;
        isReleased = true;

        // 刚松手时，把透明度先降到70%
        if (sr != null)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.7f);
        }
    }

    private void FinishHold()
    {
        isFinished = true;
        Destroy(gameObject, 0.2f);
    }
}