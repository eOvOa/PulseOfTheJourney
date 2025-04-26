using UnityEngine;

public class Note : MonoBehaviour
{
    [Header("Movement Settings")]
    public float targetTime;     // 音符应该到达判定线的时间
    public float moveSpeed;      // 移动速度

    private bool isHit = false;  // 是否被击中了
    private float missTimer = 0f;

    private static float judgementLineX = 3f; // 判定线X位置 (不是Y了！)
    private static float hitWindow = 0.3f;     // 判定容差（可以调）

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 向右移动 (!!! 注意是right)
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        // 如果没被击中且明显超过判定线
        if (!isHit && transform.position.x >= (judgementLineX + hitWindow))
        {
            Miss();
        }

        // Miss后计时销毁
        if (missTimer > 0)
        {
            missTimer -= Time.deltaTime;
            if (missTimer <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Hit()
    {
        if (isHit) return;

        isHit = true;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f); // 透明度0%
        Destroy(gameObject, 0.2f);
    }

    private void Miss()
    {
        isHit = true;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.25f); // 透明度25%
        missTimer = 2f;
    }

    // 检查能否打击
    public bool CanBeHit()
    {
        float diff = Mathf.Abs(transform.position.x - judgementLineX);
        return diff <= hitWindow;
    }
}