using UnityEngine;

public class Note : MonoBehaviour
{
    [Header("Movement Settings")]
    public float targetTime;     // 音符应该到达判定线的时间（现在不用）
    public float moveSpeed;      // 移动速度

    private bool isHit = false;  // 是否被击中了
    private float missTimer = 0f;

    private static float judgementLineX = 3f; // 判定线的X位置
    private static float hitWindow = 0.3f;     // 判定窗口（可打击范围）

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 音符沿着x轴向右移动
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);

        // 没击中且超过判定线 -> Miss
        if (!isHit && transform.position.x >= (judgementLineX + hitWindow))
        {
            Miss();
        }

        // Miss后倒计时销毁
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
        if (isHit) return; // 防止多次击中

        isHit = true;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f); // 透明度0%
        Destroy(gameObject, 0.2f); // 稍微延迟销毁
    }

    private void Miss()
    {
        isHit = true;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.25f); // 透明度25%
        missTimer = 2f;
    }

    // 检查是否能被打击
    public bool CanBeHit()
    {
        float diff = Mathf.Abs(transform.position.x - judgementLineX);
        return diff <= hitWindow;
    }
}